using System.Numerics;
using Xunit;
using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain.Tests.Common
{
    public class Base58Tests
    {
        [Fact]
        public void FromBigInteger_EmptyArray_ReturnsEmptyString()
        {
            // Arrange
            BigInteger zeroInteger = 0;
            
            // Act
            string result = Base58.FromBigInteger(zeroInteger);
            
            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FromBigInteger_SingleByte_ReturnsCorrectEncoding()
        {
            // Test a few sample values that map directly to the alphabet
            (BigInteger, string)[] testCases = [
                (0, ""),      // 0 maps to empty string since BigInteger ignores leading zeros
                (1, "2"),     // 1 maps to '2' (index 1 in alphabet)
                (10, "B"),    // 10 maps to 'B' (index 10 in alphabet)
                (55, "x")     // 55 maps to 'x' (index 55 in alphabet)
            ];

            foreach (var (input, expected) in testCases)
            {
                // Act
                string result = Base58.FromBigInteger(input);
                
                // Assert
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void FromBigInteger_MultipleBytes_ReturnsCorrectEncoding()
        {
            // Test cases with known expected outputs
            (BigInteger, string)[] testCases = [
                (1, "2"),
                (258, "5T"),
                (65535, "LUv"),
                (66051, "Ldp"),
                (2147483647, "4GmR58"), // Max 32-bit integer
            ];

            foreach (var (input, expected) in testCases)
            {
                // Act
                string result = Base58.FromBigInteger(input);
                
                // Assert
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ToBigInteger_EmptyString_ReturnsEmptyArray()
        {
            // Arrange
            string emptyString = string.Empty;
            
            // Act
            BigInteger result = Base58.ToBigInteger(emptyString);
            
            // Assert
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void ToBigInteger_NullString_ReturnsEmptyArray()
        {
            // Arrange
            string nullString = null;
            
            // Act
            BigInteger result = Base58.ToBigInteger(nullString);
            
            // Assert
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void ToBigInteger_InvalidCharacter_ThrowsArgumentException()
        {
            // Arrange - "0" is not in the alphabet
            string invalidInput = "0abc";
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                Base58.ToBigInteger(invalidInput));
            
            Assert.Contains("Character '0' is not in the alphabet", exception.Message);
        }
    }
}
