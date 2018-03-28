using DBHelper.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBHelper.Factory {
    public class SQLiteFactory : IDBFactory {
        /// <summary>
        /// string.Format("DataSource={0}", fileName)
        /// </summary>
        public string ConnStr { get; set; }
        public IDbTransaction Trans { get;  set;  }

        public IDbConnection CreateConnection() {
            return new SQLiteConnection(ConnStr);
        }

        public IDbConnection CreateConnection(string strConn) {
            return new SQLiteConnection(strConn);
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand cmd) {
            return new SQLiteDataAdapter(cmd as SQLiteCommand);
        }

        public IDataReader CreateDataReader(IDbCommand myComm) {
            return myComm.ExecuteReader();
        }

        public IDbTransaction BeginTransaction(IDbConnection myConn) {
            return myConn.BeginTransaction();
        }
        public IDbCommand CreateCommand(string sql, params object[] args) {
            var opList = new List<SQLiteParameter>();
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
                SQLiteParameter op = new SQLiteParameter(opName, args[m]);
                opList.Add(op);
            }//替换
            SQLiteCommand command = new SQLiteCommand(sql);
            opList.ForEach(q => command.Parameters.Add(q));
            return command;
        }

        public List<Column_HB> GetColumns(string tableName) { throw new NotImplementedException(); }
        public List<string> GetTables() {
            var nameList = new List<string>();
            using (SQLiteConnection Conn = new SQLiteConnection(ConnStr)) {
                try {
                    Conn.Open();
                    var dt = Conn.GetSchema("Table" );
                    foreach (DataRow dr in dt.Rows) {
                        nameList.Add(dr["TABLE_NAME"].ToStringIsNull());
                    }
                    return nameList;
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
        
    }
}
