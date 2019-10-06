using System;
using System.Collections.Generic;
using System.Linq;

namespace DbFirst.Template
{
    /// <summary>
    /// Table information
    /// </summary>
    public class Table
    {
        public string NameSpaces { get; set; }
        public string TableName { get; set; }
        public string ClassName { get; set; }

        public string CreateTable { get; set; }

        public List<Column> Columns { get; set; } = new List<Column>();
        public List<Index> Indexes { get; set; } = new List<Index>();
    }
}
