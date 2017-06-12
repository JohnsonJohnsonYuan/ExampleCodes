using System;
using System.Collections.Generic;
using System.Collections;
using IpInfo.Core.Models;

namespace IpInfo.Core
{
    public sealed class IpRecordComparer : IComparer, IComparer<IpRecord>
    {
        public int Compare(IpRecord x, IpRecord y)
        {
            return x.StartIpNumber.CompareTo(y.StartIpNumber);
        }

        int IComparer.Compare(object x, object y)
        {
            return Compare((IpRecord)x, (IpRecord)y);
        }
    }
}
