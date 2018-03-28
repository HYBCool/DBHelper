using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper.Model {
    /// <summary>
    /// 连接字符串对象
    /// </summary>
    public class ConnectionString {
        /// <summary>
        /// 服务器名称或IP
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
