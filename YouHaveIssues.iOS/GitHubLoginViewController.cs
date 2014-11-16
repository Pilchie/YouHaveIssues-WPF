using Octokit;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;

namespace YouHaveIssues
{
    partial class GitHubLoginViewController : UIViewController
    {
        private readonly GitHubConfig config = GitHubConfig.Load();
        private readonly GitHubClient unauthenticatedClient = new GitHubClient(new ProductHeaderValue("YouHaveIssues"));

        public event EventHandler<GitHubClientEventArgs> AccountAuthenticated = delegate { };

        public GitHubLoginViewController(IntPtr handle)
            : base(handle)
        {
        }

        private async void HandleLoadStarted(object sender, EventArgs e)
        {
            if (webView.Request.Url.AbsoluteString.StartsWith("http://youhaveissues.azurewebsites.net"))
            {
                webView.StopLoading();

                var parameters = webView.Request.Url.Query.Split('&');
                Task<OauthToken> authTokenTask = null;
                foreach (var param in parameters)
                {
                    var parts = param.Split('=');
                    if (parts[0] == "code")
                    {
                        var code = parts[1];
                        authTokenTask = unauthenticatedClient.Oauth.CreateAccessToken(
                            new OauthTokenRequest(config.ClientID, config.ClientSecret, code));
                        break;
                    }
                }

                if (authTokenTask != null)
                {
                    var token = await authTokenTask;
                    var client = new GitHubClient(new ProductHeaderValue("YouHaveIssues"));
                    client.Credentials = new Credentials(token.AccessToken);
                    DismissViewController(animated: true, completionHandler: null);
                    AccountAuthenticated(this, new GitHubClientEventArgs(client));
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var loginRequest = new OauthLoginRequest(config.ClientID);
            var loginUri = unauthenticatedClient.Oauth.GetGitHubLoginUrl(loginRequest);
            var nsUrlRequest = new NSUrlRequest(new NSUrl(loginUri.OriginalString));

            webView.LoadStarted += HandleLoadStarted;
            webView.LoadRequest(nsUrlRequest);
        }
    }
}
