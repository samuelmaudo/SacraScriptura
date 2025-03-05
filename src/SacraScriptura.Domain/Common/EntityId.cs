using System.Numerics;
using System.Security.Cryptography;

namespace SacraScriptura.Domain.Common
{
    /// <summary>
    /// Base class for all entity identifiers in the system.
    /// Generates 18-character identifiers using a base-56 alphabet with a timestamp component (millisecond precision)
    /// followed by cryptographically secure random characters.
    /// Identifiers are designed to be ordered from oldest to newest when sorted alphabetically.
    /// </summary>
    public abstract class EntityId : IEquatable<EntityId>, IComparable<EntityId>
    {
        private const int Length = 18;

        // Número total de bits para el identificador (106 bits)
        private const int TotalBits = 106;

        // Bits reservados para el timestamp (48 bits)
        private const int TimestampBits = 48;

        // Bits para la entropía aleatoria
        private const int RandomBits = TotalBits - TimestampBits; // 58 bits

        // Unix epoch for timestamp calculations (January 1, 2020)
        private const long EpochTimestamp = 1577836800;

        /// <summary>
        /// Gets the string representation of this identifier.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new EntityId.
        /// </summary>
        protected EntityId()
        {
            Value = GenerateIdentifier(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

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

            if (value.Length != Length)
            {
                throw new ArgumentException($"Identifier must be exactly {Length} characters long.", nameof(value));
            }

            if (!Base58.IsValidId(value))
            {
                throw new ArgumentException("Identifier contains invalid characters.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Creates a new EntityId with the specified timestamp.
        /// </summary>
        /// <param name="dateTimeOffset">The timestamp.</param>
        protected EntityId(DateTimeOffset dateTimeOffset)
        {
            Value = GenerateIdentifier(dateTimeOffset.ToUnixTimeMilliseconds());
        }

        /// <summary>
        /// Extracts the timestamp from an identifier.
        /// </summary>
        /// <returns>The timestamp as a DateTimeOffset.</returns>
        public DateTimeOffset GetTimestamp()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(ExtractTimestamp(Value));
        }

        /// <summary>
        /// Genera el identificador de 18 caracteres en Base58.
        /// </summary>
        private static string GenerateIdentifier(long timestamp)
        {
            long millisecondsSinceEpoch = timestamp - EpochTimestamp;

            // Nos aseguramos de que el timestamp cabe en 48 bits (no se excederá por años futuros)
            if (millisecondsSinceEpoch >= (1L << TimestampBits))
            {
                throw new InvalidOperationException("El timestamp excede los 48 bits reservados.");
            }

            // Convertir el timestamp a BigInteger (ya está en base 10)
            BigInteger timestampPart = millisecondsSinceEpoch;

            // Paso 2: Generar 58 bits de entropía aleatoria
            BigInteger randomPart = GenerateRandomBits(RandomBits);

            // Paso 3: Combinar: colocar el timestamp en los bits más significativos y la entropía en los bits menos significativos.
            // Como el timestamp ocupa 48 bits, se desplaza 58 bits a la izquierda y se une (bitwise OR) con la parte aleatoria.
            BigInteger combined = (timestampPart << RandomBits) | randomPart;

            // Paso 4: Convertir el número combinado a Base58.
            string base58 = Base58.FromBigInteger(combined);

            // Asegurarse de que el resultado tenga 18 caracteres (se rellena con '1', que equivale al dígito 0 en Base58).
            base58 = base58.PadLeft(18, '1');

            return base58;
        }

        /// <summary>
        /// Genera un número aleatorio de 'bits' bits como BigInteger.
        /// </summary>
        private static BigInteger GenerateRandomBits(int bits)
        {
            int bytes = (bits + 7) / 8; // Redondear al byte más cercano.
            byte[] data = new byte[bytes];
            RandomNumberGenerator.Fill(data);

            // Asegurarse de que los bits no deseados (en el byte más significativo) sean cero.
            int extraBits = bytes * 8 - bits;
            if (extraBits > 0)
            {
                byte mask = (byte)(0xFF >> extraBits);
                data[0] &= mask;
            }

            return new BigInteger(data, isUnsigned: true, isBigEndian: true);
        }

        /// <summary>
        /// Decodifica el identificador Base58 y extrae el timestamp (48 bits) en milisegundos.
        /// </summary>
        private static long ExtractTimestamp(string value)
        {
            // Convertir el string Base58 a BigInteger.
            var combined = Base58.ToBigInteger(value);

            // Desplazar a la derecha 58 bits para obtener los 48 bits más significativos (timestamp).
            var timestampPart = combined >> RandomBits;

            var millisecondsSinceEpoch = (long)timestampPart;

            return millisecondsSinceEpoch + EpochTimestamp;
        }

        /// <summary>
        /// Returns the string representation of this identifier.
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Determines whether this identifier is equal to another object.
        /// </summary>
        public override bool Equals(object? obj)
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
        public bool Equals(EntityId? other)
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
        public int CompareTo(EntityId? other)
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
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right) => !(left == right);

        public static bool operator <(EntityId left, EntityId right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EntityId left, EntityId right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EntityId left, EntityId right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EntityId left, EntityId right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}