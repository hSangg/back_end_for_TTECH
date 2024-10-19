namespace tech_project_back_end.Models.ViewModel
{
    public class CartProductModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public CategoryModel Category { get; set; }
        public SupplierModel Supplier { get; set; }
        public Image Image { get; set; }
    }
}


