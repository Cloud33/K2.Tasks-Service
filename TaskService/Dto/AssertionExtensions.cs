using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    /// <summary>
    /// Assertion extension class.
    /// Provides methods for object assertion.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Indicates whether an object instance is null.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <returns>True or false</returns>
        public static bool Null(this object instance)
        {
            return instance == null;
        }

        /// <summary>
        /// Indicates whether an object instance is not null.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <returns>True or false</returns>
        public static bool NotNull(this object instance)
        {
            return instance != null;
        }

        /// <summary>
        /// Indicates whether a enumerable collection is null or empty.
        /// </summary>
        /// <typeparam name="T">Collection instance type</typeparam>
        /// <param name="enumerable">Enumerable instance</param>
        /// <returns>True or false</returns>
        public static bool NullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Indicates whether a enumerable collection is not null or empty.
        /// </summary>
        /// <typeparam name="T">Collection instance type</typeparam>
        /// <param name="enumerable">Enumerable instance</param>
        /// <returns>True or false</returns>
        public static bool NotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        /// <summary>
        /// Indicates whether a guid is empty. 
        /// </summary>
        /// <param name="guid">Guid instance</param>
        /// <returns>True or false</returns>
        public static bool Empty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Indicates whether a guid is not empty. 
        /// </summary>
        /// <param name="guid">Guid instance</param>
        /// <returns>True or false</returns>
        public static bool NotEmpty(this Guid guid)
        {
            return guid != Guid.Empty;
        }

        /// <summary>
        /// Indicates whether a string is null or empty.
        /// </summary>
        /// <param name="s">String instance</param>
        /// <returns>True or false</returns>
        public static bool NullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Indicates whether a string is not null or empty.
        /// </summary>
        /// <param name="s">String instance</param>
        /// <returns>True or false</returns>
        public static bool NotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Asserts an object if null that throws specified exception.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="exception">Instance of spcified exception </param>
        public static void NullThrowEx(this object instance, Exception exception)
        {
            if (instance == null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Asserts an object if null that throws argument exception.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="message">Argument exception message</param>
        public static void NullThrowArgumentEx(this object instance, string message)
        {
            if (instance == null)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Asserts an object if null that throws invalid operation exception.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="message">Invalid operation exception message</param>
        public static void NullThrowInvalidOpEx(this object instance, string message)
        {
            if (instance == null)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Asserts a string if null or empty that throws argument exception.
        /// </summary>
        /// <param name="str">String instance</param>
        /// <param name="message">Argument exception message</param>
        public static void NullOrEmptyThrowArgumentEx(this string str, string message)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Asserts an enumerable collection if null or empty that throws argument exception.
        /// </summary>
        /// <typeparam name="T">Collection instance type</typeparam>
        /// <param name="enumerable">Enumerable instance</param>
        /// <param name="message">Argument exception message</param>
        public static void NullOrEmptyThrowArgumentEx<T>(this IEnumerable<T> enumerable, string message)
        {
            if (enumerable.NullOrEmpty())
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Asserts a guid if empty that throws argument exception.
        /// </summary>
        /// <param name="guid">Guid instance</param>
        /// <param name="message">Argument exception message</param>
        public static void EmptyThrowArgumentEx(this Guid guid, string message)
        {
            if (guid.Empty())
            {
                throw new ArgumentException(message);
            }
        }
    }
}
