using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper.Model {
    /// <summary>
    /// Oracle字段类型
    /// </summary>
    public enum OracleType {
        NUMBER,
        BLOB,
        CLOB,
        VARCHAR2,
        NVARCHAR2,
        SDO_GEOMETRY,
        DATE
    }
}
