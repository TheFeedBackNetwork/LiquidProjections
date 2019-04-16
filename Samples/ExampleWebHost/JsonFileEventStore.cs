﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LiquidProjections.Abstractions;
using Newtonsoft.Json;

namespace LiquidProjections.ExampleWebHost
{
    public class JsonFileEventStore : IDisposable
    {
        private const int AverageEventsPerTransaction = 6;
        private readonly int pageSize;
        private ZipArchive zip;
        private readonly Queue<ZipArchiveEntry> entryQueue;
        private StreamReader currentReader = null;
        private static long lastCheckpoint = 0;

        public JsonFileEventStore(
            string filePath,
            int pageSize)
        {
            this.pageSize = pageSize;
            zip = ZipFile.Open(filePath, ZipArchiveMode.Read);
            entryQueue = new Queue<ZipArchiveEntry>(zip.Entries.Where(e => e.Name.EndsWith(".json")));
        }

        public IDisposable Subscribe(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId)
        {
            var subscription = new Subscription(
                lastProcessedCheckpoint ?? 0, 
                transactions => subscriber.HandleTransactions(transactions, null));
            
            Task.Run(async () =>
            {
                Task<Transaction[]> loader = LoadNextPageAsync();
                Transaction[] transactions = await loader;

                while (transactions.Length > 0)
                {
                    // Start loading the next page on a separate thread while we have the subscriber handle the previous transactions.
                    loader = LoadNextPageAsync();

                    await subscription.Send(transactions);

                    transactions = await loader;
                }
            });

            return subscription;
        }

        private Task<Transaction[]> LoadNextPageAsync()
        {
            return Task.Run(() =>
            {
                var transactions = new List<Transaction>();

                var transaction = new Transaction
                {
                    Checkpoint = ++lastCheckpoint
                };

                string json;

                do
                {
                    json = CurrentReader.ReadLine();

                    if (json != null)
                    {
                        transaction.Events.Add(new EventEnvelope
                        {
                            Body = JsonConvert.DeserializeObject(json, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All,
                                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                            })
                        });
                    }

                    if ((transaction.Events.Count == AverageEventsPerTransaction) || (json == null))
                    {
                        if (transaction.Events.Count > 0)
                        {
                            transactions.Add(transaction);
                        }

                        transaction = new Transaction
                        {
                            Checkpoint = ++lastCheckpoint
                        };
                    }
                }
                while ((json != null) && (transactions.Count < pageSize));

                return transactions.ToArray();
            });
        }

        private StreamReader CurrentReader => 
            currentReader ?? (currentReader = new StreamReader(entryQueue.Dequeue().Open()));

        public void Dispose()
        {
            zip.Dispose();
            zip = null;
        }

        internal class Subscription : IDisposable
        {
            private readonly long lastProcessedCheckpoint;
            private readonly Func<IReadOnlyList<Transaction>, Task> handler;
            private bool disposed;

            public Subscription(long lastProcessedCheckpoint, Func<IReadOnlyList<Transaction>, Task> handler)
            {
                this.lastProcessedCheckpoint = lastProcessedCheckpoint;
                this.handler = handler;
            }

            public async Task Send(IEnumerable<Transaction> transactions)
            {
                if (!disposed)
                {
                    Transaction[] readOnlyList = transactions.Where(t => t.Checkpoint > lastProcessedCheckpoint).ToArray();
                    if (readOnlyList.Length > 0)
                    {
                        await handler(readOnlyList);
                    }
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
            }

            public void Dispose()
            {
                disposed = true;
            }
        }
    }
}