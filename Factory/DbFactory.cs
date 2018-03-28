using DBHelper.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBHelper.Factory {
    //数据库类型枚举  
    public enum DatabseType {
        MSSQLSERVER = 0,
        ODBC = 1,
        OLEDB = 2,
        ORACLE = 3,
        SQLITE = 4
    }
    public class DbFactory {
        //数据库工厂接口  
        IDBFactory dbF;

        IDbConnection DbConn;
        IDbTransaction Trans;

        /// <summary>  
        /// 数据库工厂构造函数  
        /// </summary>  
        /// <param name="dbtype">数据库枚举</param>
        /// <param name="connStr">连接字符串参考各对应工厂对象的连接字符串注释</param>
        public DbFactory(DatabseType dbtype, string connStr) {
            switch (dbtype) {
                case DatabseType.MSSQLSERVER:
                    //dbF = new MSSqlDbFactory();
                    break;
                case DatabseType.ODBC:
                    //dbF = new OdbcFactory();
                    break;
                case DatabseType.OLEDB:
                    dbF = new OleDbFactory();
                    break;
                case DatabseType.ORACLE:
                    dbF = new OracleFactory();
                    break;
                case DatabseType.SQLITE:
                    dbF = new SQLiteFactory();
                    break;
            }

            dbF.ConnStr = connStr;
        }
        public DbFactory(DatabseType dbtype, ConnectionString cs) {
            switch (dbtype) {
                case DatabseType.MSSQLSERVER:
                    //dbF = new MSSqlDbFactory();
                    break;
                case DatabseType.ODBC:
                    //dbF = new OdbcFactory();
                    break;
                case DatabseType.OLEDB:
                    dbF = new OleDbFactory();
                    break;
                case DatabseType.ORACLE:
                    dbF = new OracleFactory();
                    dbF.ConnStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));Persist Security Info=True;User ID={2};Password={3};"
                        ,cs.Server,cs.ServiceName,cs.UserID,cs.Password);
                    break;
                case DatabseType.SQLITE:
                    dbF = new SQLiteFactory();
                    break;
            }
        }
        public void BeginTransaction() {
            DbConn = dbF.CreateConnection();
            DbConn.Open();
            Trans=DbConn.BeginTransaction();
        }
        public void CommitTransaction() {
            Trans.Commit();
            DbConn.Close();

            Trans.Dispose();
            DbConn.Dispose();

            Trans = null;
            DbConn = null;
        }
        public void AbortTransaction() {
            Trans.Rollback();
            DbConn.Close();

            Trans.Dispose();
            DbConn.Dispose();

            Trans = null;
            DbConn = null;
        }
        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <returns></returns>
        public bool TestConnection() {
            using (IDbConnection Conn = dbF.CreateConnection()) {
                try {
                    if (Conn.State == ConnectionState.Closed) Conn.Open();
                    return true;
                } catch (Exception ex) {
                    return false;
                } finally {
                    if (Conn.State == ConnectionState.Open) Conn.Close();
                }
            }
        }
        /// <summary>
        /// 执行增删改的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void Excute(string sql, params object[] args) {
            //无事务
            if (DbConn == null) {
                using (IDbConnection Conn = dbF.CreateConnection()) {
                    try {
                        if (Conn.State == ConnectionState.Closed) Conn.Open();
                        var com = dbF.CreateCommand(sql, args);
                        com.Connection = Conn;
                        com.ExecuteNonQuery();
                    } catch (Exception ex) {
                        throw ex;
                    } finally {
                        Conn.Close();
                    }
                }
            } //有事务
            else {
                try {
                    if (DbConn.State == ConnectionState.Closed) DbConn.Open();
                    var com = dbF.CreateCommand(sql, args);
                    com.Connection = DbConn;
                    com.Transaction = Trans;
                    com.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 执行增删改的SQL
        /// </summary>
        /// <param name="cmd">自定义对应的Command</param>
        public void Excute(IDbCommand cmd) {
            //无事务
            if (DbConn == null) {
                using (IDbConnection Conn = dbF.CreateConnection()) {
                    try {
                        if (Conn.State == ConnectionState.Closed) Conn.Open();
                        cmd.Connection = Conn;
                        cmd.ExecuteNonQuery();
                    } catch (Exception ex) {
                        throw ex;
                    } finally {
                        Conn.Close();
                    }
                }
            } //有事务
            else {
                try {
                    if (DbConn.State == ConnectionState.Closed) DbConn.Open();
                    cmd.Connection = DbConn;
                    cmd.Transaction = Trans;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 执行SQL返回单个值的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T ExcuteScalar<T>(string sql, params object[] args) {
            using (IDbConnection Conn = dbF.CreateConnection()) {
                try {
                    Conn.Open();
                    var com = dbF.CreateCommand(sql, args);
                    com.Connection = Conn;
                    return (T)System.Convert.ChangeType(com.ExecuteScalar(), typeof(T));
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
        }
        /// <summary>
        /// 执行返回datatable的查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable ExcuteDataTable(string sql, params object[] args) {
            IDataAdapter da;
            DataSet ds = new DataSet();
            using (IDbConnection Conn = dbF.CreateConnection()) {
                try {
                    Conn.Open();
                    IDbCommand com = dbF.CreateCommand(sql, args);
                    com.Connection = Conn;
                    da = dbF.CreateDataAdapter(com);
                    da.Fill(ds);
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
            return ds.Tables[0];
        }
        public DataTable Pages(int pageSize, int index, string sql, params object[] args) {
            int count = ExcuteScalar<int>(string.Format("select count(*) from ({0}) ", sql));
            sql = "select rownum r_n_," + sql.ToUpper().Trim().Substring(6);
            int a = index * pageSize + 1;
            int b = a + pageSize-1;
            IDataAdapter da;
            DataSet ds = new DataSet();
            using (IDbConnection Conn = dbF.CreateConnection()) {
                try {
                    Conn.Open();
                    sql = string.Format("select * from ({0}) where (r_n_ between {1} and {2})", sql, a, b);
                    IDbCommand com = dbF.CreateCommand(sql, args);
                    com.Connection = Conn;
                    da = dbF.CreateDataAdapter(com);
                    da.Fill(ds);
                } catch (Exception ex) { throw ex; } finally { Conn.Close(); }
            }
            return ds.Tables[0];
        }
        public List<T> Query<T>(string sql, params object[] args) {
            List<T> List = new List<T>();
            var dt = ExcuteDataTable(sql, args);
            foreach (DataRow dr in dt.Rows) {
                List.Add(ConvertToEntity<T>(dr));
            }
            return List;
        }
        public T ConvertToEntity<T>(DataRow dr){
            
            Type t = typeof(T);
            object entity = null;
            if (t.GetConstructors().Count(q=>q.GetParameters().Count()==0) >0) {
                entity = Activator.CreateInstance(t);
                foreach (var info in t.GetProperties()) {
                    string colName = "";
                    if (Attribute.IsDefined(info, typeof(DBIgnoreAttribute))) continue;
                    if (Attribute.IsDefined(info, typeof(DBColumnNameAttribute))) {
                        colName = info.GetCustomAttributes(typeof(DBColumnNameAttribute), false)[0].ToStringIsNull();
                    } else { colName = info.Name; }
                    if (dr.Table.Columns.IndexOf(colName) < 0) continue;
                    if (dr[colName] == DBNull.Value) continue;
                    info.SetValue(entity, Convert.ChangeType(dr[colName], info.PropertyType), null);
                }
                return (T)entity;
            } else {
                return (T)dr[0];
            }
        }

        public List<Column_HB> GetColumns(string tableName) {
            return dbF.GetColumns(tableName);
        }
        public List<string> GetTables() {
            return dbF.GetTables();
        }


    }
}
