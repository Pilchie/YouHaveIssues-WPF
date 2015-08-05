using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace YouHaveIssues.WPF
{
    public class IssueViewModel : ViewModel
    {
        private static readonly Dictionary<string, string> s_nameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
        };
        private readonly Issue _issue;

        public IssueViewModel(Issue i)
        {
            _issue = i;
        }

        public int Number => _issue.Number;
        public string Title => _issue.Title;
        public string Milestone => _issue.Milestone?.Title;
        public string HtmlUrl => _issue.HtmlUrl.OriginalString;

        public string AssigneeLogin
        {
            get
            {
                string mapping = null;
                if (s_nameMap.TryGetValue(_issue.Assignee.Login, out mapping))
                {
                    return mapping;
                }

                return _issue.Assignee.Login;
            }
        }
    }
}
