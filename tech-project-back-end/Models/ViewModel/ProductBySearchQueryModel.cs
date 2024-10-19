namespace tech_project_back_end.Models.ViewModel
{
    public class ProductBySearchQueryModel
    {
        public Product Product { get; set; }
        public CategoryModel Category { get; set; }
        public  SupplierModel Supplier { get; set; }
        public Image Image { get; set; }
    }
}
