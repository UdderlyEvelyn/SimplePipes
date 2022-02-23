using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public interface ICompoundResourceUser : ICompoundPipe
    {
        public float[] AmountPerTick { get; set; }

        public bool Enabled { get; set; }
    }
}