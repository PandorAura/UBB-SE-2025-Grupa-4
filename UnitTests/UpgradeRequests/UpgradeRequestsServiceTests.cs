using App1.Models;
using App1.Repositories;
using App1.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.UpgradeRequests
{
    // Test subclass that exposes the protected method for testing
    public class TestableUpgradeRequestsService : UpgradeRequestsService
    {
        public TestableUpgradeRequestsService(
            IUpgradeRequestsRepository upgradeRequestsRepository,
            IRolesRepository rolesRepository,
            IUserRepository userRepository)
            : base(upgradeRequestsRepository, rolesRepository, userRepository)
        {
            // Skip calling RemoveUpgradeRequestsFromBannedUsers in constructor
            // We'll call it explicitly in our test
        }

        // Expose the protected method for testing
        public void PublicRemoveUpgradeRequestsFromBannedUsers()
        {
            this.RemoveUpgradeRequestsFromBannedUsers();
        }
    }

    public class UpgradeRequestsServiceTests
    {
        private readonly Mock<IUpgradeRequestsRepository> mockUpgradeRequestsRepository;
        private readonly Mock<IRolesRepository> mockRolesRepository;
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly UpgradeRequestsService upgradeRequestsService;

        public UpgradeRequestsServiceTests()
        {
            // Setup mocks
            mockUpgradeRequestsRepository = new Mock<IUpgradeRequestsRepository>();
            mockRolesRepository = new Mock<IRolesRepository>();
            mockUserRepository = new Mock<IUserRepository>();

            // Setup default roles for all tests
            var roles = new List<Role>
            {
                new Role(RoleType.Banned, "Banned"),
                new Role(RoleType.User, "User"),
                new Role(RoleType.Manager, "Manager"),
                new Role(RoleType.Admin, "Admin")
            };
            mockRolesRepository.Setup(r => r.GetAllRoles()).Returns(roles);

            // Setup default empty list for RetrieveAllUpgradeRequests
            mockUpgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(new List<UpgradeRequest>());

            // Create service with mocked dependencies
            upgradeRequestsService = new UpgradeRequestsService(
                mockUpgradeRequestsRepository.Object,
                mockRolesRepository.Object,
                mockUserRepository.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesService()
        {
            mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.Once);
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
            mockUpgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(expectedRequests);

            // Act
            var result = upgradeRequestsService.RetrieveAllUpgradeRequests();

            // Assert
            Assert.Equal(expectedRequests, result);
            mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.AtLeastOnce);
        }

        [Fact]
        public void GetRoleNameBasedOnIdentifier_ReturnsRoleName()
        {
            // Arrange
            RoleType roleType = RoleType.User;
            string expectedRoleName = "User";

            // Act
            var result = upgradeRequestsService.GetRoleNameBasedOnIdentifier(roleType);

            // Assert
            Assert.Equal(expectedRoleName, result);
            mockRolesRepository.Verify(r => r.GetAllRoles(), Times.AtLeastOnce);
        }

        [Fact]
        public void ProcessUpgradeRequest_WhenAccepted_UpgradesUserRole()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;
            int requestingUserIdentifier = 100;
            var upgradeRequest = new UpgradeRequest(upgradeRequestIdentifier, requestingUserIdentifier, "Test User");
            var nextRole = new Role(RoleType.Manager, "Manager");

            mockUpgradeRequestsRepository.Setup(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier))
                .Returns(upgradeRequest);
            mockUserRepository.Setup(r => r.GetHighestRoleTypeForUser(requestingUserIdentifier))
                .Returns(RoleType.User);
            mockRolesRepository.Setup(r => r.GetNextRoleInHierarchy(RoleType.User))
                .Returns(nextRole);

            // Act
            upgradeRequestsService.ProcessUpgradeRequest(true, upgradeRequestIdentifier);

            // Assert
            mockUpgradeRequestsRepository.Verify(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
            mockUserRepository.Verify(r => r.GetHighestRoleTypeForUser(requestingUserIdentifier), Times.Once);
            mockRolesRepository.Verify(r => r.GetNextRoleInHierarchy(RoleType.User), Times.Once);
            mockUserRepository.Verify(r => r.AddRoleToUser(requestingUserIdentifier, nextRole), Times.Once);
            mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void ProcessUpgradeRequest_WhenDeclined_OnlyRemovesRequest()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;

            // Act
            upgradeRequestsService.ProcessUpgradeRequest(false, upgradeRequestIdentifier);

            // Assert
            mockUpgradeRequestsRepository.Verify(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Never);
            mockUserRepository.Verify(r => r.GetHighestRoleTypeForUser(It.IsAny<int>()), Times.Never);
            mockRolesRepository.Verify(r => r.GetNextRoleInHierarchy(It.IsAny<RoleType>()), Times.Never);
            mockUserRepository.Verify(r => r.AddRoleToUser(It.IsAny<int>(), It.IsAny<Role>()), Times.Never);
            mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
        }

        [Fact]
        public void RemoveUpgradeRequestsFromBannedUsers_RemovesBannedUsersRequests()
        {
            // Arrange
            var upgradeRequests = new List<UpgradeRequest>
            {
                new UpgradeRequest(1, 100, "Regular User"),
                new UpgradeRequest(2, 101, "Banned User"),
                new UpgradeRequest(3, 102, "Another Regular User"),
                new UpgradeRequest(4, 103, "Another Banned User")
            };

            // Create new mocks specifically for this test
            var mockRepo = new Mock<IUpgradeRequestsRepository>();
            var mockRoles = new Mock<IRolesRepository>();
            var mockUsers = new Mock<IUserRepository>();

            // Setup retrieval of all upgrade requests
            mockRepo.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(upgradeRequests);

            // Setup roles
            mockRoles.Setup(r => r.GetAllRoles())
                .Returns(new List<Role>
                {
                    new Role(RoleType.Banned, "Banned"),
                    new Role(RoleType.User, "User")
                });

            // Setup user repository to identify banned users
            mockUsers.Setup(u => u.GetHighestRoleTypeForUser(101)).Returns(RoleType.Banned);
            mockUsers.Setup(u => u.GetHighestRoleTypeForUser(103)).Returns(RoleType.Banned);
            mockUsers.Setup(u => u.GetHighestRoleTypeForUser(100)).Returns(RoleType.User);
            mockUsers.Setup(u => u.GetHighestRoleTypeForUser(102)).Returns(RoleType.User);

            // Create testable service with our mocks
            var testableService = new TestableUpgradeRequestsService(
                mockRepo.Object,
                mockRoles.Object,
                mockUsers.Object);

            // Act
            testableService.PublicRemoveUpgradeRequestsFromBannedUsers();

            // Assert
            // Verify that only the banned users' requests were removed
            mockRepo.Verify(r => r.RemoveUpgradeRequestByIdentifier(2), Times.AtLeastOnce);
            mockRepo.Verify(r => r.RemoveUpgradeRequestByIdentifier(4), Times.AtLeastOnce);
            mockRepo.Verify(r => r.RemoveUpgradeRequestByIdentifier(1), Times.Never);
            mockRepo.Verify(r => r.RemoveUpgradeRequestByIdentifier(3), Times.Never);
        }
    }
}