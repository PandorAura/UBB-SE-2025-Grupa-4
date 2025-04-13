using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace App1.Infrastructure
{
    // Interfaces to abstract SQL dependencies
    public interface ISqlConnectionFactory
    {
        ISqlConnection CreateConnection();
    }

    public interface ISqlConnection : IDisposable
    {
        void Open();
        void Close();
        ISqlCommand CreateCommand();
        string ConnectionString { get; }
    }

    public interface ISqlCommand : IDisposable
    {
        ISqlDataReader ExecuteReader();
        int ExecuteNonQuery();
        ISqlParameterCollection Parameters { get; }
        string CommandText { get; set; }
        ISqlConnection Connection { get; set; }
    }

    public interface ISqlDataReader : IDisposable
    {
        bool Read();
        void Close();
        int GetInt32(int i);
        string GetString(int i);
    }

    public interface ISqlParameterCollection
    {
        ISqlParameter Add(string parameterName, SqlDbType dbType);
        ISqlParameter AddWithValue(string parameterName, object value);
    }

    public interface ISqlParameter
    {
        string ParameterName { get; set; }
        object Value { get; set; }
    }

    public interface ISqlDataAdapter
    {
        ISqlCommand DeleteCommand { get; set; }
    }

    // Implementations that wrap the actual SQL classes
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ISqlConnection CreateConnection()
        {
            return new SqlConnectionWrapper(new SqlConnection(_connectionString));
        }
    }

    public class SqlConnectionWrapper : ISqlConnection
    {
        public SqlConnection _connection;

        public SqlConnectionWrapper(SqlConnection connection)
        {
            _connection = connection;
        }

        public string ConnectionString => _connection.ConnectionString;

        public void Close()
        {
            _connection.Close();
        }

        public ISqlCommand CreateCommand()
        {
            return new SqlCommandWrapper(_connection.CreateCommand());
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public void Open()
        {
            _connection.Open();
        }
    }

    public class SqlCommandWrapper : ISqlCommand
    {
        public SqlCommand _command;

        public SqlCommandWrapper(SqlCommand command)
        {
            _command = command;
            Parameters = new SqlParameterCollectionWrapper(_command.Parameters);
        }

        public string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public ISqlConnection Connection
        {
            get => new SqlConnectionWrapper((SqlConnection)_command.Connection);
            set => _command.Connection = ((SqlConnectionWrapper)value)._connection;
        }

        public ISqlParameterCollection Parameters { get; }

        public void Dispose()
        {
            _command.Dispose();
        }

        public int ExecuteNonQuery()
        {
            return _command.ExecuteNonQuery();
        }

        public ISqlDataReader ExecuteReader()
        {
            return new SqlDataReaderWrapper(_command.ExecuteReader());
        }
    }

    public class SqlDataReaderWrapper : ISqlDataReader
    {
        private readonly SqlDataReader _reader;

        public SqlDataReaderWrapper(SqlDataReader reader)
        {
            _reader = reader;
        }

        public void Close()
        {
            _reader.Close();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public int GetInt32(int i)
        {
            return _reader.GetInt32(i);
        }

        public string GetString(int i)
        {
            return _reader.GetString(i);
        }

        public bool Read()
        {
            return _reader.Read();
        }
    }

    public class SqlParameterCollectionWrapper : ISqlParameterCollection
    {
        private readonly SqlParameterCollection _parameters;

        public SqlParameterCollectionWrapper(SqlParameterCollection parameters)
        {
            _parameters = parameters;
        }

        public ISqlParameter Add(string parameterName, SqlDbType dbType)
        {
            return new SqlParameterWrapper(_parameters.Add(parameterName, dbType));
        }

        public ISqlParameter AddWithValue(string parameterName, object value)
        {
            return new SqlParameterWrapper(_parameters.AddWithValue(parameterName, value));
        }
    }

    public class SqlParameterWrapper : ISqlParameter
    {
        private readonly SqlParameter _parameter;

        public SqlParameterWrapper(SqlParameter parameter)
        {
            _parameter = parameter;
        }

        public string ParameterName
        {
            get => _parameter.ParameterName;
            set => _parameter.ParameterName = value;
        }

        public object Value
        {
            get => _parameter.Value;
            set => _parameter.Value = value;
        }
    }

    public class SqlDataAdapterWrapper : ISqlDataAdapter
    {
        private readonly SqlDataAdapter _adapter;

        public SqlDataAdapterWrapper(SqlDataAdapter adapter)
        {
            _adapter = adapter;
        }

        public ISqlCommand DeleteCommand
        {
            get => new SqlCommandWrapper(_adapter.DeleteCommand);
            set => _adapter.DeleteCommand = ((SqlCommandWrapper)value)._command;
        }
    }
}