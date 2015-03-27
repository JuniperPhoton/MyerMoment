using ChaoFunctionRT;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace MyerMomentUniversal
{
    public sealed partial class SettingFlyoutPage : SettingsFlyout
    {

        private bool iscomComDirty = false;
        private bool isposComDirty = false;
        public SettingFlyoutPage()
        {
            this.InitializeComponent();
        }


        private void positionCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isposComDirty == false)
            {
                isposComDirty = true;
                return;
            }
            var combox = sender as ComboBox;
            if (combox != null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("Position", selectedIndex.ToString());
            }
        }
    }
}
