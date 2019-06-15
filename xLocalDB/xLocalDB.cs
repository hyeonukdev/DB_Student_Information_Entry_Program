using System;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace 의료IT공학과.데이터베이스
{
    //----------------------------------
    public interface iLocalDB
    {
        void Open();
        void Close();
        string Query(string sql);
        void ExecuteNonQuery(string sql);
        void ExecuteReader(string sql);
        bool Read();
        bool HasRows { get; }
        int FieldCount { get; }
        string GetName(int index);
        object GetData(string dataName);
        object GetData(int index);
    }

    //---------------------------------
    public class xLocalDB : iLocalDB
    {
        xSqlDB sqlDB = null;
        xOleDB oleDB = null;

        public xLocalDB(string connStr)
        {
            
            string str = connStr.ToLower();

            if(str.Contains(".accdb"))
            {
                oleDB = new xOleDB(connStr);
            }
            else if(str.Contains(".mdf"))
            {
                sqlDB = new xSqlDB(connStr);
            }
            else
            {
                Console.WriteLine("xLocalDB: 알 수 없는 DB파일입니다.\n" + connStr);
                Environment.Exit(0);
            }
        }
        
        public void Open()
        {
            if      (sqlDB != null) sqlDB.Open();
            else if (oleDB != null) oleDB.Open();
        }

        public void Close()
        {
            if      (sqlDB != null) sqlDB.Close();
            else if (oleDB != null) oleDB.Close();
        }

        public string Query(string sql)
        {
            if      (sqlDB != null) return sqlDB.Query(sql);
            else if (oleDB != null) return oleDB.Query(sql);
            else return "Error: DB가 열려있지 않습니다.";

        }

        public void ExecuteNonQuery(string sql)
        {
            if      (sqlDB != null) sqlDB.ExecuteNonQuery(sql);
            else if (oleDB != null) oleDB.ExecuteNonQuery(sql);
        }

        public void ExecuteReader(string sql)
        {
            if      (sqlDB != null) sqlDB.ExecuteReader(sql);
            else if (oleDB != null) oleDB.ExecuteReader(sql);
        }

        public bool Read()
        {
            if      (sqlDB != null) return sqlDB.Read();
            else if (oleDB != null) return oleDB.Read();
            return false;
        }

        public bool HasRows
        {
            get
            {
                if      (sqlDB != null) return sqlDB.HasRows;
                else if (oleDB != null) return oleDB.HasRows;
                else return false;
            }

        }

        public int FieldCount
        {
            get
            {
                if      (sqlDB != null) return sqlDB.FieldCount;
                else if (oleDB != null) return oleDB.FieldCount;
                else return 0;
            }
        }

        public string GetName(int index)
        {
            if      (sqlDB != null) return sqlDB.GetName(index);
            else if (oleDB != null) return oleDB.GetName(index);
            else return "null";
        }

        public object GetData(string dataName)
        {
            if      (sqlDB != null) return sqlDB.GetData(dataName);
            else if (oleDB != null) return oleDB.GetData(dataName);
            else return null;
        }

        public object GetData(int index)
        {
            if      (sqlDB != null) return sqlDB.GetData(index);
            else if (oleDB != null) return oleDB.GetData(index);
            else return null;
        }

    }//class xLocalDB

    //------------------------------------------------
    public class xSqlDB : iLocalDB
    {
        SqlCommand comm = null;
        SqlConnection conn = null;
        SqlDataReader reader = null;

        string connectionStr = null;

        public xSqlDB(string connStr)
        {
            connectionStr = connStr;
        }

        public void Open()
        {
            if (conn != null) Close();

            comm = new SqlCommand();
            conn = new SqlConnection();

            conn.ConnectionString = connectionStr;
            conn.Open();
            comm.Connection = conn;
        }

        public void Close()
        {
            if (conn != null) conn.Close();
            if (reader != null) reader.Close();
            conn = null;
            comm = null;
            reader = null;
        }

        public string Query(string sql)
        {
            string trimmedSQL = sql.Trim();
            string[] words = trimmedSQL.Split(' ');

            try
            {
                if (words[0].ToUpper().Equals("SELECT")) ExecuteReader(trimmedSQL);
                else ExecuteNonQuery(trimmedSQL);
            }
            catch (InvalidOperationException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;

        }

        public void ExecuteNonQuery(string sql)
        {
            if (reader != null) reader.Close();
            comm.CommandText = sql;
            comm.ExecuteNonQuery();
        }

        public void ExecuteReader(string sql)
        {
            if (reader != null) reader.Close();
            comm.CommandText = sql;
            reader = comm.ExecuteReader();
        }

        public bool Read()
        {
            if (reader == null) return false;

            return reader.Read();
        }

        public bool HasRows
        {
            get
            {
                if (reader == null) return false;
                return reader.HasRows;
            }

        }

        public int FieldCount
        {
            get
            {
                if (reader == null) return 0;
                return reader.FieldCount;
            }
        }

        public string GetName(int index)
        {
            if (reader == null) return "";

            return reader.GetName(index);
        }

        public object GetData(string dataName)
        {
            return reader[dataName];
        }

        public object GetData(int index)
        {
            return reader[index];
        }

    }//xSQL


    //----------------------------------------------------
    public class xOleDB : iLocalDB
    {
        OleDbCommand comm = null;
        OleDbConnection conn = null;
        OleDbDataReader reader = null;

        string connectionStr = null;

        public xOleDB(string connStr)
        {
            connectionStr = connStr;
        }

        public void Open()
        {
            if (conn != null) Close();

            comm = new OleDbCommand();
            conn = new OleDbConnection();

            conn.ConnectionString = connectionStr;
            conn.Open();
            comm.Connection = conn;
        }

        public void Close()
        {
            if (conn != null) conn.Close();
            if (reader != null) reader.Close();
            conn = null;
            comm = null;
            reader = null;
        }

        public string Query(string sql)
        {
            string trimmedSQL = sql.Trim();
            string[] words = trimmedSQL.Split(' ');

            try
            {
                if (words[0].ToUpper().Equals("SELECT")) ExecuteReader(trimmedSQL);
                else ExecuteNonQuery(trimmedSQL);
            }
            catch (InvalidOperationException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;

        }

        public void ExecuteNonQuery(string sql)
        {
            if (reader != null) reader.Close();
            comm.CommandText = sql;
            comm.ExecuteNonQuery();
        }

        public void ExecuteReader(string sql)
        {
            if (reader != null) reader.Close();
            comm.CommandText = sql;
            reader = comm.ExecuteReader();
        }

        public bool Read()
        {
            if (reader == null) return false;

            return reader.Read();
        }

        public bool HasRows
        {
            get
            {
                if (reader == null) return false;
                return reader.HasRows;
            }

        }

        public int FieldCount
        {
            get
            {
                if (reader == null) return 0;
                return reader.FieldCount;
            }
        }

        public string GetName(int index)
        {
            if (reader == null) return "";

            return reader.GetName(index);
        }

        public object GetData(string dataName)
        {
            return reader[dataName];
        }

        public object GetData(int index)
        {
            return reader[index];
        }

    }//xOLE
}