﻿namespace LiquidProjections.ExampleWebHost.Events
{
    internal class ValidityPeriodCanceledEvent
    {
        public string DocumentNumber { get; set; }
        public int Sequence { get; set; }
    }
}