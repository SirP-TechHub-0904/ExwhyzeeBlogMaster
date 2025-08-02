using ModelData.Models;

namespace AdminUI.Areas.Admin.ViewModels
{
   

    public class CategoryRowViewModel
    {
        public Category Cat { get; set; } = null!;
        public int Indent { get; set; }
        public string Prefix { get; set; } = "";
        public IList<Category> AllCategories { get; set; } = new List<Category>();
        public int SerialNumber { get; set; }
    }
}
