﻿
using MyerMomentUniversal.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MyerMomentUniversal.Model;
using Windows.Storage;
using JP.Utils.Data;
using Windows.Globalization;
using System.Reflection;
using Windows.UI;

// 有关“空白应用程序”模板的信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=234227

namespace MyerMomentUniversal
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
        ContinuationManager continuationManager;
#endif

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            App.Current.RequestedTheme = ApplicationTheme.Dark;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以打开特定的文件或显示搜索结果等操作时，
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            if (LocalSettingHelper.HasValue("AppLang") == false)
            {
                var lang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                if (lang.Contains("zh"))
                {
                    ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                }
                else ApplicationLanguages.PrimaryLanguageOverride = "en-US";

                LocalSettingHelper.AddValue("AppLang", ApplicationLanguages.PrimaryLanguageOverride);
            }
            else ApplicationLanguages.PrimaryLanguageOverride = LocalSettingHelper.GetValue("AppLang");

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                // TODO: 将此值更改为适合您的应用程序的缓存大小
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // 删除用于启动的旋转门导航。
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // 当未还原导航堆栈时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                if (!rootFrame.Navigate(typeof(NewMainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // 确保当前窗口处于活动状态
            Window.Current.Activate();

            MomentConfig.InitialMomentConfig();

            SetUpTitleBar(false);
        }

        public static void SetUpTitleBar(bool isGray)
        {
#if WINDOWS_APP
            var v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var allProperties = v.GetType().GetRuntimeProperties();
            var titleBar = allProperties.FirstOrDefault(x => x.Name == "TitleBar");
            if (titleBar == null) return;
            dynamic titleBarInst = titleBar.GetMethod.Invoke(v, null);
            if (!isGray)
            {
                titleBarInst.BackgroundColor = Colors.Black;
                titleBarInst.ForegroundColor = Colors.White;
                titleBarInst.ButtonBackgroundColor = Colors.Black;
                titleBarInst.ButtonForegroundColor = Colors.White;
                titleBarInst.ButtonHoverBackgroundColor = Colors.Black;
                titleBarInst.ButtonHoverForegroundColor = Colors.White;
                titleBarInst.ButtonPressedBackgroundColor = Colors.Black;

                titleBarInst.InactiveBackgroundColor = Colors.Black;
                titleBarInst.InactiveForegroundColor = Colors.White;
                titleBarInst.ButtonInactiveBackgroundColor = Colors.Black;
                titleBarInst.ButtonInactiveForegroundColor = Colors.White;
            }
            else
            {
                titleBarInst.BackgroundColor = (App.Current.Resources["MyerListGray"] as SolidColorBrush).Color;
                titleBarInst.ForegroundColor = Colors.Black;
                titleBarInst.ButtonBackgroundColor = (App.Current.Resources["MyerListGray"] as SolidColorBrush).Color;
                titleBarInst.ButtonForegroundColor = Colors.Black;
            }
#endif
        }



#if WINDOWS_APP
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {

            //args.Request.ApplicationCommands.Add(new SettingsCommand(
            //    "Setting", "Setting", (handler) =>
            //    {
            //        SettingFlyoutPage CustomSettingFlyout = new SettingFlyoutPage();
            //        CustomSettingFlyout.Show();
            //    }));

            //args.Request.ApplicationCommands.Add(new SettingsCommand(
            //    "About", "About", (handler) =>
            //    {
            //        AboutFlyout CustomSettingFlyout = new AboutFlyout();
            //        CustomSettingFlyout.Show();
            //    }));
        }

#endif

#if WINDOWS_PHONE_APP
        /// <summary>
        /// 启动应用程序后还原内容转换。
        /// </summary>
        /// <param name="sender">附加了处理程序的对象。</param>
        /// <param name="e">有关导航事件的详细信息。</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// 在将要挂起应用程序执行时调用。    将保存应用程序状态
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
        }
#if WINDOWS_PHONE_APP
        protected async override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            continuationManager = new ContinuationManager();

            Frame rootFrame = CreateRootFrame();
            await RestoreStatusAsync(e.PreviousExecutionState);

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(NewMainPage));
            }

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                Frame scenarioFrame = Window.Current.Content as Frame;
                if (scenarioFrame != null)
                {
                    // Call ContinuationManager to handle continuation activation
                    continuationManager.Continue(continuationEventArgs, scenarioFrame);
                }
            }

            Window.Current.Activate();
        }
#endif
        protected async override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            
            Frame rootFrame = CreateRootFrame();
            await RestoreStatusAsync(args.PreviousExecutionState);

            if (rootFrame.Content == null)
            {
                ShareOperation shareOperation = args.ShareOperation;
                if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await shareOperation.Data.GetStorageItemsAsync();
                    var firstItem = items.FirstOrDefault();
                    if (firstItem != null)
                    {
                        var path = firstItem.Path;
                        var fileToOpen = await StorageFile.GetFileFromPathAsync(path);
                        
                        PageNavigateData data = new PageNavigateData(fileToOpen, true);
                        rootFrame.Navigate(typeof(ImageHandlePage), data);

                        Window.Current.Content = rootFrame;
                        Window.Current.Activate();
                    }
                }
            }
        }


    }
}