using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mifare.Exceptions
{
    public class CardLoginException: Exception 
    {
        public CardLoginException(String msg)
            : base(msg)
        {
        }
    }
}
