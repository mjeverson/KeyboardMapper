using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardParser.Model
{
    public class Key
    {
        public string KeyCap { get; set; }

        public List<string> AltChars { get; set; }
    }
}
