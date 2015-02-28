using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private IReadOnlyList<Repository> repositories;
        Repository selectedRepository;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            token.Text = Settings.Default.AuthenticationToken;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.AuthenticationToken = token.Text;
            Settings.Default.SelectedRepository = (string)repositoriesCombo.SelectedItem;
            Settings.Default.Save();
        }

        private async void OnAuthenticateClicked(object sender, RoutedEventArgs e)
        {
            githubClient.Credentials = new Credentials(token.Text);
            repositories = await githubClient.Repository.GetAllForCurrent();
            foreach (var repo in repositories.OrderBy(r => r.FullName))
            {
                repositoriesCombo.Items.Add(repo.FullName);
                if (repo.FullName == Settings.Default.SelectedRepository)
                {
                    selectedRepository = repo;
                    repositoriesCombo.SelectedItem = repo.FullName;
                }
            }
        }

        private async void repositoriesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var issues = await githubClient.Issue.GetForRepository(
                selectedRepository.Owner.Login, selectedRepository.Name);
            dataGrid.ItemsSource = issues;
        }
    }
}
