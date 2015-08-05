using System.Diagnostics;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YouHaveIssues.WPF.Properties;
using System.Threading.Tasks;

namespace YouHaveIssues.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GitHubConfig config = GitHubConfig.Load();
        private readonly GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("YouHaveIssues"));

        #region DPs

        public string Token
        {
            get { return (string)GetValue(TokenProperty); }
            set { SetValue(TokenProperty, value); }
        }
        public static readonly DependencyProperty TokenProperty =
            DependencyProperty.Register("Token", typeof(string), typeof(MainWindow));

        public IReadOnlyList<Repository> Repositories
        {
            get { return (IReadOnlyList<Repository>)GetValue(RepositoriesProperty); }
            set { SetValue(RepositoriesProperty, value); }
        }
        public static readonly DependencyProperty RepositoriesProperty =
            DependencyProperty.Register("Repositories", typeof(IReadOnlyList<Repository>), typeof(MainWindow));

        public IReadOnlyList<MilestoneViewModel> Milestones
        {
            get { return (IReadOnlyList<MilestoneViewModel>)GetValue(MilestonesProperty); }
            set { SetValue(MilestonesProperty, value); }
        }

        public static readonly DependencyProperty MilestonesProperty =
            DependencyProperty.Register("Milestones", typeof(IReadOnlyList<MilestoneViewModel>), typeof(MainWindow));

        public Repository SelectedRepository
        {
            get { return (Repository)GetValue(SelectedRepositoryProperty); }
            set { SetValue(SelectedRepositoryProperty, value); }
        }

        public static readonly DependencyProperty SelectedRepositoryProperty =
            DependencyProperty.Register("SelectedRepository", typeof(Repository), typeof(MainWindow), new PropertyMetadata(
                (d, e) =>
                {
                    var mw = d as MainWindow;
                    if (mw == null) return;
                    mw.SelectedRepositoryChanged();
                }));

        public async void SelectedRepositoryChanged()
        {
            using (new WaitCursor(this))
            {
                var milestones = from m in await githubClient.Issue.Milestone.GetForRepository(
                                       SelectedRepository.Owner.Login,
                                       SelectedRepository.Name)
                                 select new MilestoneViewModel(m);
                this.Milestones = milestones.ToList();
            }

            await RefreshIssues();
        }

        private async Task RefreshIssues()
        {
            using (new WaitCursor(this))
            {
                var issues = from i in await githubClient.Issue.GetForRepository(
                                       SelectedRepository.Owner.Login,
                                       SelectedRepository.Name,
                                       new RepositoryIssueRequest
                                       {
                                           State = ItemState.Open,
                                       })
                             where i.PullRequest == null
                             where i.Labels.Any(l => l.Name == "Area-IDE")
                             select new IssueViewModel(i);

                if (Milestones.Any(m => m.IsIncluded))
                {
                    this.Issues = issues.Where(i => Milestones.Any(m => m.IsIncluded && m.Title == i.Milestone)).ToList();
                }
                else
                {
                    this.Issues = issues.Where(i => i.Milestone == null).ToList();
                }
            }
        }

        public IReadOnlyList<IssueViewModel> Issues
        {
            get { return (IReadOnlyList<IssueViewModel>)GetValue(IssuesProperty); }
            set { SetValue(IssuesProperty, value); }
        }
        public static readonly DependencyProperty IssuesProperty =
            DependencyProperty.Register("Issues", typeof(IReadOnlyList<IssueViewModel>), typeof(MainWindow));
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            Loaded += MainWindow_Loaded;
            Token = Settings.Default.AuthenticationToken;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Token))
            {
                await AuthenticateAsync();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Token != null) Settings.Default.AuthenticationToken = Token;
            if (SelectedRepository != null) Settings.Default.SelectedRepository = SelectedRepository.FullName;
            Settings.Default.Save();
        }

        private async void OnAuthenticateClicked(object sender, RoutedEventArgs e)
        {
            await AuthenticateAsync();
        }

        private async Task AuthenticateAsync()
        {
            using (new WaitCursor(this))
            {
                githubClient.Credentials = new Credentials(Token);
                Repositories = await githubClient.Repository.GetAllForCurrent();
            }

            SelectedRepository = Repositories.FirstOrDefault(r => r.FullName == Settings.Default.SelectedRepository);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void OnClickRefresh(object sender, RoutedEventArgs e)
        {
            await RefreshIssues();
        }
    }
}
