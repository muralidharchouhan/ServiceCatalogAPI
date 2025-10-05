namespace ServiceCatalogAPI.Models
{
    public class CatalogItem
    {
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string AssignedTo { get; set; }
    public string RequestedFor { get; set; }
    public string SupportGroup { get; set; }
    }
}
