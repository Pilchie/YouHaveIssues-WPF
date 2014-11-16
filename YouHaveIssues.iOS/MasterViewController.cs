﻿using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Octokit;

namespace YouHaveIssues
{
    public partial class MasterViewController : UITableViewController
    {
        DataSource dataSource;

        public MasterViewController(IntPtr handle)
            : base(handle)
        {
            Title = NSBundle.MainBundle.LocalizedString("Accounts", "Accounts");
			
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                PreferredContentSize = new SizeF(320f, 600f);
                ClearsSelectionOnViewWillAppear = false;
            }
			
            // Custom initialization
        }

        public DetailViewController DetailViewController
        {
            get;
            set;
        }

        void AddNewItem(object sender, EventArgs args)
        {
            PerformSegue("showGitHubLogin", this);

        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.LeftBarButtonItem = EditButtonItem;

            var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
            NavigationItem.RightBarButtonItem = addButton;

            TableView.Source = dataSource = new DataSource(this);
        }

        class DataSource : UITableViewSource
        {
            static readonly NSString CellIdentifier = new NSString("Cell");
            readonly List<GitHubClient> clients = new List<GitHubClient>();
            readonly MasterViewController controller;

            public DataSource(MasterViewController controller)
            {
                this.controller = controller;
            }

            public IList<GitHubClient> Clients
            {
                get { return clients; }
            }

            // Customize the number of sections in the table view.
            public override int NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                return clients.Count;
            }

            // Customize the appearance of table view cells.
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (UITableViewCell)tableView.DequeueReusableCell(CellIdentifier, indexPath);

                clients[indexPath.Row].User.Current().ContinueWith(prev =>
                    {
                        InvokeOnMainThread(() => cell.TextLabel.Text = prev.Result.Login + " (" + prev.Result.Name + ")");
                    });

                return cell;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                // Return false if you do not want the specified item to be editable.
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                if (editingStyle == UITableViewCellEditingStyle.Delete)
                {
                    // Delete the row from the data source.
                    clients.RemoveAt(indexPath.Row);
                    controller.TableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                }
                else if (editingStyle == UITableViewCellEditingStyle.Insert)
                {
                    // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
                }
            }

            /*
			// Override to support rearranging the table view.
			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
			{
			}
			*/

            /*
			// Override to support conditional rearranging of the table view.
			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the item to be re-orderable.
				return true;
			}
			*/

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                    controller.DetailViewController.SetDetailItem(clients[indexPath.Row]);
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "showDetail")
            {
                var indexPath = TableView.IndexPathForSelectedRow;
                var item = dataSource.Clients[indexPath.Row];

                ((DetailViewController)segue.DestinationViewController).SetDetailItem(item);
            }
            else if (segue.Identifier == "showGitHubLogin")
            {
                ((GitHubLoginViewController)segue.DestinationViewController).AccountAuthenticated += (o, e) =>
                {
                    dataSource.Clients.Insert(0, e.Client);
                    using (var indexPath = NSIndexPath.FromRowSection(0, 0))
                    {
                        TableView.InsertRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
                    }
                };
            }
        }
    }
}

