using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mifare.Exceptions
{
    public class CardWriteValueException: Exception
    {
        public CardWriteValueException(String msg)
            : base(msg)
        {
        }
    }
}
