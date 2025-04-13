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
            base.RemoveUpgradeRequestsFromBannedUsers();
        }
    }

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

            // Setup default roles for all tests
            var roles = new List<Role>
            {
                new Role(RoleType.Banned, "Banned"),
                new Role(RoleType.User, "User"),
                new Role(RoleType.Manager, "Manager"),
                new Role(RoleType.Admin, "Admin")
            };
            _mockRolesRepository.Setup(r => r.GetAllRoles()).Returns(roles);

            // Setup default empty list for RetrieveAllUpgradeRequests
            _mockUpgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(new List<UpgradeRequest>());

            // Create service with mocked dependencies
            _upgradeRequestsService = new UpgradeRequestsService(
                _mockUpgradeRequestsRepository.Object,
                _mockRolesRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesService()
        {
           

            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.Once);
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
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.AtLeastOnce);
        }

        [Fact]
        public void GetRoleNameBasedOnIdentifier_ReturnsRoleName()
        {
            // Arrange
            RoleType roleType = RoleType.User;
            string expectedRoleName = "User";

            // Act
            var result = _upgradeRequestsService.GetRoleNameBasedOnIdentifier(roleType);

            // Assert
            Assert.Equal(expectedRoleName, result);
            _mockRolesRepository.Verify(r => r.GetAllRoles(), Times.AtLeastOnce);
        }

        [Fact]
        public void ProcessUpgradeRequest_WhenAccepted_UpgradesUserRole()
        {
            // Arrange
            int upgradeRequestIdentifier = 1;
            int requestingUserIdentifier = 100;
            var upgradeRequest = new UpgradeRequest(upgradeRequestIdentifier, requestingUserIdentifier, "Test User");
            var nextRole = new Role(RoleType.Manager, "Manager");

            _mockUpgradeRequestsRepository.Setup(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier))
                .Returns(upgradeRequest);
            _mockUserRepository.Setup(r => r.GetHighestRoleTypeForUser(requestingUserIdentifier))
                .Returns(RoleType.User);
            _mockRolesRepository.Setup(r => r.GetNextRole(RoleType.User))
                .Returns(nextRole);

            // Act
            _upgradeRequestsService.ProcessUpgradeRequest(true, upgradeRequestIdentifier);

            // Assert
            _mockUpgradeRequestsRepository.Verify(r => r.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
            _mockUserRepository.Verify(r => r.GetHighestRoleTypeForUser(requestingUserIdentifier), Times.Once);
            _mockRolesRepository.Verify(r => r.GetNextRole(RoleType.User), Times.Once);
            _mockUserRepository.Verify(r => r.AddRoleToUser(requestingUserIdentifier, nextRole), Times.Once);
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
            _mockUserRepository.Verify(r => r.GetHighestRoleTypeForUser(It.IsAny<int>()), Times.Never);
            _mockRolesRepository.Verify(r => r.GetNextRole(It.IsAny<RoleType>()), Times.Never);
            _mockUserRepository.Verify(r => r.AddRoleToUser(It.IsAny<int>(), It.IsAny<Role>()), Times.Never);
            _mockUpgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier), Times.Once);
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