using System.Collections.Generic;

namespace RespoBot.Events.Args
{
    internal class MemberInfoUpdatedEventArgs : System.EventArgs
    {
        public List<DataContext.TrackedMember> Members { get; set; }
    }
}
