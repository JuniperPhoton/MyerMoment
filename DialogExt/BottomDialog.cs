using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;

namespace DialogExt
{
    public class BottomDialog :ContentControl
    {
        #region DependencyProperty

        public string TitleContent
        {
            get { return (string)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        public static DependencyProperty TitleContentProperty = DependencyProperty.Register("TitleContent",
            typeof(string), typeof(BottomDialog), new PropertyMetadata(null));


        public string ContentContent
        {
            get { return (string)GetValue(ContentContentProperty); }
            set { SetValue(ContentContentProperty, value); }
        }

        public static DependencyProperty ContentContentProperty = DependencyProperty.Register("ContentContent",
            typeof(string), typeof(BottomDialog), new PropertyMetadata(null));


        public SolidColorBrush DialogBackground
        {
            get { return (SolidColorBrush)GetValue(DialogBackgroundProperty); }
            set { SetValue(DialogBackgroundProperty, value); }
        }
        public static DependencyProperty DialogBackgroundProperty = DependencyProperty.Register("DialogBackground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public SolidColorBrush DialogForeground
        {
            get { return (SolidColorBrush)GetValue(DialogForegroundProperty); }
            set { SetValue(DialogForegroundProperty, value); }
        }

        public static DependencyProperty DialogForegroundProperty = DependencyProperty.Register("DialogForeground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public SolidColorBrush LeftButtonBackground
        {
            get { return (SolidColorBrush)GetValue(LeftButtonBackgroundProperty); }
            set { SetValue(LeftButtonBackgroundProperty, value); }
        }

        public static DependencyProperty LeftButtonBackgroundProperty = DependencyProperty.Register("LeftButtonBackground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public SolidColorBrush RightButtonBackground
        {
            get { return (SolidColorBrush)GetValue(RightButtonBackgroundProperty); }
            set { SetValue(RightButtonBackgroundProperty, value); }
        }

        public static DependencyProperty RightButtonBackgroundProperty = DependencyProperty.Register("RightButtonBackground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public SolidColorBrush LeftButtonForeground
        {
            get { return (SolidColorBrush)GetValue(LeftButtonForegroundProperty); }
            set { SetValue(LeftButtonForegroundProperty, value); }
        }

        public static DependencyProperty LeftButtonForegroundProperty = DependencyProperty.Register("LeftButtonForeground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public SolidColorBrush RightButtonForeground
        {
            get { return (SolidColorBrush)GetValue(RightButtonForegroundProperty); }
            set { SetValue(RightButtonForegroundProperty, value); }
        }

        public static DependencyProperty RightButtonForegroundProperty = DependencyProperty.Register("RightButtonForeground",
            typeof(SolidColorBrush), typeof(BottomDialog), new PropertyMetadata(null));

        public string LeftButtonContent
        {
            get { return (string)GetValue(LeftButtonContentProperty); }
            set { SetValue(LeftButtonContentProperty, value); }
        }

        public static DependencyProperty LeftButtonContentProperty = DependencyProperty.Register("LeftButtonContent",
            typeof(string), typeof(BottomDialog), new PropertyMetadata(null));

        public object RightButtonContent
        {
            get { return (string)GetValue(RightButtonContentProperty); }
            set { SetValue(RightButtonContentProperty, value); }
        }

        public static DependencyProperty RightButtonContentProperty = DependencyProperty.Register("RightButtonContent",
            typeof(object), typeof(BottomDialog), new PropertyMetadata(null));

        private bool isOpen { get; set; }

        #endregion

        private Page CurrentPage
        {
            get
            {
                return ((Window.Current.Content as Frame).Content) as Page;
            }
        }

        private CommandBar CurrentAppBar
        {
            get
            {
                return CurrentPage.BottomAppBar as CommandBar;
            }
        }

        private TextBlock titleTB;
        private TextBlock contentTB;
        private Button leftBtn;
        private Button rightBtn;
        private TextBlock leftBtnTB;
        private TextBlock rightBtnTB;
        private Grid rootGrid;
        private Border maskBorder;
        private Storyboard inStory;
        private Storyboard outStory;

        //Use popup to show the control
        private Popup CurrentPopup;

        public  EventHandler<DialogEventArgs> OnLeftBtnClick { get; set; }
        public  EventHandler<DialogEventArgs> OnRightBtnClick { get; set; }

        //Provide the method to solve getting Storyboard before OnApplyTemplate() execute problem.
        System.Threading.Tasks.TaskCompletionSource<int> tcs;

        public BottomDialog()
        {
            DefaultStyleKey=(typeof(BottomDialog));
            
            if(!DesignMode.DesignModeEnabled)
            {
                tcs = new TaskCompletionSource<int>();
                
                if (CurrentPopup == null)
                {
                    CurrentPopup = new Popup();
                    CurrentPopup.VerticalAlignment = VerticalAlignment.Stretch;
                    this.Height = (Window.Current.Content as Frame).Height;
                    this.Width = (Window.Current.Content as Frame).Width;
                    CurrentPopup.Child = this;
                    CurrentPopup.IsOpen = true;
                }
            }
        }

        public BottomDialog(EventHandler<DialogEventArgs> leftAct,EventHandler<DialogEventArgs> rightAct) :this()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                OnLeftBtnClick = leftAct;
                OnRightBtnClick = rightAct;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignMode.DesignModeEnabled)
            {
                InitialPane();
            }
        }

        private void InitialPane()
        {
            rootGrid = GetTemplateChild("RootGrid") as Grid;
            titleTB = GetTemplateChild("TitleTB") as TextBlock;
            contentTB = GetTemplateChild("ContentTB") as TextBlock;
            leftBtn = GetTemplateChild("LeftBtn") as Button;
            rightBtn = GetTemplateChild("RightBtn") as Button;
            leftBtnTB = GetTemplateChild("LeftBtnTB") as TextBlock;
            rightBtnTB = GetTemplateChild("RightBtnTB") as TextBlock;
            maskBorder = GetTemplateChild("MaskBorder") as Border;

            maskBorder.Tapped += ((sendert, et) =>
              {
                  if (!isOpen)
                  {
                      return;
                  }
                  Hide();
              });

            inStory = rootGrid.Resources["InStory"] as Storyboard;
            outStory = rootGrid.Resources["OutStory"] as Storyboard;

            tcs.SetResult(1);

            rootGrid.Width = Window.Current.Bounds.Width;
            rootGrid.Height = Window.Current.Bounds.Height;

            leftBtn.Click += LeftBtn_Click;
            rightBtn.Click += RightBtn_Click;
        }


        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            OnRightBtnClick.Invoke(this, new DialogEventArgs("Right"));
        }

        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            OnLeftBtnClick.Invoke(this, new DialogEventArgs("Left"));
        }

        public async void Show()
        {
            //Hide appbar if it's visible.
            if(CurrentAppBar!=null)
            {
                CurrentAppBar.IsSticky = false;
                CurrentAppBar.Visibility = Visibility.Collapsed;
            }
           
            if(isOpen)
            {
                return;
            }
            await tcs.Task;
            isOpen = true;
            maskBorder.Visibility = Visibility.Visible;
            inStory.Begin();

            //Handle Backpressed buttons on Windows Phone.
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if(isOpen)
            {
                e.Handled = true;
                Hide();
            }
        }

        public void Hide()
        {
            if (CurrentAppBar != null)
            {
                CurrentAppBar.IsSticky = true;
                CurrentAppBar.Visibility = Visibility.Visible;
            }

            isOpen = false;
            outStory.Begin();
            maskBorder.Visibility = Visibility.Collapsed;

            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
    }
}
