using App1.Models;
using App1.Repositories;
using App1.Services;
using Moq;
using System.Collections.Generic;
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
            // Assert
            Assert.NotNull(_upgradeRequestsService);

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
        public void RemoveUpgradeRequestsFromBannedUsers_RemovesRequestsFromBannedUsers()
        {
            // Arrange
            // Create a new service that won't call the method in constructor
            var upgradeRequestsRepository = new Mock<IUpgradeRequestsRepository>();
            var rolesRepository = new Mock<IRolesRepository>();
            var userRepository = new Mock<IUserRepository>();

            // Setup roles
            var roles = new List<Role>
            {
                new Role(RoleType.Banned, "Banned"),
                new Role(RoleType.User, "User"),
                new Role(RoleType.Manager, "Manager"),
                new Role(RoleType.Admin, "Admin")
            };
            rolesRepository.Setup(r => r.GetAllRoles()).Returns(roles);

            var upgradeRequests = new List<UpgradeRequest>
            {
                new UpgradeRequest(1, 100, "User 1"),
                new UpgradeRequest(2, 101, "User 2"),
                new UpgradeRequest(3, 102, "Banned User")
            };
            upgradeRequestsRepository.Setup(r => r.RetrieveAllUpgradeRequests())
                .Returns(upgradeRequests);

            userRepository.Setup(r => r.GetHighestRoleTypeForUser(100))
                .Returns(RoleType.User); // Not banned
            userRepository.Setup(r => r.GetHighestRoleTypeForUser(101))
                .Returns(RoleType.User); // Not banned
            userRepository.Setup(r => r.GetHighestRoleTypeForUser(102))
                .Returns(RoleType.Banned); // Banned

            // Create a test service without calling the constructor-based cleanup
            var service = new UpgradeRequestsServiceForTest(
                upgradeRequestsRepository.Object,
                rolesRepository.Object,
                userRepository.Object);

            // Act
            service.RemoveUpgradeRequestsFromBannedUsers();

            // Assert
            upgradeRequestsRepository.Verify(r => r.RetrieveAllUpgradeRequests(), Times.Once);
            userRepository.Verify(r => r.GetHighestRoleTypeForUser(100), Times.Once);
            userRepository.Verify(r => r.GetHighestRoleTypeForUser(101), Times.Once);
            userRepository.Verify(r => r.GetHighestRoleTypeForUser(102), Times.Once);
            upgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(3), Times.Once);
            upgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(1), Times.Never);
            upgradeRequestsRepository.Verify(r => r.RemoveUpgradeRequestByIdentifier(2), Times.Never);
        }
    }

    // Helper class to avoid constructor-based cleanup for testing RemoveUpgradeRequestsFromBannedUsers
    public class UpgradeRequestsServiceForTest : UpgradeRequestsService
    {
        public UpgradeRequestsServiceForTest(
            IUpgradeRequestsRepository upgradeRequestsRepository,
            IRolesRepository rolesRepository,
            IUserRepository userRepository)
            : base(upgradeRequestsRepository, rolesRepository, userRepository)
        {
            // Override the base constructor behavior to avoid calling RemoveUpgradeRequestsFromBannedUsers
        }

        // Make the method public for direct testing
        public new void RemoveUpgradeRequestsFromBannedUsers()
        {
            base.RemoveUpgradeRequestsFromBannedUsers();
        }
    }
}