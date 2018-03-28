using DBHelper.Model;
//using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DBHelper.Tool {
    public class FieldConvertTool {
        /*
        /// <summary>
        /// 数据库标准中字段类型对应esriFieldType类型
        /// </summary>
        /// <param name="type">数据库标准中字段类型</param>
        /// <returns></returns>
        public static esriFieldType ConvertStd2Esri(stdFieldType type) {
            esriFieldType eft = 0;
            switch (type) {
                case stdFieldType.CHAR:
                case stdFieldType.VARCHAR:
                    eft = esriFieldType.esriFieldTypeString;
                    break;
                case stdFieldType.FLOAT:
                    eft = esriFieldType.esriFieldTypeDouble;
                    break;
                case stdFieldType.INT:
                    eft = esriFieldType.esriFieldTypeInteger;
                    break;
                case stdFieldType.VARBIN:
                    eft = esriFieldType.esriFieldTypeBlob;
                    break;
                case stdFieldType.DATE:
                    eft = esriFieldType.esriFieldTypeDate;
                    break;
            }
            return eft;
        }
        /// <summary>
        /// 数据库标准中字段类型对应esriFieldType类型
        /// </summary>
        /// <param name="type">数据库标准中字段类型</param>
        /// <returns></returns>
        public static esriFieldType ConvertOracle2Esri(OracleType type) {
            esriFieldType eft = 0;
            switch (type) {
                case OracleType.VARCHAR2:
                case OracleType.NVARCHAR2:
                    eft = esriFieldType.esriFieldTypeString;
                    break;
                case OracleType.NUMBER:
                    eft = esriFieldType.esriFieldTypeDouble;
                    break;
                case OracleType.SDO_GEOMETRY:
                    eft = esriFieldType.esriFieldTypeGeometry;
                    break;
                case OracleType.BLOB:
                case OracleType.CLOB:
                    eft = esriFieldType.esriFieldTypeBlob;
                    break;
                case OracleType.DATE:
                    eft = esriFieldType.esriFieldTypeDate;
                    break;
            }
            return eft;
        }*/
        /// <summary>
        /// 数据库标准中字段类型对应OleDbType类型
        /// </summary>
        /// <param name="type">数据库标准中字段类型</param>
        /// <returns></returns>
        public static OleDbType ConvertStd2OleDb(stdFieldType type) {
            OleDbType odt = 0;
            switch (type) {
                case stdFieldType.CHAR:
                    odt = OleDbType.WChar;
                    break;
                case stdFieldType.VARCHAR:
                    odt = OleDbType.VarWChar;
                    break;
                case stdFieldType.FLOAT:
                    odt = OleDbType.Double;
                    break;
                case stdFieldType.INT:
                    odt = OleDbType.BigInt;
                    break;
                case stdFieldType.VARBIN:
                    odt = OleDbType.Binary;
                    break;
                case stdFieldType.DATE:
                    odt = OleDbType.DBTimeStamp;
                    break;
            }
            return odt;
        }
        /// <summary>
        /// OleDbType类型对应的DotNet数据类型
        /// </summary>
        /// <param name="type">OleDbType字段类型</param>
        /// <returns></returns>
        public static Type ConvertOleDb2DotNET(OleDbType type) {
            Type dnt = null;
            switch (type) {
                case OleDbType.Integer:
                    dnt = typeof(int);
                    break;
                case OleDbType.WChar:
                case OleDbType.VarWChar:
                    dnt = typeof(string);
                    break;
                case OleDbType.Double:
                    dnt = typeof(double);
                    break;
                case OleDbType.BigInt:
                    dnt = typeof(Int64);
                    break;
                case OleDbType.Binary:
                    dnt = typeof(byte[]);
                    break;
                case OleDbType.DBTimeStamp:
                    dnt = typeof(DateTime);
                    break;
            }
            return dnt;
        }
        /// <summary>
        /// OleDb的数字转换为OleDbType
        /// </summary> 
        /// <param name="typeInt">mdb对应的类型数字</param>
        /// <returns></returns>
        public static OleDbType ConvertInt2OleDb(int typeInt) {
            OleDbType t;
            switch (typeInt) {
                case 2:
                case 3: t = OleDbType.Integer; break;
                case 4:
                case 5: t = OleDbType.Double; break;
                case 6:
                case 131: t = OleDbType.Decimal; break;
                case 7: t = OleDbType.DBTimeStamp; break;
                case 11: t = OleDbType.Boolean; break;
                case 128:
                case 17: t = OleDbType.Binary; break;
                case 72:
                case 130:
                default: t = OleDbType.VarWChar; break;
            }
            return t;
        }
        public static OleDbType Convert2OleDb(object obj) {
            if(obj.GetType()==typeof(int)) {
                return ConvertInt2OleDb((int)obj);
            } else {
                return ConvertStd2OleDb((stdFieldType)obj);
            }
        }
        public static Type ConvertOracle2DotNET(OracleType type) {
            Type dnt = null;
            switch (type) {
                case OracleType.NUMBER:
                    dnt = typeof(Decimal);
                    break;
                case OracleType.VARCHAR2:
                case OracleType.NVARCHAR2:
                    dnt = typeof(string);
                    break;
                case OracleType.BLOB:
                case OracleType.CLOB:
                case OracleType.SDO_GEOMETRY:
                    dnt = typeof(byte[]);
                    break;
                case OracleType.DATE:
                    dnt = typeof(DateTime);
                    break;
            }
            return dnt;
        }
    }
}
