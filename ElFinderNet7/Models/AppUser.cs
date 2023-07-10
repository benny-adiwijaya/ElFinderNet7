using System;
namespace ElFinderNet7.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string VolumePath { get; set; }
        public long QuotaInBytes { get; set; }
    }
}
