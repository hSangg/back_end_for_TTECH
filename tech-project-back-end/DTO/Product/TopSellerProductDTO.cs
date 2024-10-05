using DocumentFormat.OpenXml.Vml;

namespace tech_project_back_end.DTO
{
    public class TopSellerProductDTO
    {
        public string ProductId { get; set; }
        public int TotalQuantitySold { get; set; }
        public string ProductName { get; set; }
        public ICollection<ImageDTO> Image { get; set; }
    }
}
