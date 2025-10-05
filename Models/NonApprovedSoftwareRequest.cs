namespace ServiceCatalogAPI.Models
{
    public class NonApprovedSoftwareRequest
    {
    public string? Id { get; set; }
        public string RequestedFor { get; set; }
        public string SoftwareName { get; set; }
        public string SoftwareUrl { get; set; }
        public string BusinessJustification { get; set; }
        public int NumberOfUsers { get; set; }
        public bool IsLicenseNeeded { get; set; }
        public string Owner { get; set; }
    }
}
