using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Sunset
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SystemNavigationManager CurrentView;
        public MainPage()
        {
            this.InitializeComponent();
            MainWebView.Source = new Uri("http://m.youku.com/");
            MainWebView.NewWindowRequested += MainWebView_NewWindowRequested;
            MainWebView.DOMContentLoaded += MainWebView_DOMContentLoaded;
            MainWebView.UnsupportedUriSchemeIdentified += MainWebView_UnsupportedUriSchemeIdentified;
            MainWebView.ContainsFullScreenElementChanged += MainWebView_ContainsFullScreenElementChanged;
            CurrentView = SystemNavigationManager.GetForCurrentView();
            CurrentView.BackRequested += CurrentView_BackRequested;
        }

        private void MainWebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            args.Handled = true;
        }

        private void MainWebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            string FixScript = @"window.login = function login(callback){window.location.href = 'https://account.youku.com?callback='+ encodeURIComponent(window.location.href);}; console.log(1);";
            MainWebView.InvokeScriptAsync("eval", new string[] { FixScript });
        }

        private void MainWebView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (sender.ContainsFullScreenElement)
            {
                applicationView.TryEnterFullScreenMode();
            }
            else if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
            }
        }


        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainWebView.CanGoBack)
            {
                MainWebView.GoBack();
                e.Handled = true;
                return;
            }
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void MainWebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            MainWebView.Source = args.Uri;
            args.Handled = true;
        }
    }
}
