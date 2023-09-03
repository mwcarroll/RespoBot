using System.Collections.Generic;

namespace RespoBot.EventArgs
{
    internal class MemberInfoUpdatedEventArgs : System.EventArgs
    {
        public List<DataContext.TrackedMember> Members { get; set; }
    }
}
