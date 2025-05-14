using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Blazor.Shared
{
    public class ProjectRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdditionalText { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> Videos { get; set; }
    }
}
