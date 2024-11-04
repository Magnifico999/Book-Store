using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book.Models.ViewModels
{
    public class UpdateProductViewModel
    {
        public GetProduct Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
