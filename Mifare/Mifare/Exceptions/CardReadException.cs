using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mifare.Exceptions
{
    public class CardReadException: Exception 
    {
        public CardReadException(String msg)
            : base(msg)
        {
        }
    }
}
