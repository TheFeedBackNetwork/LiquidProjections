﻿namespace LiquidProjections.ExampleWebHost.Events
{
    internal class StateTransitionedEvent
    {
        public string State { get; set; }
        public string DocumentNumber { get; set; }
    }
}