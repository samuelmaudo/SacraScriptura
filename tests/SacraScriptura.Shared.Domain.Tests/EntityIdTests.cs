using SacraScriptura.Shared.Domain;
using Xunit;

namespace SacraScriptura.Shared.Domain.Tests;

public class UserId : EntityId
{
    public UserId()
    {
    }

    public UserId(string value) : base(value)
    {
    }

    public UserId(DateTimeOffset dateTimeOffset) : base(dateTimeOffset)
    {
    }
}

public class EntityIdTests
{
    // Alphabet used for the IDs: 123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz
    private const string ValidCharacters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    private const int IdLength = 18;

    [Fact]
    public void EntityId_ShouldHaveCorrectFormat()
    {
        // Arrange & Act
        var userId = new UserId();

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

        // Act - Create a list of IDs with a small delay between them
        for (int i = 0; i < 3; i++)
        {
            ids.Add(new UserId());

            // Wait 5ms between creations to ensure different timestamps
            Thread.Sleep(5);
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
        var id = new UserId();
        var afterCreation = DateTimeOffset.UtcNow;

        // Act
        var extractedTimestamp = id.GetTimestamp();

        // Assert
        // Verify that the extracted timestamp is between the before and after timestamps
        // Allow a small tolerance of 1 second to account for processing time
        Assert.True(
            extractedTimestamp >= beforeCreation.AddSeconds(-1)
            && extractedTimestamp <= afterCreation.AddSeconds(1),
            $"Extracted timestamp ({extractedTimestamp}) should be close to creation time (between {beforeCreation} and {afterCreation})"
        );
    }

    [Fact]
    public void EntityId_ShouldHaveTimestampWithMillisecondPrecision()
    {
        // Arrange
        var id1 = new UserId();
        Thread.Sleep(1); // Wait for 1ms
        var id2 = new UserId();

        // Act
        var timestamp1 = id1.GetTimestamp();
        var timestamp2 = id2.GetTimestamp();

        // Assert
        // Calculate the difference in milliseconds
        var diffInMilliseconds = Math.Round((timestamp2 - timestamp1).TotalMilliseconds);

        // The difference should be at least 1 millisecond
        Assert.True(
            diffInMilliseconds >= 1,
            $"Timestamps should have millisecond precision. Difference: {diffInMilliseconds} milliseconds"
        );
    }

    [Fact]
    public void EntityId_ShouldRejectInvalidFormat()
    {
        // Arrange
        var validId = new UserId().Value;

        // Act & Assert
        // 1. Null or empty should throw
        Assert.Throws<ArgumentException>(() => new UserId(null));
        Assert.Throws<ArgumentException>(() => new UserId(string.Empty));
        Assert.Throws<ArgumentException>(() => new UserId("   "));

        // 2. Wrong length should throw
        Assert.Throws<ArgumentException>(() => new UserId(validId.Substring(0, 17))); // Too short
        Assert.Throws<ArgumentException>(() => new UserId(validId + "2")); // Too long

        // 3. Invalid characters should throw
        var invalidCharId = "0" + validId.Substring(1); // '0' is not in the alphabet
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
            var id = new UserId().Value;

            // Assert
            Assert.DoesNotContain(id, ids);
            ids.Add(id);
        }
    }
}