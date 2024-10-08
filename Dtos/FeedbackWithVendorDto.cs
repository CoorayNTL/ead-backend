/*
 * File: FeedbackWithVendorDto.cs
 * Project: TechFixBackend.Dtos
 * Description: Data Transfer Object (DTO) for feedback that includes vendor-related details. 
 *              This DTO contains properties such as Id, Vendor, Customer, Product, FeedbackMessage, 
 *              and Rating to provide comprehensive feedback information involving the vendor.
 */


using TechFixBackend._Models;

namespace TechFixBackend.Dtos
{
    public class FeedbackWithVendorDto
    {
        public string Id { get; set; }
        public User Vendor { get; set; }
        public User Customer { get; set; }
        public Product Product { get; set; }
        public string FeedbackMessage { get; set; }
        public int Rating { get; set; }
    }
}
