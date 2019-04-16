using System;

namespace LiquidProjections.ExampleWebHost.Events
{
    internal class CountryCorrectedEvent
    {
        public string DocumentNumber { get; set; }
        public Guid Country { get; set; }
    }
}