using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mifare.Exceptions
{
    public class CardTypeException : Exception
    {
        public CardTypeException(String msg)
            : base(msg)
        {
        }
    }
}
