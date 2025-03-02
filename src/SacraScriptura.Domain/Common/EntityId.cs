using System;
using System.Globalization;
using System.Security.Cryptography;

namespace SacraScriptura.Domain.Common
{
    /// <summary>
    /// Base class for all entity identifiers in the system.
    /// Generates 18-character identifiers using a base-56 alphabet with a timestamp component (tenths of a second precision)
    /// followed by cryptographically secure random characters.
    /// Identifiers are designed to be ordered from oldest to newest when sorted alphabetically.
    /// </summary>
    public abstract class EntityId : IEquatable<EntityId>, IComparable<EntityId>
    {
        private const string Alphabet = "23456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        private const int IdLength = 18;
        private const int Base = 56; // Alphabet length
        private const int TimestampLength = 8; // Length of the timestamp part in the ID
        private const long MaxTimestamp = 281474976710655; // Maximum value that can be represented in TimestampLength characters in base-56

        /// <summary>
        /// Gets the string representation of this identifier.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new EntityId with the specified value.
        /// </summary>
        /// <param name="value">The string value of the identifier.</param>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid identifier.</exception>
        protected EntityId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(value));
            }

            if (value.Length != IdLength)
            {
                throw new ArgumentException($"Identifier must be exactly {IdLength} characters long.", nameof(value));
            }

            if (!IsValidId(value))
            {
                throw new ArgumentException("Identifier contains invalid characters.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Generates a new unique identifier.
        /// </summary>
        /// <returns>A new identifier string.</returns>
        protected static string GenerateNewId()
        {
            // Get current timestamp with tenths of a second precision
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 100; // Convert to tenths of a second
            
            // Invert the timestamp to ensure older timestamps come first alphabetically
            // We subtract from a large constant to ensure the timestamp part has consistent length
            var invertedTimestamp = MaxTimestamp - timestamp;
            
            // Convert inverted timestamp to base-56
            var timestampPart = ConvertToBase56(invertedTimestamp);
            
            // Pad timestamp part to ensure consistent length
            var paddedTimestampPart = timestampPart.PadLeft(TimestampLength, Alphabet[0]);
            
            // Calculate how many random characters we need
            var randomPartLength = IdLength - paddedTimestampPart.Length;
            
            // Generate random part
            var randomPart = GenerateRandomString(randomPartLength);
            
            // Combine parts
            return paddedTimestampPart + randomPart;
        }

        /// <summary>
        /// Extracts the timestamp from an identifier.
        /// </summary>
        /// <returns>The timestamp as a DateTimeOffset.</returns>
        public DateTimeOffset GetTimestamp()
        {
            // Extract the timestamp part (first TimestampLength characters)
            var timestampPart = Value.Substring(0, TimestampLength);
            
            // Convert from base-56 to decimal
            var invertedTimestampTenthsOfSecond = ConvertFromBase56(timestampPart);
            
            // Invert back to get the original timestamp
            var timestampTenthsOfSecond = MaxTimestamp - invertedTimestampTenthsOfSecond;
            
            // Convert to DateTimeOffset
            return DateTimeOffset.FromUnixTimeMilliseconds(timestampTenthsOfSecond * 100);
        }

        /// <summary>
        /// Converts a decimal number to a base-56 string using the defined alphabet.
        /// </summary>
        private static string ConvertToBase56(long value)
        {
            if (value == 0)
            {
                return Alphabet[0].ToString();
            }

            var result = string.Empty;
            
            while (value > 0)
            {
                result = Alphabet[(int)(value % Base)] + result;
                value /= Base;
            }
            
            return result;
        }

        /// <summary>
        /// Converts a base-56 string back to a decimal number.
        /// </summary>
        private static long ConvertFromBase56(string value)
        {
            long result = 0;
            
            foreach (char c in value)
            {
                result = result * Base + Alphabet.IndexOf(c);
            }
            
            return result;
        }

        /// <summary>
        /// Generates a cryptographically secure random string of the specified length.
        /// </summary>
        private static string GenerateRandomString(int length)
        {
            var result = new char[length];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[length];
                rng.GetBytes(buffer);
                
                for (int i = 0; i < length; i++)
                {
                    result[i] = Alphabet[buffer[i] % Alphabet.Length];
                }
            }
            
            return new string(result);
        }

        /// <summary>
        /// Checks if a string is a valid identifier.
        /// </summary>
        private static bool IsValidId(string value)
        {
            foreach (char c in value)
            {
                if (Alphabet.IndexOf(c) == -1)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Returns the string representation of this identifier.
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Determines whether this identifier is equal to another object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is EntityId other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this identifier is equal to another identifier.
        /// </summary>
        public bool Equals(EntityId other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Different types of IDs should never be equal
            if (GetType() != other.GetType())
            {
                return false;
            }

            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a hash code for this identifier.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

        /// <summary>
        /// Compares this identifier to another identifier.
        /// </summary>
        public int CompareTo(EntityId other)
        {
            if (other is null)
            {
                return 1;
            }

            // Different types of IDs should be sorted by type name first
            if (GetType() != other.GetType())
            {
                return string.Compare(GetType().Name, other.GetType().Name, StringComparison.Ordinal);
            }

            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right) => !(left == right);

        public static bool operator <(EntityId left, EntityId right)
        {
            if (left is null)
            {
                return !(right is null);
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EntityId left, EntityId right)
        {
            if (left is null)
            {
                return true;
            }

            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EntityId left, EntityId right)
        {
            if (left is null)
            {
                return false;
            }

            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EntityId left, EntityId right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.CompareTo(right) >= 0;
        }
    }
}
