

using Microsoft.VisualStudio.TextTemplating;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Dapper;
using System.Text;
using System.Text.RegularExpressions;

namespace DbFirst.Template
{
    /// <summary>
    /// Database Tool Program
    /// </summary>
    public static class TemplateUtilsExtensions
    {
        public static TemplateUtils TemplateUtils(this TextTransformation transformation, ITextTemplatingEngineHost host, string connectionStrings, StringBuilder template)
        {
            return new TemplateUtils(host, connectionStrings, template);
        }
    }

    /// <summary>
    /// Database Tool Program
    /// </summary>
    public class TemplateUtils
    {
        /// <summary>
        /// Database connection string
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Filter table names, optional here, tables that need to be updated
        /// </summary>
        private List<string> _filterTableNames;

        /// <summary>
        /// Connect
        /// </summary>
        private IDbConnection con;

        /// <summary>
        /// Database name
        /// </summary>
        public string Schema { get; private set; }

        /// <summary>
        /// Type conversion
        /// </summary>
        private MySqlDataTypeTransformation _mySqlDataTypeTransfer;

        /// <summary>
        /// Table
        /// </summary>
        public List<Table> Tables { get; private set; }

        public Manager TemplateManager { get; private set; }
        /// <summary>
        /// Version number
        /// </summary>
        public const string Version = "1.0.0";

        public TemplateUtils(ITextTemplatingEngineHost host, string connectionStrings, StringBuilder template, List<string> filterTableNames = null)
        {
            this._connectionString = connectionStrings;
            this._filterTableNames = filterTableNames;
            this._mySqlDataTypeTransfer = new MySqlDataTypeTransformation();
            this.TemplateManager = Manager.Create(host, template);
        }

        public void Init()
        {
            try
            {
                this.con = new MySqlConnection(this._connectionString);
                this.Schema = this.Schema ?? con.Database;
                this.Tables = new List<Table>();

                GetTablesFromDb();
                GetAllTableColumns();
                GetAllTableKeys();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.con.Dispose();
            }
        }

        private string ParseName(string name)
        {
             var a = name.Split('_').ToList().Select(x=> {
                 var res = x;
                 var regex = new Regex("^[a-z]");
                 if (regex.IsMatch(x)) res =  x.Substring(0, 1).ToUpper() + x.Substring(1);
                 return res;
             });
            return (a.Count() > 0) ? string.Join("", a) : name;
        }

        private void GetTablesFromDb()
        {
            string sql = string.Empty;

            if (_filterTableNames != null && _filterTableNames.Any())
            {
                sql = $"show full tables from `{Schema}` where Table_type = 'BASE TABLE' and Tables_in_{Schema} in ('{string.Join("','", _filterTableNames)}')";
            }
            else
            {
                sql = $"show full tables from `{Schema}` where Table_type = 'BASE TABLE'";
            }

            List<string> tables = con.Query<string>(sql).AsList();

            foreach (var t in tables)
            {
                Tables.Add(new Table
                {
                    TableName = t,
                    ClassName = this.ParseName(t)
                });
            }
            
        }

        /// <summary>
        /// Get field information for all tables
        /// </summary>
        private void GetAllTableColumns()
        {
            Tables.ForEach(t => GetTableColumns(t));
        }

        private void GetTableColumns(Table table)
        {
            var sql = @"select 
	                        COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,COLUMN_COMMENT,EXTRA 
                        from 
	                        information_schema.columns
                        where 
 	                        TABLE_SCHEMA=@tableSchema and TABLE_NAME = @tableName 
                        order by ORDINAL_POSITION asc;";

            var reader = con.ExecuteReader(sql, new { tableSchema = Schema, tableName = table.TableName });

            while (reader.Read())
            {
                Column column = new Column
                {
                    Name = reader["COLUMN_NAME"].ToString(),
                    PropertyName = this.ParseName(reader["COLUMN_NAME"].ToString()),
                    Type = this._mySqlDataTypeTransfer.Map(reader["COLUMN_TYPE"].ToString()),
                    IsNull = reader["IS_NULLABLE"].ToString(),
                    DefaultValue = reader["COLUMN_DEFAULT"].ToString(),
                    Comment = reader["COLUMN_COMMENT"].ToString(),
                    Extra = reader["EXTRA"].ToString()
                };
                table.Columns.Add(column);
            }

            reader.Close();
        }

        /// <summary>
        /// Get index information for all tables
        /// </summary>
        private void GetAllTableKeys()
        {
            Tables.ForEach(t=> GetTableKeys(t));
        }

        private void GetTableKeys(Table table)
        {
            // var sql = $"show keys from {Schema}.{table.TableName}";
            var sql = $"show keys from `{table.TableName}`";
            var reader = con.ExecuteReader(sql);
            
            Index last = null;
            while (reader.Read())
            {
                string keyName = reader["Key_name"].ToString();
                
                if (last == null || keyName != last.IndexName)
                {
                    last = new Index();
                    last.IndexName = keyName;
                    last.Columns.Add(reader["Column_name"].ToString());
                    last.NotUnique = reader["Non_unique"].ToString();

                    table.Indexes.Add(last);
                }
                else
                {
                    // Indicates that the two keys are in the same index
                    last.Columns.Add(reader["Column_name"].ToString());
                }
            }

            reader.Close();
        }

    }


}