# DBHelper
>项目支持sqlite数据访问需要引用BIN/System.Data.SQLite.dll（sqlite提供，非MS提供），支持oracle数据访问引用BIN/Oracle.ManagedDataAccess.dll(Oracle公司提供的驱动，非MS提供的驱动)，支持oledb数据访问需要MS提供的System.Data.dll和System.Data.DataSetExtensions.dll

## 示例：
### 1.数据库连接：
***
`public DbFactory SQLiteDBHelper = new DbFactory(DatabseType.SQLITE,"DataSource=" + System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Config", "Config.hb"));

public DbFactory OracleDBHelper = new DbFactory(DatabseType.ORACLE,string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));Persist Security Info=True;User ID={2};Password={3};",server, serviceName, UserID, Password));

public DbFactory OleDbHelper = new DbFactory(DatabseType.OLEDB,string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False", sxbPath));`
### 2.增删改查：
***
增删改使用Excute
`string name="heyabo";
int id=1;
OleDbHelper.Excute("insert into user (name,id)values(@0,@1)",name,id);
`
查询使用ExcuteScalar<T>(返回单值T)、ExcuteDataTable（返回DataTable）、Query<T>(返回List<T>)、Pages（返回分页的DataTable）
`DataTable dt=OleDbHelper.ExcuteDataTable(select * from user);

int id=OleDbHelper.ExcuteScalar<int>("select id from user where name=@0",name);

List<int> idList=OleDbHelper.Query<int>(""select id from user");

int PAGESIZE=10,pageIndex=0;
DataTable page= OleDbHelper.Pages(PAGESIZE, pageIndex, "select * from user");`
所有函数支持参数占位符 **@0** 风格，后面跟动态参数
