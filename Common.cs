using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper {
    public static class Common {
        public static object DBNull2Null(this object obj) {
            if (obj is DBNull) return null;
            return obj;
        }
        public static string ToStringIsNull(this object obj) {
            if (obj == null) return string.Empty;
            return obj.ToString();
        }
    }
}

