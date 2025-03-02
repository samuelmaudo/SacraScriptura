using System;

namespace SacraScriptura.Domain.Common
{
    /// <summary>
    /// Represents a unique identifier for a user.
    /// </summary>
    public class UserId : EntityId
    {
        private UserId(string value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new UserId with a generated value.
        /// </summary>
        public static UserId New() => new UserId(GenerateNewId());

        /// <summary>
        /// Creates a UserId from an existing string value.
        /// </summary>
        /// <param name="value">The string representation of the user ID.</param>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid identifier.</exception>
        public static UserId From(string value) => new UserId(value);
    }
}
