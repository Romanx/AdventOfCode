using System.Collections.Generic;
using Spectre.Console;

namespace Shared
{
    public static class OutputExtensions
    {
        public static IOutput WriteTable(
            this IOutput output,
            IDictionary<string, string> dictionary)
        {
            output.WriteBlock(() =>
            {
                var table = new Table()
                {
                    ShowHeaders = false
                };
                table.AddColumns("", "");
                foreach (var (key, value) in dictionary)
                {
                    table.AddRow(key, value);
                }

                return table;
            });

            return output;
        }
    }
}
