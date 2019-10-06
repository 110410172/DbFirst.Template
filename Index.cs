using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFirst.Template
{
    /// <summary>
    /// Index information
    /// </summary>
    public class Index
    {
        public List<string> Columns { get; set; } = new List<string>();

        public string IndexName { get; set; }

        /// <summary>
        /// 0 indicate unique，1 indicate general index
        /// </summary>
        public string NotUnique { get; set; }
    }
}
