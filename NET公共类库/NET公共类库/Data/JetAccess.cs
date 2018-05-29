/**********************************************
 * 类作用：   Access数据库操作辅助类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;
using System.Data.OleDb;
using System.Collections;

namespace Svnhost.Data
{
     /// <summary>
    /// Access数据库操作辅助
    /// </summary>
    /// <remarks>
    /// 作 者: abaal
    /// 日 期: 2008-09-03
    /// </remarks>
    public static class JetAccess
    {
        //Jet OLEDB:Engine Type Jet x.x Format MDB Files 
        //1 JET10 
        //2 JET11 
        //3 JET2X 
        //4 JET3X 
        //5 JET4X 

        /// <summary>
        /// 新建带密码的空Access 2000 数据库
        /// </summary>
        /// <param name="mdbFilePath">数据库文件路径</param>
        /// <param name="password">数据库密码</param>
        /// <returns>字符0为操作成功，否则为失败异常消息。</returns>
        public static string CreateMDB(string mdbFilePath, string password)
        {
            try
            {
                string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;";
                if (password == null || password.Trim() == "")
                {
                    connStr += "Data Source=" + mdbFilePath;
                }
                else
                {
                    connStr += "Jet OLEDB:Database Password=" + password + ";Data Source=" + mdbFilePath;
                }
                object objCatalog = Activator.CreateInstance(Type.GetTypeFromProgID("ADOX.Catalog"));
                object[] oParams = new object[] { connStr };
                objCatalog.GetType().InvokeMember("Create", BindingFlags.InvokeMethod, null, objCatalog, oParams);
                Marshal.ReleaseComObject(objCatalog);
                objCatalog = null;
                return "0";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        /// <summary>
        /// 新建空的Access数据库
        /// </summary>
        /// <param name="mdbFilePath">数据库文件路径</param>
        /// <returns>字符0为操作成功，否则为失败异常消息。</returns>
        public static string CreateMDB(string mdbFilePath)
        {
            return CreateMDB(mdbFilePath, null);
        }

        /// <summary>
        /// 压缩带密码Access数据库
        /// </summary>
        /// <param name="mdbFilePath">数据库文件路径</param>
        /// <param name="password">数据库密码</param>
        /// <returns>字符0为操作成功，否则为失败异常消息。</returns>
        public static string CompactMDB(string mdbFilePath, string password)
        {
            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:Engine Type=5;";
            string connStrTemp = connStr;
            string tmpPath = mdbFilePath + ".tmp";
            if (password == null || password.Trim() == "")
            {
                connStr += "Data Source=" + mdbFilePath;
                connStrTemp += "Data Source=" + tmpPath;
            }
            else
            {
                connStr += "Jet OLEDB:Database Password=" + password + ";Data Source=" + mdbFilePath;
                connStrTemp += "Jet OLEDB:Database Password=" + password + ";Data Source=" + mdbFilePath + ".tmp";
            }

            string strRet = "";
            try
            {
                object objJRO = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
                object[] oParams = new object[] { connStr, connStrTemp };
                objJRO.GetType().InvokeMember("CompactDatabase", BindingFlags.InvokeMethod, null, objJRO, oParams);
                Marshal.ReleaseComObject(objJRO);
                objJRO = null;
            }
            catch (Exception exp)
            {
                strRet = exp.Message;
            }

            try
            {
                System.IO.File.Delete(mdbFilePath);
                System.IO.File.Move(tmpPath, mdbFilePath);
            }
            catch (Exception expio)
            {
                strRet += expio.Message;
            }

            return (strRet == "") ? "0" : strRet;

        }

        /// <summary>
        /// 压缩带密码Access数据库
        /// </summary>
        /// <param name="mdbFilePath">数据库文件路径</param>
        /// <returns>字符0为操作成功，否则为失败异常消息。</returns>
        public static string CompactMDB(string mdbFilePath)
        {
            return CompactMDB(mdbFilePath, null);
        }

        /// <summary>
        /// 设置Access数据库的访问密码
        /// </summary>
        /// <param name="mdbFilePath">数据库文件路径</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns>字符0为操作成功，否则为失败异常消息。</returns>
        public static string SetMDBPassword(string mdbFilePath, string oldPwd, string newPwd)
        {
            string connStr = string.Concat("Provider=Microsoft.Jet.OLEDB.4.0;",
                "Mode=Share Deny Read|Share Deny Write;", //独占模式
                "Jet OLEDB:Database Password=" + oldPwd + ";Data Source=" + mdbFilePath);

            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                try
                {
                    conn.Open();
                    //如果密码为空时，请不要写方括号，只写一个null即可
                    string sqlOldPwd = (oldPwd == null || oldPwd.Trim() == "") ? "null" : "[" + oldPwd + "]";
                    string sqlNewPwd = (newPwd == null || newPwd.Trim() == "") ? "null" : "[" + newPwd + "]";
                    OleDbCommand cmd = new OleDbCommand(string.Concat("ALTER DATABASE PASSWORD ", sqlNewPwd, " ", sqlOldPwd),
                           conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    return "0";
                }
                catch (Exception exp)
                {
                    return exp.Message;
                }
            }
        }
    }

}
