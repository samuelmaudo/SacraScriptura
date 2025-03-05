using System.Numerics;
using System.Text;

namespace SacraScriptura.Domain.Common;

/// <summary>
/// Converts integer numbers to strings using a base-58 alphabet.
/// </summary>
public static class Base58
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    private const int Base = 58; // Alphabet length

    /// <summary>
    /// Converts an integer number to a base-58 string.
    /// </summary>
    /// <param name="number">An arbitrarily large signed integer.</param>
    public static string FromBigInteger(BigInteger number)
    {
        var sb = new StringBuilder();
        var target = number;

        // Dividir repetidamente entre 58, guardando los residuos.
        while (target > 0)
        {
            BigInteger remainder;
            target = BigInteger.DivRem(target, Base, out remainder);
            // El residuo es un índice en el alfabeto Base58.
            sb.Insert(0, Alphabet[(int)remainder]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a base-58 string to an integer number.
    /// </summary>
    /// <param name="string">A base-58 encoded string.</param>
    /// <exception cref="ArgumentException">Thrown when the string contains invalid characters.</exception>
    public static BigInteger ToBigInteger(string @string)
    {
        BigInteger result = 0;
        if (string.IsNullOrEmpty(@string))
        {
            return result;
        }

        foreach (char c in @string)
        {
            int digit = Alphabet.IndexOf(c);
            if (digit < 0) {
                throw new ArgumentException($"Character '{c}' is not in the alphabet.");
            }
            result = result * Base + digit;
        }

        return result;
    }

    /// <summary>
    /// Checks if a base-58 string is a valid.
    /// </summary>
    /// <param name="string">A base-58 encoded string.</param>
    public static bool IsValidId(string @string)
    {
        foreach (char c in @string)
        {
            if (!Alphabet.Contains(c))
            {
                return false;
            }
        }
            
        return true;
    }
}