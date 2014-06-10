using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException(string message) : base(message)
        {
        }
    }
}
