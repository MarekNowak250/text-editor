using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextEditor.Controls
{
    /// <summary>
    /// Interaction logic for SideScroll.xaml
    /// </summary>
    public partial class SideScroll : UserControl
    {
        internal readonly ScrollBarDrawer Drawer;
        public event Action<int> Scrolled;

        private bool _scrolling = false;

        internal SideScroll(IScrollBarDrawer drawer)
        {
            InitializeComponent();
            drawer.Init(SideScrollCanvas);
        }

        private void SideScrollCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _scrolling = false;
        }

        private void SideScrollCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_scrolling)
                return;

            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            Scrolled?.Invoke((int)Math.Floor(mousePosition.Y / parent.ActualHeight * 100));
        }

        private void SideScrollCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _scrolling = false;
        }

        private void SideScrollCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _scrolling = true;
            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            Scrolled?.Invoke((int)Math.Floor(mousePosition.Y / parent.ActualHeight * 100));
        }
    }
}
