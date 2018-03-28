using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper.Tool
{
    public static class WhereHelper
    {
        public static string GetWhere(string str1, string str2)
        {
            string res = "";
            if (string.IsNullOrEmpty(str1))
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    res = "where " + str2;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    res = str1 + " and " + str2;
                }
                else
                {
                    res = str1;
                }
            }
            return res;
        }
        /// <summary>
        /// 得到like语句 如: where filed like 'dwdmList[0]' or filed like 'dwdmList[1]' OR where filed in('0','1')
        /// </summary>
        /// <param name="filed">like 字段</param>
        /// <param name="dwdmList">要like 的值</param>
        /// <param name="isin">是否用in</param>
        /// <returns>string：where filed like 'dwdmList[0]' or filed like 'dwdmList[1]' or where filed in('0','1')</returns>
        public static string GetLikeORIn(string filed, List<string> dwdmList, bool isin = false)
        {
            string likes = "";
            if (!isin)
            {
                if (dwdmList.Any())
                {
                    dwdmList.ForEach(dm => { likes += filed + " like '" + dm + "%' or "; });
                }
                if (!string.IsNullOrEmpty(likes))
                {
                    likes = " where " + likes.Replace("or", "");
                }
            }
            else
            {
                if (dwdmList.Any())
                {
                    var instr = "";
                    dwdmList.ForEach(dm => { instr += "'" + dm + "',"; });
                    if (!string.IsNullOrEmpty(instr))
                    {
                        likes = " where " + filed + " in " + "(" + instr.TrimEnd(',') + ") ";
                    }
                }
            }
            return likes;
        }

        public static string GetExWhere(string wheres, string expression, List<string> dwdmList,
            Tuple<string, string> date)
        {
            string wheresql = "";
            if (!string.IsNullOrEmpty(wheres) && string.IsNullOrEmpty(expression))
            {
                if (dwdmList != null && dwdmList.Any())
                {
                    wheresql = GetLikeORIn(wheres, dwdmList);
                }
            }
            else if (!string.IsNullOrEmpty(wheres) && !string.IsNullOrEmpty(expression))
            {
                if (date != null)
                {
                    expression = expression.Trim().Replace(" ", "");
                    switch (expression)
                    {
                        case "<":
                        case "<=":
                            wheresql = string.Format(" where to_char({0},'yyyy-MM-dd')<='{1}' ", wheres, date.Item1);
                            break;
                        case ">":
                        case ">=":
                            wheresql = string.Format(" where to_char({0},'yyyy-MM-dd')>='{1}' ", wheres, date.Item1);
                            break;
                        case "<<":
                            wheresql = string.Format(" where to_char({0},'yyyy-MM-dd')>='{1}' and to_char({0},'yyyy-MM-dd')<='{2}'", wheres, date.Item1, date.Item2);
                            break;
                    }
                }
            }

            return wheresql;
        }
    }
}
