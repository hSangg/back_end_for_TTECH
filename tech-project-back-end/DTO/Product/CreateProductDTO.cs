namespace tech_project_back_end.DTO;

public class CreateProductDTO
{
    public string ProductId { get; set; }

    public string NamePr { get; set; }

    public string NameSerial { get; set; }

    public string Detail { get; set; }

    public ulong Price { get; set; }

    public int QuantityPr { get; set; }

    public int GuaranteePeriod { get; set; }

    public bool IsDeleted { get; set; }

    public string SupplierId { get; set;}

    public string CategoryId { get; set; }
}