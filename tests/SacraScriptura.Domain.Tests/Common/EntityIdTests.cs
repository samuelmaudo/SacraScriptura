using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;
using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain.Tests.Common
{
    public class EntityIdTests
    {
        // Alphabet used for the IDs: 23456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ
        private const string ValidCharacters = "23456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        private const int IdLength = 18;

        [Fact]
        public void EntityId_ShouldHaveCorrectFormat()
        {
            // Arrange & Act
            var userId = UserId.New();
            
            // Assert
            // 1. ID should be exactly 18 characters long
            Assert.Equal(IdLength, userId.Value.Length);
            
            // 2. ID should only contain characters from the base-56 alphabet
            foreach (char c in userId.Value)
            {
                Assert.Contains(c, ValidCharacters);
            }
        }

        [Fact]
        public void EntityIds_ShouldBeOrderedByCreationTime_WhenSortedAlphabetically()
        {
            // Arrange
            var ids = new List<UserId>();
            var timestamps = new List<DateTimeOffset>();
            
            // Act - Create a list of IDs with a small delay between them
            for (int i = 0; i < 3; i++)
            {
                var beforeCreation = DateTimeOffset.UtcNow;
                var id = UserId.New();
                var afterCreation = DateTimeOffset.UtcNow;
                
                ids.Add(id);
                timestamps.Add(beforeCreation);
                
                // Wait 500ms between creations to ensure different timestamps
                Thread.Sleep(500); 
            }
            
            // Sort the IDs alphabetically
            var sortedIds = ids.OrderBy(id => id.Value).ToList();
            
            // Assert
            // Verify that the IDs are in the same order as they were created
            // This confirms that older IDs come first when sorted alphabetically
            for (int i = 0; i < ids.Count; i++)
            {
                Assert.Equal(ids[i], sortedIds[i]);
            }
        }
        
        [Fact]
        public void EntityId_ShouldExtractTimestampCloseToCreationTime()
        {
            // Arrange
            var beforeCreation = DateTimeOffset.UtcNow;
            var id = UserId.New();
            var afterCreation = DateTimeOffset.UtcNow;
            
            // Act
            var extractedTimestamp = id.GetTimestamp();
            
            // Assert
            // Verify that the extracted timestamp is between the before and after timestamps
            // Allow a small tolerance of 1 second to account for processing time
            Assert.True(
                extractedTimestamp >= beforeCreation.AddSeconds(-1) && 
                extractedTimestamp <= afterCreation.AddSeconds(1),
                $"Extracted timestamp ({extractedTimestamp}) should be close to creation time (between {beforeCreation} and {afterCreation})"
            );
        }
        
        [Fact]
        public void EntityId_ShouldHaveTimestampWithTenthsOfSecondPrecision()
        {
            // Arrange
            var id1 = UserId.New();
            Thread.Sleep(100); // Wait for 100ms (1 tenth of a second)
            var id2 = UserId.New();
            
            // Act
            var timestamp1 = id1.GetTimestamp();
            var timestamp2 = id2.GetTimestamp();
            
            // Assert
            // Calculate the difference in tenths of a second
            var diffInTenths = Math.Round((timestamp2 - timestamp1).TotalMilliseconds / 100);
            
            // The difference should be at least 1 tenth of a second
            Assert.True(diffInTenths >= 1, 
                $"Timestamps should have tenths of second precision. Difference: {diffInTenths} tenths of a second");
        }
        
        [Fact]
        public void EntityId_ShouldRejectInvalidFormat()
        {
            // Arrange
            var validId = UserId.New().Value;
            
            // Act & Assert
            // 1. Null or empty should throw
            Assert.Throws<ArgumentException>(() => new UserId(null));
            Assert.Throws<ArgumentException>(() => new UserId(string.Empty));
            Assert.Throws<ArgumentException>(() => new UserId("   "));
            
            // 2. Wrong length should throw
            Assert.Throws<ArgumentException>(() => new UserId(validId.Substring(0, 17))); // Too short
            Assert.Throws<ArgumentException>(() => new UserId(validId + "2")); // Too long
            
            // 3. Invalid characters should throw
            var invalidCharId = "1" + validId.Substring(1); // '1' is not in the alphabet
            Assert.Throws<ArgumentException>(() => new UserId(invalidCharId));
        }
        
        [Fact]
        public void EntityIds_ShouldBeUnique()
        {
            // Arrange & Act
            var ids = new HashSet<string>();
            
            // Generate 100 IDs and ensure they're all unique
            for (int i = 0; i < 100; i++)
            {
                var id = UserId.New().Value;
                
                // Assert
                Assert.DoesNotContain(id, ids);
                ids.Add(id);
            }
        }
    }
}
