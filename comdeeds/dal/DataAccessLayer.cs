using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Assemblies;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace comdeeds.dal
{
    public class DataAccessLayer
    {
        #region Connection and Command Object
        public SqlConnection _con;
        public SqlCommand _com;
        #endregion

        #region properties
        public SqlConnection con
        {
            get
            {
                return _con;
            }
            set
            {
                _con = value;
            }
        }
        public SqlCommand com
        {
            get
            {
                return _com;
            }
            set
            {
                _com = value;
            }
        }

        #endregion

        #region PrivateMethods
        private void openconnection()
        {
            if (_con == null)
            {
                _con = new SqlConnection(ConfigurationManager.ConnectionStrings["ods"].ConnectionString);
                _com = new SqlCommand();
                _com.Connection = _con;
            }
            if (_con.State == ConnectionState.Closed)
            {
                _con.Open();

            }

        }
        private void closeconnection()
        {
            if (_con.State == ConnectionState.Open)
            {
                _com.Dispose();
                _con.Close();
            }
        }

        private void disposeconnection()
        {
            if (_con.State == ConnectionState.Closed)
            {
                _con.Dispose();
                _con = null;
            }
        }
        #endregion

        #region PublicMethods
        public int executesql(string strsql)
        {
            openconnection();
            _com.CommandType = CommandType.Text;
            _com.CommandTimeout = 30;
            _com.CommandText = strsql;
            int result = _com.ExecuteNonQuery();
            closeconnection();
            disposeconnection();
            return result;
        }

        public DataTable getdata(string strsql)
        {
            openconnection();
            _com.CommandType = CommandType.Text;
            _com.CommandTimeout = 30;
            _com.CommandText = strsql;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = _com;
            da.Fill(dt);
            closeconnection();
            disposeconnection();
            return dt;
        }

        public DataSet getdataset(string strsql)
        {
            openconnection();
            _com.CommandType = CommandType.Text;
            _com.CommandTimeout = 30;
            _com.CommandText = strsql;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = _com;
            da.Fill(ds);
            closeconnection();
            disposeconnection();
            return ds;
        }

        public int executeprocedure(string spname, SqlParameter[] arrparam)
        {
            openconnection();
            _com.CommandType = CommandType.StoredProcedure;
            _com.CommandTimeout = 30;
            _com.CommandText = spname;

            if (arrparam != null)
            {
                _com.Parameters.Clear();

                for (int i = 0; i < arrparam.Length; i++)
                {
                    _com.Parameters.Add(arrparam[i]);
                }
            }
            int result = _com.ExecuteNonQuery();
            closeconnection();
            disposeconnection();
            return result;
        }
        public string executeprocedure_returnid(string spname, SqlParameter[] arrparam)
        {
            openconnection();
            _com.CommandType = CommandType.StoredProcedure;
            _com.CommandTimeout = 30;
            _com.CommandText = spname;

            if (arrparam != null)
            {
                _com.Parameters.Clear();

                for (int i = 0; i < arrparam.Length; i++)
                {
                    _com.Parameters.Add(arrparam[i]);
                }
            }
            string modified = _com.ExecuteScalar().ToString();
            closeconnection();
            disposeconnection();
            return modified;
        }
        public DataTable executedtprocedure(string spname, SqlParameter[] arrparam, bool var)
        {
            openconnection();
            _com.CommandType = CommandType.StoredProcedure;
            _com.CommandTimeout = 30;
            _com.CommandText = spname;

            if (arrparam != null)
            {
                _com.Parameters.Clear();

                for (int i = 0; i < arrparam.Length; i++)
                {
                    _com.Parameters.Add(arrparam[i]);
                }
            }
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = _com;
            da.Fill(dt);
            closeconnection();
            disposeconnection();
            return dt;

        }
        public DataSet executedtprocedure_dataset(string spname, SqlParameter[] arrparam, bool var)
        {
            openconnection();
            _com.CommandType = CommandType.StoredProcedure;
            _com.CommandTimeout = 30;
            _com.CommandText = spname;

            if (arrparam != null)
            {
                _com.Parameters.Clear();

                for (int i = 0; i < arrparam.Length; i++)
                {
                    _com.Parameters.Add(arrparam[i]);
                }
            }
            DataSet dt = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = _com;
            da.Fill(dt);
            closeconnection();
            disposeconnection();
            return dt;

        }
        public SqlDataReader getDataReader(string strsql)
        {
            openconnection();
            _com.CommandType = CommandType.Text;
            _com.CommandTimeout = 30;
            _com.CommandText = strsql;
            // _com(strsql, _con);
            SqlDataReader rd = _com.ExecuteReader();
            // closeconnection();
            // disposeconnection();
            return rd;
        }


        #endregion
    }
}