using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public abstract class ValueBase
    {
        public virtual string ToString(string formatString = "")
        {
            return string.Empty;
        }
    }
}
