using System;
using System.Collections.Generic;
using System.Timers;

namespace ElFinderNet7.Models
{
    public class UploadPulseModel
    {
        public List<string> UploadedFiles { get; set; }
        public DateTimeOffset LastPulse { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
