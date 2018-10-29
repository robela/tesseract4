using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocr.Base.Models
{
    public class Row
    {
        public int Line { get; set; }
        public string Word { get; set; }
        public int Colomun { get; set; }
        public int Type { get; set; }
        public string StringType { get; set; }
        public float XCordinateStart { get; set; }
        public float XCordinateEnd { get; set; }
        public float YCordinateStart { get; set; }
        public float YCordinateEnd { get; set; }
    }

    public class IndexedRow
    {
        public Row Row { get; set; }
        public int Index { get; set; }

    }
}
