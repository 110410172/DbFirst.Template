using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFirst.Template
{

    public class MySqlDataTypeTransformation 
    {
        static Dictionary<string, string> names = new Dictionary<string, string>();
        static MySqlDataTypeTransformation()
        {

            RegisterType("bit", "int");
            RegisterType("tinyint", "int");
            RegisterType("smallint", "int");
            RegisterType("mediumint", "int");
            RegisterType("int", "int");
            RegisterType("integer", "int");
            RegisterType("bigint", "long");
            RegisterType("decimal", "decimal");
            RegisterType("dec", "decimal");
            RegisterType("numeric", "decimal");
            RegisterType("fixed", "decimal");
            RegisterType("float", "float");
            RegisterType("double", "double");
            RegisterType("precision", "double");
            RegisterType("real", "double");
            RegisterType("bool", "int");
            RegisterType("boolean", "int");

            RegisterType("char", "string");
            RegisterType("varchar", "string");
            RegisterType("tinytext", "string");
            RegisterType("text", "string");
            RegisterType("mediumtext", "string");
            RegisterType("longtext", "string");
            RegisterType("binary", "string");

            RegisterType("date", "DateTime");
            RegisterType("datetime", "DateTime");
            RegisterType("timestamp", "uint");
            RegisterType("time", "DateTime");
            RegisterType("year", "int");

            RegisterType("tinyblob", "byte[]");
            RegisterType("blob", "byte[]");
            RegisterType("mediumblob", "byte[]");
            RegisterType("longtext", "byte[]");
        }
        static void RegisterType(string DataTypeName, string FieldTypeName)
        {
            names[DataTypeName] = FieldTypeName;
        }
        /// <summary>
        /// Gets the model field type name corresponding to the data column type name
        /// </summary>
        /// <param name="ColumnTypeName"></param>
        /// <returns></returns>
        public string Map(string ColumnTypeName)
        {
            ColumnTypeName = ColumnTypeName.Trim();
            ColumnTypeName = ColumnTypeName.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries)[0];
            var FieldTypeName = string.Empty;
            names.TryGetValue(ColumnTypeName, out FieldTypeName);
            return FieldTypeName;
        }
    }
}
