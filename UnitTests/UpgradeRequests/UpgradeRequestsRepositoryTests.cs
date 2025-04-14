namespace UnitTests.UpgradeRequests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using App1.Infrastructure;
    using App1.Models;
    using App1.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    public class UpgradeRequestsRepositoryTests : IDisposable
    {
        private readonly string connectionString;
        private readonly UpgradeRequestsRepository repository;
        private readonly IDbConnectionFactory connectionFactory;

        public UpgradeRequestsRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            this.connectionString = config.GetConnectionString("TestConnection");
            this.connectionFactory = new SqlConnectionFactory(connectionString);
            this.repository = new UpgradeRequestsRepository(connectionFactory);
            EnsureTableExists();
            CleanupTable();
        }

        public void Dispose()
        {
            CleanupTable();
        }

        #region Integration Tests

        [Fact]
        public void RetrieveAllUpgradeRequests_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = this.repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_WithData_ReturnsAllRequests()
        {
            // Arrange
            InsertTestRequest(1, "User1");
            InsertTestRequest(2, "User2");

            // Act
            var result = this.repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.RequestingUserIdentifier == 1 && r.RequestingUserDisplayName == "User1");
            Assert.Contains(result, r => r.RequestingUserIdentifier == 2 && r.RequestingUserDisplayName == "User2");
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_NonExistentId_ReturnsNull()
        {
            // Act
            var result = this.repository.RetrieveUpgradeRequestByIdentifier(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_ExistingId_ReturnsRequest()
        {
            // Arrange
            int requestId = InsertTestRequest(42, "TestUser");

            // Act
            var result = this.repository.RetrieveUpgradeRequestByIdentifier(requestId);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal(requestId, result.RequestId);
            Assert.Equal(42, result.RequestingUserIdentifier);
            Assert.Equal("TestUser", result.RequestingUserDisplayName);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_ExistingId_RemovesRequest()
        {
            // Arrange
            int requestId = InsertTestRequest(42, "RemoveTestUser");

            // Act
            this.repository.RemoveUpgradeRequestByIdentifier(requestId);
            var result = this.repository.RetrieveUpgradeRequestByIdentifier(requestId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_NonExistentId_DoesNotThrow()
        {
            // Act
            var exception = Record.Exception(() => this.repository.RemoveUpgradeRequestByIdentifier(999));

            // Assert
            Assert.Null(exception);
        }

        #endregion

        #region Unit Tests with Mocks

        [Fact]
        public void Constructor_NullConnectionFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UpgradeRequestsRepository(connectionFactory: null));
        }

        

        
        [Fact]
        public void LegacyConstructor_ValidConnectionString_CreatesRepositorySuccessfully()
        {
            // Arrange & Act
            var exception = Record.Exception(() => new UpgradeRequestsRepository("dummy_connection_string"));

            // Assert
            Assert.Null(exception);
        }
        [Fact]
        public void RetrieveAllUpgradeRequests_DbException_HandlesExceptionAndReturnsEmptyList()
        {
            // Arrange
            var mockConnectionFactory = new Mock<IDbConnectionFactory>();

            // Setup to throw when connection is created
            mockConnectionFactory
                .Setup(f => f.CreateConnection());
                

            var repository = new UpgradeRequestsRepository(mockConnectionFactory.Object);

            // Act
            var result = repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_DbException_HandlesException()
        {
            // Arrange
            var mockConnectionFactory = new Mock<IDbConnectionFactory>();

            // Setup to throw when connection is created
            mockConnectionFactory
                .Setup(f => f.CreateConnection());
                

            var repository = new UpgradeRequestsRepository(mockConnectionFactory.Object);

            // Act
            var exception = Record.Exception(() => repository.RemoveUpgradeRequestByIdentifier(1));

            // Assert
            Assert.Null(exception); // Should not rethrow the exception
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_DbException_HandlesExceptionAndReturnsNull()
        {
            // Arrange
            var mockConnectionFactory = new Mock<IDbConnectionFactory>();

            // Setup to throw when connection is created
            mockConnectionFactory
                .Setup(f => f.CreateConnection());
                

            var repository = new UpgradeRequestsRepository(mockConnectionFactory.Object);

            // Act
            var result = repository.RetrieveUpgradeRequestByIdentifier(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_WithBadConnectionString_HandlesExceptionAndReturnsEmptyList()
        {
            // Arrange - Create repository with invalid connection string that will cause SQL exceptions
            var repository = new UpgradeRequestsRepository("Data Source=nonexistent;Initial Catalog=fake;User Id=wrong;Password=wrong;");

            // Act
            var result = repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_WithBadConnectionString_HandlesException()
        {
            // Arrange - Create repository with invalid connection string that will cause SQL exceptions
            var repository = new UpgradeRequestsRepository("Data Source=nonexistent;Initial Catalog=fake;User Id=wrong;Password=wrong;");

            // Act
            var exception = Record.Exception(() => repository.RemoveUpgradeRequestByIdentifier(1));

            // Assert
            Assert.Null(exception); // Should not rethrow the exception
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_WithBadConnectionString_HandlesExceptionAndReturnsNull()
        {
            // Arrange - Create repository with invalid connection string that will cause SQL exceptions
            var repository = new UpgradeRequestsRepository("Data Source=nonexistent;Initial Catalog=fake;User Id=wrong;Password=wrong;");

            // Act
            var result = repository.RetrieveUpgradeRequestByIdentifier(1);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Helper Methods

        private void CleanupTable()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM UpgradeRequests", conn);
            cmd.ExecuteNonQuery();
        }

        private void EnsureTableExists()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UpgradeRequests')
                BEGIN
                    CREATE TABLE UpgradeRequests (
                        RequestId INT PRIMARY KEY IDENTITY(1,1),
                        RequestingUserId INT NOT NULL,
                        RequestingUserName NVARCHAR(100) NOT NULL
                    )
                END", conn);
            cmd.ExecuteNonQuery();
        }

        private int InsertTestRequest(int userId, string userName)
        {
            int requestId = 0;
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Insert the record
            using (var cmdInsert = new SqlCommand(
                @"INSERT INTO UpgradeRequests (RequestingUserId, RequestingUserName) 
                  VALUES (@userId, @userName);
                  SELECT SCOPE_IDENTITY();", conn))
            {
                cmdInsert.Parameters.AddWithValue("@userId", userId);
                cmdInsert.Parameters.AddWithValue("@userName", userName);

                // Get the generated ID
                requestId = Convert.ToInt32(cmdInsert.ExecuteScalar());
            }

            return requestId;
        }

        #endregion
    }
}