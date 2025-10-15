namespace ServiceCatalogAPI.Models
{
    public class PowerPlatformRequest
    {
        public string? Id { get; set; }
        public string TypeOfRequest { get; set; }
        public string EnvironmentName { get; set; }
    public string? AssignedGroup { get; set; }
    public string? AssignedTo { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Comments { get; set; }
    }
}
