using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Exceptions
{
    public class UtilsException : Exception
    {
        public UtilsException()
        {
        }

        public UtilsException(string message)
            : base(message)
        {
        }

        public UtilsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
