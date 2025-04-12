using System;
using System.Collections.Generic;
using System.Linq;
using App1.AutoChecker;
using Moq;
using Xunit;

namespace UnitTests.Autocheck
{
    public class AutoCheckTests
    {
        private readonly Mock<IOffensiveWordsRepository> _mockRepository;
        private readonly HashSet<string> _testOffensiveWords;
        private readonly AutoCheck _autoCheck;

        public AutoCheckTests()
        {
            _testOffensiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bad",
                "offensive",
                "inappropriate"
            };

            _mockRepository = new Mock<IOffensiveWordsRepository>();
            _mockRepository.Setup(r => r.LoadOffensiveWords()).Returns(_testOffensiveWords);

            _autoCheck = new AutoCheck(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AutoCheck(null));
        }

        [Fact]
        public void AutoCheckReview_NullReviewText_ReturnsFalse()
        {
            // Act
            var result = _autoCheck.AutoCheckReview(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AutoCheckReview_EmptyReviewText_ReturnsFalse()
        {
            // Act
            var result = _autoCheck.AutoCheckReview(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AutoCheckReview_WhitespaceReviewText_ReturnsFalse()
        {
            // Act
            var result = _autoCheck.AutoCheckReview("   ");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AutoCheckReview_ContainsOffensiveWord_ReturnsTrue()
        {
            // Arrange
            var reviewText = "This review contains an offensive word.";

            // Act
            var result = _autoCheck.AutoCheckReview(reviewText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AutoCheckReview_ContainsOffensiveWordWithDifferentCase_ReturnsTrue()
        {
            // Arrange
            var reviewText = "This review contains an OFFENSIVE word.";

            // Act
            var result = _autoCheck.AutoCheckReview(reviewText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AutoCheckReview_NoOffensiveWords_ReturnsFalse()
        {
            // Arrange
            var reviewText = "This review is perfectly fine and acceptable.";

            // Act
            var result = _autoCheck.AutoCheckReview(reviewText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AutoCheckReview_OffensiveWordSurroundedByDelimiters_ReturnsTrue()
        {
            // Arrange
            var reviewText = "This review has!bad,punctuation.";

            // Act
            var result = _autoCheck.AutoCheckReview(reviewText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AddOffensiveWord_NewWord_AddsToRepositoryAndLocalCache()
        {
            // Arrange
            var newWord = "terrible";
            _mockRepository.Setup(r => r.AddWord(newWord)).Verifiable();

            // Act
            _autoCheck.AddOffensiveWord(newWord);
            var words = _autoCheck.GetOffensiveWordsList();

            // Assert
            _mockRepository.Verify(r => r.AddWord(newWord), Times.Once);
            Assert.Contains(newWord, words);
        }

        [Fact]
        public void AddOffensiveWord_ExistingWord_DoesNotAddAgain()
        {
            // Arrange
            var existingWord = "offensive";

            // Act
            _autoCheck.AddOffensiveWord(existingWord);

            // Assert
            _mockRepository.Verify(r => r.AddWord(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void AddOffensiveWord_ExistingWordDifferentCase_DoesNotAddAgain()
        {
            // Arrange
            var existingWord = "OFFENSIVE";

            // Act
            _autoCheck.AddOffensiveWord(existingWord);

            // Assert
            _mockRepository.Verify(r => r.AddWord(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void AddOffensiveWord_NullOrEmptyWord_DoesNothing()
        {
            // Act
            _autoCheck.AddOffensiveWord(null);
            _autoCheck.AddOffensiveWord(string.Empty);
            _autoCheck.AddOffensiveWord("   ");

            // Assert
            _mockRepository.Verify(r => r.AddWord(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void DeleteOffensiveWord_ExistingWord_RemovesFromRepositoryAndLocalCache()
        {
            // Arrange
            var wordToDelete = "offensive";
            _mockRepository.Setup(r => r.DeleteWord(wordToDelete)).Verifiable();

            // Act
            _autoCheck.DeleteOffensiveWord(wordToDelete);
            var words = _autoCheck.GetOffensiveWordsList();

            // Assert
            _mockRepository.Verify(r => r.DeleteWord(wordToDelete), Times.Once);
            Assert.DoesNotContain(wordToDelete, words);
        }

        [Fact]
        public void DeleteOffensiveWord_NonExistingWord_DoesNothing()
        {
            // Arrange
            var nonExistingWord = "nonexistent";

            // Act
            _autoCheck.DeleteOffensiveWord(nonExistingWord);

            // Assert
            _mockRepository.Verify(r => r.DeleteWord(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void DeleteOffensiveWord_NullOrEmptyWord_DoesNothing()
        {
            // Act
            _autoCheck.DeleteOffensiveWord(null);
            _autoCheck.DeleteOffensiveWord(string.Empty);
            _autoCheck.DeleteOffensiveWord("   ");

            // Assert
            _mockRepository.Verify(r => r.DeleteWord(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetOffensiveWordsList_ReturnsNewInstanceWithSameContent()
        {
            // Act
            var result = _autoCheck.GetOffensiveWordsList();

            // Assert
            Assert.Equal(_testOffensiveWords.Count, result.Count);
            foreach (var word in _testOffensiveWords)
            {
                Assert.Contains(word, result);
            }
            Assert.NotSame(_testOffensiveWords, result);
        }
    }
}