using System;

namespace LiquidProjections.ExampleWebHost.Events
{
    internal class NextReviewScheduledEvent
    {
        public string DocumentNumber { get; set; }
        public DateTime NextReviewAt { get; set; }
    }
}