using System;
using System.Linq;

namespace RespoBot.Helpers;

internal class ComparableMemberChartByRating : iRApi.Member.MemberChart, IComparable<ComparableMemberChartByRating>, IComparable<iRApi.Member.MemberChart>
{
    public ComparableMemberChartByRating() : base()
    {
        
    }
    
    public int CompareTo(ComparableMemberChartByRating other)
    {
        if (Points.Max().Value > other.Points.Max().Value) return 1;
        else if (Points.Max().Value == other.Points.Max().Value) return 0;
        else return -1;
    }
    
    public int CompareTo(iRApi.Member.MemberChart other)
    {
        if (Points.Max().Value > other.Points.Max().Value) return 1;
        else if (Points.Max().Value == other.Points.Max().Value) return 0;
        else return -1;
    }
}