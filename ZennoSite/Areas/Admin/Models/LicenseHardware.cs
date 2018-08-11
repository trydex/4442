using System;
using System.Collections.Generic;

namespace ZennoSite.Areas.Admin.Models
{
    public class LicenseHardware
    {
        public int Id { get; set; }
        public int HardwareId { get; set; }
        public Hardware Hardware { get; set; }
        public int LicenseId { get; set; }
        public License License { get; set; }
        public DateTime? HardwareAttachingDate { get; set; }
        public DateTime? LastUse { get; set; }
    }
}