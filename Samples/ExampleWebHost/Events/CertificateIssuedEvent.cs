using System;

namespace LiquidProjections.ExampleWebHost.Events
{
    internal class CertificateIssuedEvent
    {
        public string Number { get; set; }
        public string Kind { get; set; }
        public Guid Country { get; set; }
        public string InitialState { get; set; }
    }
}