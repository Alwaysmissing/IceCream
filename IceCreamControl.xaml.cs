using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace IceCream.View
{
    /// <summary>
    /// IceCreamControl.xaml 的交互逻辑
    /// </summary>
    public partial class IceCreamControl : UserControl
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(string), typeof(IceCreamControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public IceCreamControl()
        {
            InitializeComponent();
        }
    }
}
