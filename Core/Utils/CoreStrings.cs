using JetBrains.Annotations;
using System.Globalization;
using System.Runtime.CompilerServices;

// copied from https://github.com/dotnet/efcore/blob/main/src/EFCore/Properties/CoreStrings.resx
[assembly: InternalsVisibleTo("Pandora.Test")]
namespace Pandora.Core.Utils
{
    internal static class CoreStrings
    {
        /// <summary>
        /// The property '{property}' of the argument '{argument}' cannot be null.
        /// </summary>
        public static string ArgumentPropertyNull([CanBeNull] string property, [CanBeNull] string argument)
        {
            return string.Format(CultureInfo.CurrentCulture, "The property '{0}' of the argument '{1}' cannot be null.", property, argument);
        }

        /// <summary>
        /// The property '{property}' of the argument '{argument}' cannot be null.
        /// </summary>
        public static string ArgumentPropertyEmpty([CanBeNull] string property, [CanBeNull] string argument)
        {
            return string.Format(CultureInfo.CurrentCulture, "The property '{0}' of the argument '{1}' cannot be empty.", property, argument);
        }

        /// <summary>
        /// The string argument '{argumentName}' cannot be empty.
        /// </summary>
        public static string ArgumentIsEmpty([CanBeNull] string argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, "The string argument '{0}' cannot be empty.", argumentName);
        }

        /// <summary>
        /// The number argument '{argumentName}' should be greater than zero.
        /// </summary>
        public static string ArgumentIsLowerOrEqualZero([CanBeNull] string argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, "The number argument '{0}' should be greater than zero.", argumentName);
        }

        /// <summary>
        /// If {propertyName} {message}, contact the {teamName} team.
        /// </summary>
        public static string StatusReason([NotNull] string propertyName, [NotNull] string message, [NotNull] string teamName)
        {
            return string.Format(CultureInfo.CurrentCulture, "If {0} {1}, contact the {2} team.", propertyName, message, teamName);
        }

        /// <summary>
        /// The field '{propertyName}' is {message}
        /// </summary>
        public static string StatusTitle([NotNull] string propertyName, [NotNull] string message)
        {
            return string.Format(CultureInfo.CurrentCulture, "The field '{0}' is {1}", propertyName, message);
        }
    }
}
