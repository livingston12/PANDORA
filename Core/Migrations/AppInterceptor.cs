using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pandora.Core.Migrations
{
    [ExcludeFromCodeCoverage]
    public class AppInterceptor : DbCommandInterceptor
    {
        private static readonly Regex tableAliasRegex =
            new Regex(@"(?<tableAlias>FROM +(\[.*\]\.)?(\[.*\]) AS (\[.*\])(?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);

        private static readonly Regex joinAliasRegex =
            new Regex(@"(?<tableAlias>[LEFT|INNER] JOIN +(\[.*\]\.)?(\[.*\]) AS (\[.*\])(?! WITH \(NOLOCK\))) ON",
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            OverriedCommand(command);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            OverriedCommand(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        private static void OverriedCommand(DbCommand command)
        {
            if (command != null && !command.CommandText.Contains("WITH (NOLOCK)"))
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                command.CommandText =
                    tableAliasRegex.Replace(command.CommandText,
                        "${tableAlias} WITH (NOLOCK)");
                command.CommandText =
                    joinAliasRegex.Replace(command.CommandText,
                        "${tableAlias} WITH (NOLOCK) ON");
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            }
        }
    }
}