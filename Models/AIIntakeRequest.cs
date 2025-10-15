namespace ServiceCatalogAPI.Models
{
    public class AIIntakeRequest
    {
        public string? Id { get; set; }
        public string AIProductNames { get; set; }
        public string UseCaseDescription { get; set; }
        public string VendorName { get; set; }
    }
}
