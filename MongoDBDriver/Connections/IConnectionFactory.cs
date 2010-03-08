using System;

namespace MongoDB.Driver.Connections
{
    public interface IConnectionFactory : IDisposable
    {
        Connection Open();

        void Close(Connection connection);

        string ConnectionString { get; }
    }
}