using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public interface IResourceUser : IPipe
    {
        public bool Enabled { get; set; }
    }
}