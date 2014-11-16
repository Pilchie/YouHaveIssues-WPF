// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace YouHaveIssues
{
	[Register ("GitHubLoginViewController")]
	partial class GitHubLoginViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView GitHubLoginView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView webView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (GitHubLoginView != null) {
				GitHubLoginView.Dispose ();
				GitHubLoginView = null;
			}
			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
