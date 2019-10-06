
namespace DbFirst.Template
{
    public class Column
    {
        /// <summary>
        /// Corresponding the field name of table in  database
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Property names in classes
        /// </summary>
        public string PropertyName { get; set; }
        public string Type { get; set; }

        public string IsNull { get; set; }

        public string DefaultValue { get; set; }

        public string Comment { get; set; }

        public string Extra { get; set; }
    }
}
