using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mifare.Exceptions
{
    public class CardIncValueException: Exception 
    {
        public CardIncValueException(String msg)
            : base(msg)
        {
        }
    }
}
