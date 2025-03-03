using System;

namespace SacraScriptura.Domain.Common
{
    /// <summary>
    /// Represents a unique identifier for a user in the system.
    /// </summary>
    public class UserId : EntityId
    {
        /// <summary>
        /// Creates a new UserId with the specified value.
        /// </summary>
        /// <param name="value">The string value of the identifier.</param>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid identifier.</exception>
        public UserId(string value) : base(value)
        {
        }

        /// <summary>
        /// Generates a new unique UserId.
        /// </summary>
        /// <returns>A new UserId.</returns>
        public static UserId New()
        {
            return new UserId(GenerateNewId());
        }
    }
}
