using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespoBot.EventArgs
{
    internal class SubSessionIdentifierIndexedEventArgs
    {
        public Dictionary<int, int[]> SubSessionIdentifiers { get; set; }
        public bool AreSubSessionsHosted { get; set; }
    }
}
