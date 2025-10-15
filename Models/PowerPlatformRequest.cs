namespace ServiceCatalogAPI.Models
{
    public class PowerPlatformRequest
    {
        public string? Id { get; set; }
        public string TypeOfRequest { get; set; }
        public string EnvironmentName { get; set; }
        public string Comments { get; set; }
    }
}
