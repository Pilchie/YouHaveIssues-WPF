using Octokit;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;

namespace YouHaveIssues
{
	class GitHubClientEventArgs : EventArgs
	{
        public GitHubClient Client { get; private set; }

        public GitHubClientEventArgs(GitHubClient client)
        {
            this.Client = client;
        }
	}
}
