//using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper.Tool {
    public class DefaultValueHelper {
        /// <summary>
        /// 设置OLEDB类型的默认值
        /// </summary>
        /// <param name="typeInt"></param>
        /// <returns></returns>
        public static object GetDefaultValue(int typeInt) {
            object t;
            switch (typeInt) {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 131: t = 0; break;
                case 7: t = DateTime.MinValue; break;
                case 11: t = false; break;
                case 128:
                case 17: t = new byte[] { }; break;
                case 72:
                case 130:
                default: t = ""; break;
            }
            return t;
        }
        public static object GetDefaultValue(Type targetType) {
            if (targetType == typeof(string)) return string.Empty;
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
        /*
        public static object GetDefaultValue(esriFieldType eft) {
            object obj = null; 
            switch (eft) {
                case esriFieldType.esriFieldTypeString:
                    obj = "";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                case esriFieldType.esriFieldTypeInteger:
                    obj = 0;
                    break;
                case esriFieldType.esriFieldTypeBlob:
                    obj = new byte[] { };
                    break;
                case esriFieldType.esriFieldTypeDate:
                    obj = DateTime.MinValue;
                    break;
            }
            return obj;
        }
        */
    }
}
