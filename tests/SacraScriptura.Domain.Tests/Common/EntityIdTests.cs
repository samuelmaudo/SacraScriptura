using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain.Tests.Common
{
    public class EntityIdTests
    {
        [Fact]
        public void EntityIds_ShouldBeOrderedByCreationTime_WhenSortedAlphabetically()
        {
            // Arrange
            var ids = new List<UserId>();
            
            // Act - Create a list of IDs with a small delay between them
            for (int i = 0; i < 5; i++)
            {
                ids.Add(UserId.New());
                Thread.Sleep(200); // Wait 200ms between creations to ensure different timestamps
            }
            
            // Get creation timestamps for verification
            var creationTimestamps = ids.Select(id => id.GetTimestamp()).ToList();
            
            // Sort the IDs alphabetically
            var sortedIds = ids.OrderBy(id => id.Value).ToList();
            var sortedTimestamps = sortedIds.Select(id => id.GetTimestamp()).ToList();
            
            // Assert - Verify that the timestamps are in the same order after sorting
            for (int i = 0; i < ids.Count - 1; i++)
            {
                Assert.True(
                    sortedTimestamps[i] <= sortedTimestamps[i + 1],
                    $"ID at position {i} should have an earlier or equal timestamp than ID at position {i + 1}"
                );
            }
            
            // Also verify that the original order matches the sorted order
            for (int i = 0; i < ids.Count; i++)
            {
                Assert.Equal(ids[i], sortedIds[i]);
            }
        }
        
        [Fact]
        public void EntityId_ShouldExtractCorrectTimestamp()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            var id = UserId.New();
            
            // Act
            var extractedTimestamp = id.GetTimestamp();
            
            // Assert - Verify that the extracted timestamp is close to the current time
            // We allow a small tolerance of 1 second to account for processing time
            var difference = Math.Abs((extractedTimestamp - now).TotalSeconds);
            Assert.True(difference < 1, $"Extracted timestamp should be within 1 second of creation time. Difference: {difference} seconds");
        }
        
        [Fact]
        public void EntityId_ShouldValidateFormat()
        {
            // Arrange & Act & Assert
            
            // Valid ID should not throw
            var validId = UserId.New();
            var validIdString = validId.Value;
            var parsedId = UserId.From(validIdString); // Should not throw
            
            // Invalid length should throw
            var tooShortId = validIdString.Substring(0, validIdString.Length - 1);
            var tooLongId = validIdString + "2";
            
            Assert.Throws<ArgumentException>(() => UserId.From(tooShortId));
            Assert.Throws<ArgumentException>(() => UserId.From(tooLongId));
            
            // Invalid characters should throw
            var invalidCharId = validIdString.Substring(0, validIdString.Length - 1) + "1"; // '1' is not in the alphabet
            Assert.Throws<ArgumentException>(() => UserId.From(invalidCharId));
        }
    }
}
