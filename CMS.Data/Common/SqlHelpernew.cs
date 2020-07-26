﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CMS.Data.Common
{
    internal class DBHelper
    {
        #region enums

        public enum DbProviders
        {
            SqlServer,
            OleDb,
            Oracle,
            ODBC,
            MySql,
            SQLite,
            NpgSql
        }

        #endregion

        #region private members

        private string _connectionstring = "";
        private DbProviderFactory _factory;
        private DbProviders _provider;

        #endregion

        #region properties

        public string connectionstring
        {
            get => _connectionstring;
            set
            {
                if (value != "") _connectionstring = value;
            }
        }

        public DbConnection connection { get; private set; }

        public DbCommand command { get; private set; }

        #endregion

        #region methods

        public void CreateDBObjects(string connectString, DbProviders providerList)
        {
            _provider = providerList;
            switch (providerList)
            {
                case DbProviders.SqlServer:
                    _factory = SqlClientFactory.Instance;
                    break;
            }

            connection = _factory.CreateConnection();
            command = _factory.CreateCommand();
            connection.ConnectionString = connectString;
            command.Connection = connection;
        }

        #region parameters

        public int AddParameter(string name, object value)
        {
            var parm = _factory.CreateParameter();
            parm.ParameterName = name;
            parm.Value = value;
            return command.Parameters.Add(parm);
        }

        public int AddParameter(DbParameter parameter)
        {
            return command.Parameters.Add(parameter);
        }

        public void ClearParameter()
        {
            if (command != null)
                if (command.Parameters.Count > 0)
                    command.Parameters.Clear();
        }

        #endregion

        #region transactions

        private void BeginTransaction()
        {
            if (connection.State == ConnectionState.Closed) connection.Open();

            command.Transaction = connection.BeginTransaction();
        }

        private void CommitTransaction()
        {
            command.Transaction.Commit();
            connection.Close();
        }

        private void RollbackTransaction()
        {
            command.Transaction.Rollback();
            connection.Close();
        }

        #endregion

        #region execute database functions

        public int ExecuteNonQuery(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            var i = -1;
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                BeginTransaction();
                i = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                CommitTransaction();
                command.Parameters.Clear();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    //command.Dispose();
                }
            }

            return i;
        }

        public object ExecuteScaler(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            object obj = null;
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                BeginTransaction();
                obj = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                CommitTransaction();
                command.Parameters.Clear();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    command.Dispose();
                }
            }

            return obj;
        }

        public DbDataReader ExecuteReader(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            DbDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                if (connectionstate == ConnectionState.Open)
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                else
                    reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Parameters.Clear();
            }

            return reader;
        }

        public DataSet GetDataSet(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            var adapter = _factory.CreateDataAdapter();
            command.CommandText = query;
            command.CommandType = commandtype;
            adapter.SelectCommand = command;
            var ds = new DataSet();
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Parameters.Clear();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    command.Dispose();
                }
            }

            return ds;
        }

        #endregion

        #endregion
    }
}