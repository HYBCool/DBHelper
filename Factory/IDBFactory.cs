using DBHelper.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBHelper.Factory {
    interface IDBFactory {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnStr { get; set; }
        

        /// <summary>
        /// 建立Connection对象
        /// </summary>
        /// <returns>Connection对象</returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// 根据连接字符串建立Connection对象
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <returns>Connection对象</returns>
        IDbConnection CreateConnection(string strConn);

        /// <summary>
        /// 建立Command对象
        /// </summary>
        /// <returns>Command对象</returns>
        IDbCommand CreateCommand(string sql, params object[] args);

        /// <summary>
        /// 建立DataAdapter对象
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>DataAdapter对象</returns>
        IDbDataAdapter CreateDataAdapter(IDbCommand cmd);

        /// <summary>
        /// 根据Connection建立Transaction
        /// </summary>
        /// <param name="myConn">Connection对象</param>
        /// <returns>Transaction对象</returns>
        IDbTransaction BeginTransaction(IDbConnection myConn);

        /// <summary>
        /// 根据Command对象建立DataReader
        /// </summary>
        /// <param name="myComm">Command对象</param>
        /// <returns>DataReader对象</returns>
        IDataReader CreateDataReader(IDbCommand myComm);
        /// <summary>
        /// 获取表的列信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<Column_HB> GetColumns(string tableName);
        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        List<string> GetTables();
    }
}
