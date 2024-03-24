using System.Collections.Generic;

namespace RespoBot.EventArgs
{
    internal class SubSessionIdentifierIndexedEventArgs
    {
        public Dictionary<int, int[]> SubSessionIdentifiers { get; set; }
        public bool AreSubSessionsHosted { get; set; }
    }
}
