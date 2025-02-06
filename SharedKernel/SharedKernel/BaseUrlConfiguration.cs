using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class BaseUrlConfiguration
    {
        public const string CONFIG_NAME = "baseUrls";

        public string ApiBase { get; set; }
        public string WebBase { get; set; }
    }
}
