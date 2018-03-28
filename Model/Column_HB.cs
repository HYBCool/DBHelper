using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DBHelper.Model {
    public class Column_HB {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string InnerType { get; set; }
        public Type DotNetType { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public bool IsNullable { get; set; }
        public string Comment { get; set; }
        
    }
    public class OleDbColumn:Column_HB {
        public OleDbType Type { get; set; }
    }
    public class OracleColumn : Column_HB {
        public OracleType Type { get; set; }
    }
}
