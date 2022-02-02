using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace branch_naming
{
    class Program
    {
        public static GitHttpClient git { get; private set; }

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
            var repoName = opt.GetParam("BUILD_REPOSITRY_NAME", "repo", "x-juba-experiment");

            var tfs = new ConnectionProvider(tpc).Connect();
            git = tfs.GetClient<GitHttpClient>();

            var repo = await git.GetRepositoryAsync(project: tp, repoName);
            // GetRefsAsync() cannot digest a "refs/" suffix :-/
            var heads = await git.GetRefsAsync(repositoryId: repo.Id, repo.DefaultBranch.Substring("refs/".Length));
            var mainBranch = heads.Single();

            await CreateBranch("demo/devs/max", mainBranch.ObjectId, repo.Id);
            await CreateBranch("demo/devs/julia", mainBranch.ObjectId, repo.Id);
            await CreateBranch("demo/wip/123456-fancy-ui", mainBranch.ObjectId, repo.Id);
            await CreateBranch("demo/wip/456789-smart-search", mainBranch.ObjectId, repo.Id);
            var qr2201 = await CreateBranch("demo/release/qr2201/main", mainBranch.ObjectId, repo.Id);
            await CreateBranch("demo/release/qr2201/hf/159847-big-fat-issue", qr2201.ObjectId, repo.Id);
            await CreateBranch("demo/release/qr2201/hf/951623-performance-topic", qr2201.ObjectId, repo.Id);
            var qr2204 = await CreateBranch("demo/release/qr2204/main", mainBranch.ObjectId, repo.Id);
            await CreateBranch("demo/release/qr2204/hf/584679-sql-injection-vulnerability", qr2204.ObjectId, repo.Id);
            await CreateBranch("demo/release/qr2204/hf/134679-wrong-logo", qr2204.ObjectId, repo.Id);

            heads = await git.GetRefsAsync(repositoryId: repo.Id, $"heads");
            heads.ForEach(h => Console.WriteLine(h.Name));
            // manual command to remove these 'demo' branches:
            // git branch -r | select-string -pattern "demo" |foreach { git push origin --delete "$($($_ -replace ""origin/"", """").trim())" }

            return 0;
        }

        static async Task<GitRef> CreateBranch(string branchName, string sourceBranchId, Guid repoId)
        {
            // how to create a new branch: https://oshamrai.wordpress.com/category/visual-studio/azure-devops-services/page/2/
            await git.UpdateRefsAsync(new[] {new GitRefUpdate
            {
                Name = $"refs/heads/{branchName}",
                OldObjectId = new StringBuilder().Append('0', 40).ToString(),
                /// it would be possible to use a commit id, but especially in case of
                /// creating a new branch it is not sure, that the source branch has
                /// any commits yet
                NewObjectId = sourceBranchId
            } }, repoId);
            var heads = await git.GetRefsAsync(repositoryId: repoId, $"heads/{branchName}");
            return heads.Single();
        }
    }
}
