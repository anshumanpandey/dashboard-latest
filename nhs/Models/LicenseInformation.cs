using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHS.Models
{
    public class LicenseInformation
    {
        public string TotalNodLicenseAllocated { get; set; }
        public string NoOfUsedLicense { get; set; }
        public string NoOfUnusedLicense { get; set; }
    }
}