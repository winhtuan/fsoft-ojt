using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plantpedia.ViewModel
{
    public class DropdownViewModel
    {
        public string Name { get; set; }
        public string Placeholder { get; set; }

        public List<SelectListItem> Options { get; set; }

        public DropdownViewModel()
        {
            Options = new List<SelectListItem>();
        }
    }
}
