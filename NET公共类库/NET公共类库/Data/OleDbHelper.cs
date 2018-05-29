/**********************************************
 * 类作用：   Oledb操作辅助类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Web;

namespace Svnhost.Data
{
    /// <summary>
    /// 作 者: abaal
    /// 日 期: 2008-09-03
    /// </summary>
    public class OledbHelper
    {
        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        public static OleDbConnection GetOleDbConnection()
        {
            return new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["db"].ToString()));
        }

        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, string cmdText, params object[] p)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;

            if (p != null)
            {
                foreach (object parm in p)
                    cmd.Parameters.AddWithValue(string.Empty, parm);
            }
        }

        public static DataSet ExecuteDataset(string cmdText, params object[] p)
        {
            DataSet ds = new DataSet();
            OleDbCommand command = new OleDbCommand();
            using (OleDbConnection connection = GetOleDbConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                OleDbDataAdapter da = new OleDbDataAdapter(command);
                da.Fill(ds);
            }

            return ds;
        }

        public static DataRow ExecuteDataRow(string cmdText, params object[] p)
        {
            DataSet ds = ExecuteDataset(cmdText, p);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            return null;
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="cmdText">a</param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params object[] p)
        {
            OleDbCommand command = new OleDbCommand();

            using (OleDbConnection connection = GetOleDbConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 返回SqlDataReader对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static OleDbDataReader ExecuteReader(string cmdText, params object[] p)
        {
            OleDbCommand command = new OleDbCommand();
            OleDbConnection connection = GetOleDbConnection();
            try
            {
                PrepareCommand(command, connection, cmdText, p);
                OleDbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params object[] p)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection connection = GetOleDbConnection())
            {
                PrepareCommand(cmd, connection, cmdText, p);
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cmdText"></param>
        /// <param name="countText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params object[] p)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, p).ToString());

            DataSet ds = new DataSet();

            OleDbCommand command = new OleDbCommand();
            using (OleDbConnection connection = GetOleDbConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                OleDbDataAdapter da = new OleDbDataAdapter(command);
                da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            }
            return ds;
        }
    }
}
