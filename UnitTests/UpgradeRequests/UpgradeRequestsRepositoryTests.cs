using App1.Infrastructure;
using App1.Models;
using App1.Repositories;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace UnitTests.UpgradeRequests
{
    public class UpgradeRequestsRepositoryTests
    {
        private readonly Mock<ISqlConnectionFactory> _mockConnectionFactory;
        private readonly Mock<ISqlConnection> _mockConnection;
        private readonly Mock<ISqlCommand> _mockCommand;
        private readonly Mock<ISqlDataReader> _mockDataReader;
        private readonly Mock<ISqlParameterCollection> _mockParameters;
        private readonly Mock<ISqlDataAdapter> _mockDataAdapter;
        private readonly UpgradeRequestsRepository _repository;

        public UpgradeRequestsRepositoryTests()
        {
            // Setup mocks
            _mockConnectionFactory = new Mock<ISqlConnectionFactory>();
            _mockConnection = new Mock<ISqlConnection>();
            _mockCommand = new Mock<ISqlCommand>();
            _mockDataReader = new Mock<ISqlDataReader>();
            _mockParameters = new Mock<ISqlParameterCollection>();
            _mockDataAdapter = new Mock<ISqlDataAdapter>();

            // Setup connection factory to return mock connection
            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);

            // Setup connection mock behavior
            _mockConnection.Setup(c => c.Open()).Verifiable();
            _mockConnection.Setup(c => c.Close()).Verifiable();
            _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object);

            // Setup command mock
            _mockCommand.SetupGet(c => c.Parameters).Returns(_mockParameters.Object);
            _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockDataReader.Object);
            _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

            // Setup parameter collection mock
            _mockParameters.Setup(p => p.AddWithValue(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Mock.Of<ISqlParameter>());

            // Setup data reader mock for returning data
            _mockDataReader.Setup(r => r.Read()).Returns(true);
            _mockDataReader.Setup(r => r.GetInt32(0)).Returns(1);
            _mockDataReader.Setup(r => r.GetInt32(1)).Returns(100);
            _mockDataReader.Setup(r => r.GetString(2)).Returns("Test User");
            _mockDataReader.Setup(r => r.Close()).Verifiable();

            // Create repository with mocked dependencies
            _repository = new UpgradeRequestsRepository(_mockConnectionFactory.Object, _mockDataAdapter.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesRepository()
        {
            // Arrange & Act
            var repository = new UpgradeRequestsRepository(_mockConnectionFactory.Object, _mockDataAdapter.Object);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void Constructor_WithConnectionString_CreatesRepository()
        {
            // Arrange
            string connectionString = "TestConnectionString";

            // Act
            var repository = new UpgradeRequestsRepository(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_ReturnsUpgradeRequests()
        {
            // Arrange
            var upgradeRequestReader = new Mock<ISqlDataReader>();
            upgradeRequestReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            upgradeRequestReader.SetupSequence(r => r.GetInt32(0))
                .Returns(1)
                .Returns(2);

            upgradeRequestReader.SetupSequence(r => r.GetInt32(1))
                .Returns(100)
                .Returns(200);

            upgradeRequestReader.SetupSequence(r => r.GetString(2))
                .Returns("User 1")
                .Returns("User 2");

            _mockCommand.Setup(c => c.ExecuteReader()).Returns(upgradeRequestReader.Object);

            // Act
            var result = _repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].UpgradeRequestId);
            Assert.Equal(100, result[0].RequestingUserIdentifier);
            Assert.Equal("User 1", result[0].RequestingUserDisplayName);
            Assert.Equal(2, result[1].UpgradeRequestId);
            Assert.Equal(200, result[1].RequestingUserIdentifier);
            Assert.Equal("User 2", result[1].RequestingUserDisplayName);

            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_WhenExceptionOccurs_ReturnsEmptyList()
        {
            // Arrange
            _mockConnection.Setup(c => c.Open()).Throws(new Exception("Test exception"));

            // Act
            var result = _repository.RetrieveAllUpgradeRequests();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_DeletesRequest()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;

            // Act
            _repository.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
            _mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
            _mockParameters.Verify(p => p.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void RemoveUpgradeRequestByIdentifier_WhenExceptionOccurs_HandlesException()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;
            _mockConnection.Setup(c => c.Open()).Throws(new Exception("Test exception"));

            // Act & Assert (no exception should be thrown)
            _repository.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Verify connection handling
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_ReturnsUpgradeRequest()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;

            // Act
            var result = _repository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UpgradeRequest>(result);
            Assert.Equal(1, result.UpgradeRequestId);
            Assert.Equal(100, result.RequestingUserIdentifier);
            Assert.Equal("Test User", result.RequestingUserDisplayName);

            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
            _mockParameters.Verify(p => p.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_WhenRequestNotFound_ReturnsNull()
        {
            // Arrange
            int upgradeRequestIdentifier = 999; // Non-existent ID
            _mockDataReader.Setup(r => r.Read()).Returns(false);

            // Act
            var result = _repository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            Assert.Null(result);
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
            _mockParameters.Verify(p => p.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void RetrieveUpgradeRequestByIdentifier_WhenExceptionOccurs_ReturnsNull()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;
            _mockConnection.Setup(c => c.Open()).Throws(new Exception("Test exception"));

            // Act
            var result = _repository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);

            // Assert
            Assert.Null(result);
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockConnection.Verify(c => c.Close(), Times.Once);
        }
    }
}