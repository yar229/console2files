using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console2files
{
    internal static class Extensions
    {
        public static string HackDecode(this string input)
        {
            byte[] bytes = Encoding.GetEncoding(28591)
                .GetBytes(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
