using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MetaWork.Data.SQL
{
    public interface IDataBase
    {
        #region Method
        DataTable TableList();

        DataTable ColumnList(string TableName);

        DataTable ForeignKeyList(string TableName);

        DataTable ViewList();

        bool TableExists(string TableName);

        DateTime GetCurrentDate();

        object ExecuteInsert(string SQL);

        int ExecuteNonQuery(string SQL);

        //int ExecuteNonQuery(string SQL, SQLFilter Filter);
        #endregion

        #region Connection
        string ConnectionString
        {
            get;
            set;
        }

        bool ConnectDatabase();
        #endregion

        #region Select
        DataTable GetDataTable(string SQL);

        //DataTable GetDataTable(SQLSelect SQL);

        //DataTable GetDataTable(string SQL, SQLFilter Filter);

        DataSet GetDataSet(string SQL);

        //object GetMaxInDataTable(string FieldName, string TableName, SQLFilter Filter);

        //object GetMinInDataTable(string FieldName, string TableName, SQLFilter Filter);

       
        #endregion

        #region Insert
        object Insert(string TableName, Dictionary<string, object> data);
        #endregion

        #region Update
        //int Update(string TableName, Dictionary<string, object> data, SQLFilter Filter);
        #endregion

        #region Delete
        //int Delete(string tableName, SQLFilter Filter);
        #endregion
    }
}
