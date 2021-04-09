using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace Blueprint.Polly
{
    /// <summary>
    /// Applies Guard design pattern for validating data https://github.com/ardalis/GuardClauses.
    /// </summary>
    public static class Guard
    {
        public static void AgainstNull(string argumentName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AgainstNullOrEmpty(string argumentName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AgainstNullOrEmpty(string argumentName, ICollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (value.Count == 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        public static void AgainstNegativeOrZero(string argumentName, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        public static void AgainstNegativeOrZero(string argumentName, TimeSpan value)
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        public static void AgainstNegative(string argumentName, int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        public static void AgainstNegative(string argumentName, TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        public static void AgainstNotFound(string argumentName, object value)
        {
            if (value == null)
            {
                throw new NotFoundException($"Entity \"{argumentName}\" was not found.");
            }
        }

        public static void AgainstNotFound(string argumentName, object value, object keyValue)
        {
            if (value == null)
            {
                throw new NotFoundException($"Entity \"{argumentName}\" ({keyValue}) was not found.");
            }
        }

        public static void AgainstNullOrNotAny<T>(string argumentName, ICollection<T> value)
        {
            if (value == null || !value.Any())
            {
                throw new NotFoundException($"There are no items in {argumentName}.");
            }
        }

        public static void AgainstNullOrNotAny<T>(string argumentName, IEnumerable<T> value)
        {
            if (value == null || !value.Any())
            {
                throw new NotFoundException($"There are no items in {argumentName}.");
            }
        }

        public static void AgainstInvalidArgument(string argumentName, bool isValid)
        {
            if (!isValid)
            {
                throw new ArgumentException($"The parameter {argumentName} is not valid.");
            }
        }

        public static void AgainstInvalidArgumentWithMessage(string message, bool isValid)
        {
            if (!isValid)
            {
                throw new ArgumentException(message);
            }
        }

        public static void AgainstInvalidOperationWithMessage(string message, bool isValid)
        {
            if (!isValid)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AgainstInvalidOperation(string argumentName, bool isValid)
        {
            if (!isValid)
            {
                throw new InvalidOperationException($"This operation \"{argumentName}\" is not allowed.");
            }
        }

        public static void AgainstInvalidAuthentication(string message, bool isValid)
        {
            if (!isValid)
            {
                throw new AuthenticationException(message);
            }
        }
    }
}
