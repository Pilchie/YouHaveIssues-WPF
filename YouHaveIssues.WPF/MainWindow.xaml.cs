using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YouHaveIssues.WPF.Properties;

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
        public IReadOnlyList<Repository> Repositories
        {
            get { return (IReadOnlyList<Repository>)GetValue(RepositoriesProperty); }
            set { SetValue(RepositoriesProperty, value); }
        }
        public static readonly DependencyProperty RepositoriesProperty =
            DependencyProperty.Register("Repositories", typeof(IReadOnlyList<Repository>), typeof(MainWindow));

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
            Issues = await githubClient.Issue.GetForRepository(SelectedRepository.Owner.Login, SelectedRepository.Name);
        }

        public IReadOnlyList<Issue> Issues
        {
            get { return (IReadOnlyList<Issue>)GetValue(IssuesProperty); }
            set { SetValue(IssuesProperty, value); }
        }
        public static readonly DependencyProperty IssuesProperty =
            DependencyProperty.Register("Issues", typeof(IReadOnlyList<Issue>), typeof(MainWindow));
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            token.Text = Settings.Default.AuthenticationToken;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.AuthenticationToken = token.Text;
            Settings.Default.SelectedRepository = SelectedRepository.FullName;
            Settings.Default.Save();
        }

        private async void OnAuthenticateClicked(object sender, RoutedEventArgs e)
        {
            githubClient.Credentials = new Credentials(token.Text);
            Repositories = await githubClient.Repository.GetAllForCurrent();
            SelectedRepository = Repositories.FirstOrDefault(r => r.FullName == Settings.Default.SelectedRepository);
        }
    }
}
