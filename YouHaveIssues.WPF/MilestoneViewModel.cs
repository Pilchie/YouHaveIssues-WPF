using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouHaveIssues.WPF
{
    public class MilestoneViewModel : ViewModel
    {
        private readonly Milestone milestone;
        private bool isIncluded = false;

        public MilestoneViewModel(Milestone milestone)
        {
            this.milestone = milestone;
        }

        public string Title => milestone.Title;

        public bool IsIncluded
        {
            get { return isIncluded; }
            set { SetProperty(ref isIncluded, value); }
        }
    }
}
