using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace branch_naming
{
    class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = MainAsync(args).GetAwaiter().GetResult();
            if (Debugger.IsAttached) Console.ReadKey();
            return exitCode;
        }

        internal static async Task<int> MainAsync(string[] args)
        {
            return 0;
        }
    }
}
