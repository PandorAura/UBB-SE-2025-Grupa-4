using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.UpgradeRequests
{
    public class UpgradeRequestTests
    {
        [Fact]
        public void Constructor_WithValidParameters_CreatesUpgradeRequestWithCorrectProperties()
        {
            // Arrange
            int expectedUpgradeRequestId = 1;
            int expectedRequestingUserIdentifier = 100;
            string expectedRequestingUserDisplayName = "Test User";

            // Act
            UpgradeRequest upgradeRequest = new UpgradeRequest(
                expectedUpgradeRequestId,
                expectedRequestingUserIdentifier,
                expectedRequestingUserDisplayName);

            // Assert
            Assert.Equal(expectedUpgradeRequestId, upgradeRequest.UpgradeRequestId);
            Assert.Equal(expectedRequestingUserIdentifier, upgradeRequest.RequestingUserIdentifier);
            Assert.Equal(expectedRequestingUserDisplayName, upgradeRequest.RequestingUserDisplayName);
        }

        [Fact]
        public void ToString_ReturnsRequestingUserDisplayName()
        {
            // Arrange
            int upgradeRequestId = 1;
            int requestingUserIdentifier = 100;
            string expectedDisplayName = "Test User";
            UpgradeRequest upgradeRequest = new UpgradeRequest(
                upgradeRequestId,
                requestingUserIdentifier,
                expectedDisplayName);

            // Act
            string result = upgradeRequest.ToString();

            // Assert
            Assert.Equal(expectedDisplayName, result);
        }
    }
} 