using System;

namespace LiquidProjections.ExampleWebHost.Events
{
    internal class LifetimeRestrictedEvent
    {
        public DateTime PeriodEnd { get; set; }
        public string DocumentNumber { get; set; }
    }
}