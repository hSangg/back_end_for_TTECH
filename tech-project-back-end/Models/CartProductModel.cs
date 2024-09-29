namespace tech_project_back_end.Models;

    public class CartProductModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Category Category { get; set; }
        public SupplierModel Supplier { get; set; }
        public Image Image { get; set; }
    }

    public class SupplierModel
    {
        public string SupplierId { get; set; }

        public string SupplierName { get; set; }
    }
