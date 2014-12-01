using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardParser.Model
{
    public class Language
    {
        public string SpaceLabel { get; set; }

        public string ReturnLabel { get; set; }

        public List<Row>  Rows { get; set; }
    }
}
