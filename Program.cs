using Microsoft.TeamFoundation.SourceControl.WebApi;
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
            var opt = new OptionProvider(args);

            var tpc = opt.GetParam("SYSTEM_COLLECTIONURI", "tpc", "https://apollo.healthcare.siemens.com/tfs/IKM.TPC.Projects");
            var tp = opt.GetParam("SYSTEM_TEAMPROJECT", "tp", "tools");
            var repoName = opt.GetParam("BUILD_REPOSITRY_NAME", "repo", "x-juba-experiments");

            var tfs = new ConnectionProvider().Connect();
            var git = tfs.GetClient<GitHttpClient>();

            return 0;
        }
    }
}
