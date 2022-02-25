using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public interface ICompoundSource : ICompoundResourceUser
    {
        public float[] PushedPerTick { get; set; }

        public float[] OriginalResourceTotal { get; set; }

        public float[] Remaining { get; set; }

        public bool[] LimitedAmount { get; set; }

        public bool[] Empty { get; set; }
    }
}