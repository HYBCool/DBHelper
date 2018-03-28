using DBHelper.Model;
using DBHelper.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBHelper.Factory {
    public class OleDbFactory:IDBFactory {
        /// <summary>
        /// string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False", sxbPath)
        /// </summary>
        public string ConnStr { get; set; }

        public IDbConnection CreateConnection() {
            return new OleDbConnection(ConnStr);
        }

        public IDbConnection CreateConnection(string strConn) {
            return new OleDbConnection(strConn);
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand cmd) {
            return new OleDbDataAdapter(cmd as OleDbCommand);
        }

        public IDataReader CreateDataReader(IDbCommand myComm) {
            return myComm.ExecuteReader();
        }

        public IDbTransaction BeginTransaction(IDbConnection myConn) {
            return myConn.BeginTransaction();
        }
        public IDbCommand CreateCommand(string sql, params object[] args) {
            var opList = new List<OleDbParameter>();
            Regex reg = new Regex(@"@[\d]+");
            var mCollection = reg.Matches(sql);
            List<string> placeHolder = new List<string>();
            foreach (Match m in mCollection)//去重
            {
                if (!placeHolder.Contains(m.Value)) placeHolder.Add(m.Value);
            }
            if (placeHolder.Count != args.Count()) throw new Exception("参数个数不匹配");
            //placeHolder.Sort();//排序
            for (int i = -1, m = placeHolder.Count - 1; i < m; m--) {
                string opName = "@p" + m.ToString();
                sql = sql.Replace(placeHolder[m], opName);
                OleDbParameter op = new OleDbParameter(opName, args[m]);
                opList.Add(op);
            }//替换
            OleDbCommand command = new OleDbCommand(sql);
            opList.ForEach(q => command.Parameters.Add(q));
            return command;
        }

        public List<Column_HB> GetColumns(string tableName) {
            List<Column_HB> colTypeDict = new List<Column_HB>();
            using (OleDbConnection Conn = new OleDbConnection(ConnStr)) {
                try {
                    Conn.Open();
                    DataTable columnTable = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
                    foreach (DataRow dr in columnTable.Rows) {
                        OleDbType type=FieldConvertTool.Convert2OleDb(dr["DATA_TYPE"]);
                        colTypeDict.Add(new OleDbColumn() {
                            Name = dr["COLUMN_NAME"].ToStringIsNull(),
                            IsNullable = Convert.ToBoolean(dr["IS_NULLABLE"]),
                            Type = type,
                            DotNetType = FieldConvertTool.ConvertOleDb2DotNET(type),
                            Length = dr["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? 0 : System.Convert.ToInt16(dr["CHARACTER_MAXIMUM_LENGTH"]),
                            Precision = dr["numeric_precision"] == DBNull.Value ? 0 : System.Convert.ToInt16(dr["numeric_precision"])
                        });
                    }
                    return colTypeDict;
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
        public List<string> GetTables() {
            var nameList = new List<string>();
            using (OleDbConnection Conn = new OleDbConnection(ConnStr)) {
                try {
                    Conn.Open();
                    var dt = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                    foreach (DataRow dr in dt.Rows) {
                        nameList.Add(dr["TABLE_NAME"].ToStringIsNull());
                    }
                    return nameList;
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
    }
}
