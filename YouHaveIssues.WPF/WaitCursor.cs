using System;
using System.Windows;
using System.Windows.Input;

namespace YouHaveIssues.WPF
{
    internal class WaitCursor : IDisposable
    {
        private readonly FrameworkElement _owner;
        private readonly Cursor _cursor;

        public WaitCursor(FrameworkElement fe)
        {
            _owner = fe;
            _cursor = _owner.Cursor;
            _owner.Cursor = Cursors.AppStarting;
        }

        public void Dispose()
        {
            _owner.Cursor = _cursor;
        }
    }
}