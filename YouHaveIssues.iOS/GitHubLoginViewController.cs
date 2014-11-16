using Octokit;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace YouHaveIssues
{
    partial class GitHubLoginViewController : UIViewController
    {
        public GitHubLoginViewController(IntPtr handle)
            : base(handle)
        {
        }

        private void HandleLoadStarted(object sender, EventArgs e)
        {
            if (webView.Request.Url.AbsoluteString.StartsWith("http://youhaveissues.azurewebsites.net"))
            {
                var parameters = webView.Request.Url.Query.Split('&');
                foreach (var param in parameters)
                {
                    var parts = param.Split('=');
                    if (parts[0] == "code")
                    {
                        var code = parts[1];
                    }
                }
                webView.StopLoading();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var config = GitHubConfig.Load();
            GitHubClient client = new GitHubClient(new ProductHeaderValue("YouHaveIssues"));
            var loginRequest = new OauthLoginRequest(config.ClientID);
            var loginUri = client.Oauth.GetGitHubLoginUrl(loginRequest);
            var loginUrlString = loginUri.OriginalString;
            var nsUrl = new NSUrl(loginUrlString);
            var nsUrlRequest = new NSUrlRequest(nsUrl);

            webView.LoadStarted += HandleLoadStarted;
            webView.LoadRequest(nsUrlRequest);
        }
    }
}
