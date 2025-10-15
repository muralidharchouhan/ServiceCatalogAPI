namespace ServiceCatalogAPI.Models
{
    public class Catalog
    {
        public string? CatalogId { get; set; }
        public string RequestType { get; set; }
        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
    }
}
