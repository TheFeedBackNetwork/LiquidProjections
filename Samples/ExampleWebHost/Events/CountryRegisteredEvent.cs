namespace LiquidProjections.ExampleWebHost.Events
{
    internal class CountryRegisteredEvent
    {
        public string Code { get; set; }
        public string Name{ get; set; }
    }
}