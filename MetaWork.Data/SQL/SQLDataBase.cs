using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace MetaWork.Data.SQL
{
    public class SQLDataBase : IDataBase
    {
        #region CheckDLL
        private static bool checkDLL = true;
        //public static string SetGUID
        //{
        //    set
        //    {
        //        if (value.Equals("{49D91D63-B25D-415D-8ACA-B595DB67F2CA}"))
        //            checkDLL = true;
        //        else
        //            checkDLL = false;
        //    }
        //}

        //public static string strLogFile = @"D:\Syn_log.log";
        #endregion

        #region Field-Member
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        #endregion

        #region Method
        public DataTable TableList()
        {
            return GetDataTable("SELECT TABLE_NAME as NAME, TABLE_CATALOG FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE like N'BASE TABLE' and TABLE_NAME <> 'sysdiagrams' ORDER BY TABLE_NAME");
        }

        public DataTable ColumnList(string TableName)
        {
            return GetDataTable(string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{0}'", TableName));
        }

        public DataTable ForeignKeyList(string TableName)
        {
            return GetDataTable(string.Format("SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name WHERE tc.TABLE_NAME = N'{0}'", TableName));
        }

        public DataTable ViewList()
        {
            return GetDataTable("SELECT TABLE_NAME as NAME FROM INFORMATION_SCHEMA.VIEWS");
        }

        public bool TableExists(string TableName)
        {
            if (!checkDLL) return false;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                string sql = String.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL SELECT 'true' ELSE SELECT 'false'", TableName);
                using (SqlCommand command = new SqlCommand(sql, connSQL))
                {
                    command.CommandType = CommandType.Text;
                    return (bool)command.ExecuteScalar();
                }
            }
        }

        public DateTime GetCurrentDate()
        {
            DateTime time;
            try
            {
                DataTable dt = GetDataTable("Select getdate()");
                string strDateTime = dt.Rows[0][0].ToString();
                time = DateTime.Parse(strDateTime);
            }
            catch (Exception)
            {
                time = DateTime.Now;
            }
            return time;
        }

        public object ExecuteInsert(string SQL)
        {
            object obj = null;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = SQL;
                        cmd.Connection = connSQL;
                        if (connSQL.State != ConnectionState.Open) connSQL.Open();
                        obj = cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    //Utils.Log.Save("Lỗi: " + ex.Message + Environment.NewLine + SQL, strLogFile);
                }
                finally
                {
                    connSQL.Close();
                }
            }
            if (obj != null)
                return obj;
            else
                return 0;
        }

        public int ExecuteNonQuery(string SQL)
        {
            if (!checkDLL) return -1;
            int iResult = 0;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SQL, connSQL))
                    {
                        cmd.Connection = connSQL;
                        if (connSQL.State != ConnectionState.Open)
                            connSQL.Open();
                        iResult = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    //Utils.Log.Save("Lỗi: " + ex.Message + Environment.NewLine + SQL, strLogFile);
                }
                finally
                {
                    connSQL.Close();
                }
                return iResult;
            }
        }

        //public int ExecuteNonQuery(string SQL, SQLFilter Filter)
        //{
        //    if (!checkDLL) return -1;
        //    int iResult = 0;
        //    using (var connSQL = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand(SQL, connSQL);
        //            Filter.GetSQLFilter();
        //            foreach (KeyValuePair<string, object> kvp in Filter.Parameters)
        //            {
        //                if (kvp.Value is Guid)
        //                {
        //                    SqlParameter para = new SqlParameter(kvp.Key, DbType.Binary);
        //                    para.Value = ((Guid)kvp.Value).ToByteArray();
        //                    cmd.Parameters.Add(para);
        //                }
        //                else
        //                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        //            }
        //            cmd.Connection = connSQL;
        //            if (connSQL.State != ConnectionState.Open)
        //                connSQL.Open();
        //            iResult = cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            //Utils.Log.Save("Lỗi: " + ex.Message + Environment.NewLine + SQL, strLogFile);
        //        }
        //        finally
        //        {
        //            connSQL.Close();
        //        }
        //        return iResult;
        //    }
        //}

        private object ExecuteInsert(SqlCommand cmd)
        {
            if (!checkDLL) return null;

            object obj = null;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    cmd.Connection = connSQL;
                    if (connSQL.State != ConnectionState.Open) connSQL.Open();
                    obj = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    //Utils.Log.Save("Lỗi: " + ex.Message + Environment.NewLine + cmd.CommandText, strLogFile);
                }
                finally
                {
                    connSQL.Close();
                }
            }

            if (obj != null)
                return obj;
            else
                return null;
        }

        private int ExecuteNonQuery(SqlCommand cmd)
        {
            if (!checkDLL) return 0;

            int RowUpdate = 0;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    cmd.Connection = connSQL;
                    if (connSQL.State != ConnectionState.Open) connSQL.Open();
                    RowUpdate = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Utils.Log.Save("Lỗi: " + ex.Message + Environment.NewLine + cmd.CommandText, strLogFile);
                }
                finally
                {
                    connSQL.Close();
                }
            }

            return RowUpdate;
        }
        #endregion

        #region CheckConnection
        public bool ConnectDatabase()
        {
            bool bResult = true;
            if (!checkDLL) return false;
            using (SqlConnection connSQL = new SqlConnection())
            {
                try
                {
                    connSQL.ConnectionString = ConnectionString;
                    connSQL.Open();
                }
                catch (Exception ex)
                {
                    bResult = false;
                    ////throw ex;
                }
                finally
                {
                    connSQL.Close();
                }
                return bResult;
            }
        }
        #endregion

        #region Select
        //public DataTable GetDataTable(SQLSelect SQL)
        //{
        //    if (!checkDLL) return null;
        //    DataTable dt = new DataTable();
        //    SqlDataReader reader = null;
        //    using (var connSQL = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand(SQL.GetSQLSelect(), connSQL);
        //            foreach (KeyValuePair<string, object> kvp in SQL.Parameters)
        //            {
        //                if (kvp.Value is Guid)
        //                {
        //                    SqlParameter para = new SqlParameter(kvp.Key, DbType.Binary);
        //                    para.Value = ((Guid)kvp.Value).ToByteArray();
        //                    cmd.Parameters.Add(para);
        //                }
        //                else
        //                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        //            }

        //            if (connSQL.State != ConnectionState.Open) connSQL.Open();
        //            reader = cmd.ExecuteReader();
        //            dt.Load(reader);
        //        }
        //        catch (Exception ex)
        //        {
        //            //throw ex;
        //        }
        //        finally
        //        {
        //            reader.Close();
        //            connSQL.Close();
        //        }
        //    }
        //    return dt;
        //}

        public DataTable GetDataTable(string SQL)
        {
            if (!checkDLL) return null;
            DataTable dt = new DataTable();

            string Temp = SQL.ToLower();
            if (Temp.Contains("update") || Temp.Contains("insert") ||
                Temp.Contains("delete") || Temp.Contains("call") || Temp.Contains("drop")) return dt;

            SqlDataReader reader = null;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SQL, connSQL))
                    {
                        if (connSQL.State != ConnectionState.Open) connSQL.Open();
                        reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connSQL.Close();
                }
            }
            return dt;
        }

        //public DataTable GetDataTable(string SQL, SQLFilter Filter)
        //{
        //    if (!checkDLL) return null;
        //    string Temp = SQL.ToLower();
        //    if (Temp.Contains("update") || Temp.Contains("insert") ||
        //        Temp.Contains("delete") || Temp.Contains("call") || Temp.Contains("drop")) return null;

        //    DataTable dt = new DataTable();
        //    SqlDataReader reader = null;
        //    using (var connSQL = new SqlConnection(_connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand(SQL, connSQL);
        //            Filter.GetSQLFilter();
        //            foreach (KeyValuePair<string, object> kvp in Filter.Parameters)
        //            {
        //                if (kvp.Value is Guid)
        //                {
        //                    SqlParameter para = new SqlParameter(kvp.Key, DbType.Binary);
        //                    para.Value = ((Guid)kvp.Value).ToByteArray();
        //                    cmd.Parameters.Add(para);
        //                }
        //                else
        //                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        //            }
        //            if (connSQL.State != ConnectionState.Open) connSQL.Open();
        //            reader = cmd.ExecuteReader();
        //            dt.Load(reader);
        //        }
        //        catch (Exception ex)
        //        {
        //            //throw ex;
        //        }
        //        finally
        //        {
        //            reader.Close();
        //            connSQL.Close();
        //        }
        //    }
        //    return dt;
        //}

        public DataSet GetDataSet(string SQL)
        {
            if (!checkDLL) return null;
            string Temp = SQL.ToLower();
            if (Temp.Contains("update") || Temp.Contains("insert") ||
                Temp.Contains("delete") || Temp.Contains("call") || Temp.Contains("drop")) return null;

            DataSet ds = new DataSet();
            //SqlDataAdapter adapter;
            using (var connSQL = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SQL, connSQL))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }


                }
                catch (Exception exSelect)
                {
                    //throw exSelect;
                }
                finally
                {
                    connSQL.Close();
                }
            }
            return ds;
        }

        //public object GetMaxInDataTable(string FieldName, string TableName, SQLFilter Filter)
        //{
        //    SQLSelect cmd = new SQLSelect(TableName);
        //    cmd.TopRecord = 0;
        //    if (Filter != null)
        //        cmd.Filter = Filter;
        //    else
        //        cmd.Filter.Clear();
        //    cmd.ColumnNames = string.Format("Max({0})", FieldName);
        //    cmd.GroupBy = string.Empty;
        //    cmd.OrderBy = string.Empty;
        //    cmd.PageNumber = 0;
        //    cmd.PageSize = 0;
        //    DataTable dt = GetDataTable(cmd.GetSQLSelect());
        //    if (dt.Rows.Count > 0)
        //    {
        //        return dt.Rows[0][0];
        //    }
        //    return null;
        //}

        //public object GetMinInDataTable(string FieldName, string TableName, SQLFilter Filter)
        //{
        //    SQLSelect cmd = new SQLSelect(TableName);
        //    cmd.TopRecord = 0;
        //    if (Filter != null)
        //        cmd.Filter = Filter;
        //    else
        //        cmd.Filter.Clear();
        //    cmd.ColumnNames = string.Format("Min({0})", FieldName);
        //    cmd.GroupBy = string.Empty;
        //    cmd.OrderBy = string.Empty;
        //    cmd.PageNumber = 0;
        //    cmd.PageSize = 0;
        //    DataTable dt = GetDataTable(cmd.GetSQLSelect());
        //    if (dt.Rows.Count > 0)
        //    {
        //        return dt.Rows[0][0];
        //    }
        //    return null;
        //}

        //public object GetCountInDataTable(string FieldName, string TableName, SQLFilter Filter)
        //{
        //    SQLSelect cmd = new SQLSelect(TableName);
        //    cmd.TopRecord = 0;
        //    if (Filter != null)
        //        cmd.Filter = Filter;
        //    else
        //        cmd.Filter.Clear();
        //    cmd.ColumnNames = string.Format("Count({0})", FieldName);
        //    cmd.GroupBy = string.Empty;
        //    cmd.OrderBy = string.Empty;
        //    cmd.PageNumber = 0;
        //    cmd.PageSize = 0;
        //    DataTable dt = GetDataTable(cmd.GetSQLSelect());
        //    if (dt.Rows.Count > 0)
        //    {
        //        return dt.Rows[0][0];
        //    }
        //    return null;
        //}
        #endregion

        #region Insert
        public object Insert(string tableName, Dictionary<string, object> data)
        {
            if (!checkDLL) return null;
            string columns = "";
            string values = "";
            using (SqlCommand cmd = new SqlCommand())
            {
                foreach (KeyValuePair<string, object> val in data)
                {
                    columns += string.Format(" {0},", val.Key);
                    if (val.Value != null && val.Value != DBNull.Value)
                    {
                        values += string.Format(" @{0},", val.Key);
                        if (val.Value is Guid)
                        {
                            SqlParameter para = new SqlParameter("@" + val.Key, DbType.Binary);
                            para.Value = ((Guid)val.Value).ToByteArray();
                            cmd.Parameters.Add(para);
                        }
                        else
                            cmd.Parameters.AddWithValue("@" + val.Key, val.Value);
                    }
                    else
                    {
                        values += " null,";
                    }
                }
                columns = columns.Substring(0, columns.Length - 1);
                values = values.Substring(0, values.Length - 1);
                cmd.CommandText = string.Format("insert into {0}({1}) values({2}); SELECT SCOPE_IDENTITY()", tableName, columns, values);
                return ExecuteInsert(cmd);

            }

        }
        #endregion

        #region Update
        //public int Update(string tableName, Dictionary<string, object> data, SQLFilter Fitler)
        //{
        //    if (!checkDLL) return -1;

        //    SqlCommand cmd = new SqlCommand();
        //    string keyWhere = Fitler.GetSQLFilter();
        //    foreach (KeyValuePair<string, object> kvp in Fitler.Parameters)
        //    {
        //        if (kvp.Value != null && kvp.Value != DBNull.Value)
        //        {
        //            if (kvp.Value is Guid)
        //            {
        //                SqlParameter para = new SqlParameter(kvp.Key, DbType.Binary);
        //                para.Value = ((Guid)kvp.Value).ToByteArray();
        //                cmd.Parameters.Add(para);
        //            }
        //            else
        //                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        //        }
        //        else
        //            keyWhere += string.Format(" {0} = null,", kvp.Key.Replace("@", ""));
        //    }

        //    string keyValues = "";
        //    foreach (KeyValuePair<string, object> kvp in data)
        //    {
        //        if (kvp.Value != null && kvp.Value != DBNull.Value)
        //        {
        //            if (!cmd.Parameters.Contains("@" + kvp.Key))
        //            {
        //                keyValues += string.Format(" {0} = @{0},", kvp.Key);
        //                if (kvp.Value is Guid)
        //                {
        //                    SqlParameter para = new SqlParameter("@" + kvp.Key, DbType.Binary);
        //                    para.Value = ((Guid)kvp.Value).ToByteArray();
        //                    cmd.Parameters.Add(para);
        //                }
        //                else
        //                    cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
        //            }
        //            else
        //            {
        //                keyValues += string.Format(" {0} = @{0}2,", kvp.Key);
        //                if (kvp.Value is Guid)
        //                {
        //                    SqlParameter para = new SqlParameter("@" + kvp.Key + "2", DbType.Binary);
        //                    para.Value = ((Guid)kvp.Value).ToByteArray();
        //                    cmd.Parameters.Add(para);
        //                }
        //                else
        //                    cmd.Parameters.AddWithValue("@" + kvp.Key + "2", kvp.Value);
        //            }
        //        }
        //        else
        //            keyValues += string.Format(" {0} = null,", kvp.Key);
        //    }

        //    if (keyValues.Length > 0)
        //        keyValues = keyValues.Substring(0, keyValues.Length - 1);

        //    cmd.CommandText = string.Format("update {0} set {1} where {2};", tableName, keyValues, keyWhere);
        //    return ExecuteNonQuery(cmd);
        //}
        #endregion

        #region Delete
        //public int Delete(string TableName, SQLFilter Fitler)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    string strFilter = Fitler.GetSQLFilter();
        //    foreach (KeyValuePair<string, object> kvp in Fitler.Parameters)
        //    {
        //        if (kvp.Value != null && kvp.Value != DBNull.Value)
        //        {
        //            if (kvp.Value is Guid)
        //            {
        //                SqlParameter para = new SqlParameter(kvp.Key, DbType.Binary);
        //                para.Value = ((Guid)kvp.Value).ToByteArray();
        //                cmd.Parameters.Add(para);
        //            }
        //            else
        //                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        //        }
        //        else
        //            strFilter += string.Format(" {0} = null,", kvp.Key.Replace("@", ""));
        //    }

        //    cmd.CommandText = string.Format("Delete from {0} where {1};", TableName, strFilter);
        //    return ExecuteNonQuery(cmd);
        //}
        #endregion
    }
}
