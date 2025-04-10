using App1.Models;
using App1.Repositories;
using App1.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.UpgradeRequests
{
    public class UpgradeRequestsServiceTests
    {
        private readonly Mock<IUpgradeRequestsRepository> _mockUpgradeRequestsRepository;
        private readonly Mock<IRolesRepository> _mockRolesRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UpgradeRequestsService _upgradeRequestsService;

        public UpgradeRequestsServiceTests()
        {
            // Setup mocks
            _mockUpgradeRequestsRepository = new Mock<IUpgradeRequestsRepository>();
            _mockRolesRepository = new Mock<IRolesRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            // Create service with mocked dependencies
            _upgradeRequestsService = new UpgradeRequestsService(
                _mockUpgradeRequestsRepository.Object,
                _mockRolesRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesService()
        {
            // Assert
            Assert.NotNull(_upgradeRequestsService);
        }

        [Fact]
        public void RetrieveAllUpgradeRequests_ReturnsListFromRepository()
        {
            // Arrange
            var expectedRequests = new List<UpgradeRequest>
            {
                new UpgradeRequest(1, 100, "User 1"),
                new UpgradeRequest(2, 101, "User 2")
            };
            _mockUpgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(expectedRequests);

            // Act
            var result = _upgradeRequestsService.RetrieveAllUpgradeRequests();

            // Assert
            Assert.Equal(expectedRequests, result);
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.Once);
        }

        [Fact]
        public void GetRoleNameBasedOnIdentifier_ReturnsRoleName()
        {
            // Arrange
            int roleIdentifier = 1;
            string expectedRoleName = "User";
            var roles = new List<Role>
            {
                new Role(0, "Banned"),
                new Role(1, "User"),
                new Role(2, "Manager"),
                new Role(3, "Admin")
            };
            _mockRolesRepository.Setup(r => r.getRoles()).Returns(roles);

            // Act
            var result = _upgradeRequestsService.GetRoleNameBasedOnIdentifier(roleIdentifier);

            // Assert
            Assert.Equal(expectedRoleName, result);
            _mockRolesRepository.Verify(r => r.getRoles(), Times.Once);
        }

        [Fact]
        public void ProcessUpgradeRequest_WhenAccepted_UpgradesUserRole()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;
            int requestingUserIdentifier = 100;
            int currentHighestRoleIdentifier = 1;
            var upgradeRequest = new UpgradeRequest(upgradeRequestIdentifier, requestingUserIdentifier, "Test User");
            var nextRole = new Role(2, "Manager");

            _mockUpgradeRequestsRepository.Setup(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier))
                .Returns(upgradeRequest);
            _mockUserRepository.Setup(r => r.getHighestRoleIdBasedOnUserId(requestingUserIdentifier))
                .Returns(currentHighestRoleIdentifier);
            _mockRolesRepository.Setup(r => r.getUpgradedRoleBasedOnCurrentId(currentHighestRoleIdentifier))
                .Returns(nextRole);

            // Act
            _upgradeRequestsService.ProcessUpgradeRequest(true, upgradeRequestIdentifier);

            // Assert
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
            _mockUserRepository.Verify(r => r.getHighestRoleIdBasedOnUserId(requestingUserIdentifier), Times.Once);
            _mockRolesRepository.Verify(r => r.getUpgradedRoleBasedOnCurrentId(currentHighestRoleIdentifier), Times.Once);
            _mockUserRepository.Verify(r => r.addRoleToUser(requestingUserIdentifier, nextRole), Times.Once);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void ProcessUpgradeRequest_WhenDeclined_OnlyRemovesRequest()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;

            // Act
            _upgradeRequestsService.ProcessUpgradeRequest(false, upgradeRequestIdentifier);

            // Assert
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Never);
            _mockUserRepository.Verify(r => r.getHighestRoleIdBasedOnUserId(It.IsAny<int>()), Times.Never);
            _mockRolesRepository.Verify(r => r.getUpgradedRoleBasedOnCurrentId(It.IsAny<int>()), Times.Never);
            _mockUserRepository.Verify(r => r.addRoleToUser(It.IsAny<int>(), It.IsAny<Role>()), Times.Never);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void RemoveUpgradeRequestsFromBannedUsers_RemovesRequestsFromBannedUsers()
        {
            // Arrange
            var upgradeRequests = new List<UpgradeRequest>
            {
                new UpgradeRequest(1, 100, "User 1"),
                new UpgradeRequest(2, 101, "User 2"),
                new UpgradeRequest(3, 102, "Banned User")
            };
            _mockUpgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(upgradeRequests);
            _mockUserRepository.Setup(r => r.getHighestRoleIdBasedOnUserId(100))
                .Returns(1); // Not banned
            _mockUserRepository.Setup(r => r.getHighestRoleIdBasedOnUserId(101))
                .Returns(1); // Not banned
            _mockUserRepository.Setup(r => r.getHighestRoleIdBasedOnUserId(102))
                .Returns(0); // Banned

            // Act
            _upgradeRequestsService.RemoveUpgradeRequestsFromBannedUsers();

            // Assert
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.Once);
            _mockUserRepository.Verify(r => r.getHighestRoleIdBasedOnUserId(100), Times.Once);
            _mockUserRepository.Verify(r => r.getHighestRoleIdBasedOnUserId(101), Times.Once);
            _mockUserRepository.Verify(r => r.getHighestRoleIdBasedOnUserId(102), Times.Once);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(3), Times.Once);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(1), Times.Never);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(2), Times.Never);
        }
    }
} 