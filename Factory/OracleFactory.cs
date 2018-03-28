using DBHelper.Model;
using DBHelper.Tool;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBHelper.Factory {
    public class OracleFactory:IDBFactory {
        /// <summary>
        /// string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));Persist Security Info=True;User ID={2};Password={3};",Host, ServiceName, UserID, Pwd);
        /// </summary>
        public string ConnStr { get; set; }

        public IDbConnection CreateConnection() {
            return new OracleConnection(ConnStr);
        }

        public IDbConnection CreateConnection(string strConn) {
            return new OracleConnection(strConn);
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand cmd) {
            return new OracleDataAdapter(cmd as OracleCommand);
        }

        public IDataReader CreateDataReader(IDbCommand myComm) {
            return myComm.ExecuteReader();
        }

        public IDbTransaction BeginTransaction(IDbConnection myConn) {
            return myConn.BeginTransaction();
        }
        public IDbCommand CreateCommand(string sql, params object[] args) {
            var opList = new List<OracleParameter>();
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
                string opName = ":p" + m.ToString();
                sql = sql.Replace(placeHolder[m], opName);
                OracleParameter op = new OracleParameter(opName, args[m]);
                opList.Add(op);
            }//替换
            OracleCommand command = new OracleCommand(sql);
            opList.ForEach(q => command.Parameters.Add(q));
            return command;
        }
        public List<Column_HB> GetColumns(string tableName) {
            List<Column_HB> colTypeDict = new List<Column_HB>();
            using (OracleConnection Conn = new OracleConnection(ConnStr)) {
                try {
                    Conn.Open();
                    DataTable columnTable = Conn.GetSchema("Columns", new string[] { ConnStr.Split(';')[2].Split('=')[1].ToUpper(), tableName });
                    foreach (DataRow dr in columnTable.Rows) {
                        string innerType=dr["DATA_TYPE"].ToStringIsNull();
                        OracleType type=(OracleType)Enum.Parse(typeof(OracleType),innerType);
                        int length=0;
                        if(dr["LENGTH"]!=DBNull.Value)length=Convert.ToInt16(dr["LENGTH"]);
                        if(dr["PRECISION"]!=DBNull.Value)length=Convert.ToInt16(dr["PRECISION"]);
                        colTypeDict.Add(new OracleColumn() {
                            Name = dr["COLUMN_NAME"].ToStringIsNull(),
                            IsNullable = dr["NULLABLE"].ToStringIsNull()=="Y",
                            InnerType=innerType,
                            Type=type,
                            DotNetType = FieldConvertTool.ConvertOracle2DotNET(type),
                            Length = length,
                            Precision = dr["SCALE"] == DBNull.Value ? 0 : Convert.ToInt16(dr["SCALE"])
                        });
                    }
                    return colTypeDict;
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
        public List<string> GetTables() {
            var nameList = new List<string>();
            using (OracleConnection Conn = new OracleConnection(ConnStr)) {
                try {
                    Conn.Open();
                    var dt = Conn.GetSchema("Tables",new string[] { ConnStr.Split(';')[2].Split('=')[1].ToUpper()} );
                    foreach (DataRow dr in dt.Rows) {
                        nameList.Add(dr["TABLE_NAME"].ToStringIsNull());
                    }
                    return nameList;
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
    }
}
