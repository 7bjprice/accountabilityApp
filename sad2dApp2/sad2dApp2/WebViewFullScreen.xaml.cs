using System;
using System.Diagnostics;

namespace sad2dApp2
{
    public partial class WebViewFullScreen : ContentPage
    {
        public WebViewFullScreen()
        {
            InitializeComponent();
            LoadLocalHtml();
            UpdateScoreInWebViewAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (GotchiService.Current != null)
            {
                UpdateScoreInWebViewAsync();
            }
        }

        private async void UpdateScoreInWebViewAsync()
        {
            if (myWebView != null)
            {
                string js = $"updateCurrentScore({(
                    GotchiService.Current.Happiness + GotchiService.Current.Wellness) / 2
                    });";
                await myWebView.EvaluateJavaScriptAsync(js);
            }
        }

        private async void LoadLocalHtml()
        {
            Debug.WriteLine("LoadLocalHtml started.");

            try
            {
                Debug.WriteLine("Opening local HTML file: index.html");
#if ANDROID
    // Load android.html
    using var stream = await FileSystem.OpenAppPackageFileAsync("android.html");
#else
                // Load windows.html
                using var stream = await FileSystem.OpenAppPackageFileAsync("windows.html");

#endif
                Debug.WriteLine("File stream opened successfully.");

                using var reader = new StreamReader(stream);
                string htmlContent = await reader.ReadToEndAsync();
                Debug.WriteLine("HTML content read successfully. Length: " + htmlContent.Length);

                Debug.WriteLine("Setting WebView source.");
                myWebView.Source = new HtmlWebViewSource
                {
                    Html = htmlContent
                };
                Debug.WriteLine("WebView source set successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in LoadLocalHtml: " + ex);
            }

            Debug.WriteLine("LoadLocalHtml finished.");
        }

        private async void OnMinimizeClicked(object sender, EventArgs e)
        {
            try
            {
                // If page was shown modally, pop it
                await Navigation.PopModalAsync(animated: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PopModalAsync failed: {ex}. Falling back to Shell navigation.");
                // Fallback: use Shell navigation if you originally navigated via Shell routes
                await Shell.Current.GoToAsync("///MainPage");
            }
        }
    }
}
