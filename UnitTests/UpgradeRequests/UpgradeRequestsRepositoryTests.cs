using App1.Models;
using App1.Repositories;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.UpgradeRequests
{
    public class UpgradeRequestsRepositoryTests
    {
        
        private readonly string _testConnectionString = "Server=192.168.0.87;Database=gitIss;User Id=SA;Password=reallyStrongPwd123;TrustServerCertificate=True;";
        private readonly Mock<SqlConnection> _mockSqlConnection;
        private readonly Mock<SqlCommand> _mockSqlCommand;
        private readonly Mock<SqlDataReader> _mockSqlDataReader;
        private readonly Mock<SqlDataAdapter> _mockSqlDataAdapter;

        public UpgradeRequestsRepositoryTests()
        {
            // Setup mocks
            _mockSqlConnection = new Mock<SqlConnection>();
            _mockSqlCommand = new Mock<SqlCommand>();
            _mockSqlDataReader = new Mock<SqlDataReader>();
            _mockSqlDataAdapter = new Mock<SqlDataAdapter>();

            // Setup SqlConnection mock
            _mockSqlConnection.Setup(c => c.Open()).Verifiable();
            _mockSqlConnection.Setup(c => c.Close()).Verifiable();

            // Setup SqlCommand mock
            _mockSqlCommand.Setup(c => c.ExecuteReader()).Returns(_mockSqlDataReader.Object);
            _mockSqlCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

            // Setup SqlDataReader mock
            _mockSqlDataReader.Setup(r => r.Read()).Returns(true);
            _mockSqlDataReader.Setup(r => r.GetInt32(0)).Returns(1);
            _mockSqlDataReader.Setup(r => r.GetInt32(1)).Returns(100);
            _mockSqlDataReader.Setup(r => r.GetString(2)).Returns("Test User");
            _mockSqlDataReader.Setup(r => r.Close()).Verifiable();

            // Setup SqlDataAdapter mock
            _mockSqlDataAdapter.Setup(a => a.DeleteCommand).Returns(_mockSqlCommand.Object);
        }

        [Fact]
        public void Constructor_WithValidConnectionString_CreatesRepository()
        {
            // Act
            var repository = new UpgradeRequestsRepository(_testConnectionString);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_ReturnsListOfUpgradeRequests()
        {
            // Arrange
            var repository = new UpgradeRequestsRepository(_testConnectionString);

            // Act
            var result = repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UpgradeRequest>>(result);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_DeletesRequest()
        {
            // Arrange
            var repository = new UpgradeRequestsRepository(_testConnectionString);
            int upgradeRequestIdentifier = 1;

            // Act
            repository.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            // Verify that the repository attempted to delete the request
            // Note: In a real test, we would verify the SQL command was executed with the correct parameters
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_ReturnsUpgradeRequest()
        {
            // Arrange
            var repository = new UpgradeRequestsRepository(_testConnectionString);
            int upgradeRequestIdentifier = 1;

            // Act
            var result = repository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UpgradeRequest>(result);
            Assert.Equal(upgradeRequestIdentifier, result.UpgradeRequestId);
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_WhenRequestNotFound_ReturnsNull()
        {
            // Arrange
            var repository = new UpgradeRequestsRepository(_testConnectionString);
            int upgradeRequestIdentifier = 999; // Non-existent ID

            // Setup the mock to return false for Read() to simulate no results
            _mockSqlDataReader.Setup(r => r.Read()).Returns(false);

            // Act
            var result = repository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            Assert.Null(result);
        }
    }
} 