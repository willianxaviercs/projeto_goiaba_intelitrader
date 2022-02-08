using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Essentials;

namespace MobileUser
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string apiBaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2" : "https://localhost";
            string apiHttpsPort = "8001";
            string apiRoute = "users";
            
            string apiUrl = $"{apiBaseAddress}:{apiHttpsPort}/{apiRoute}";

            HttpClient httpClient = GetHttpClient();

            MainPage = new MainPage(httpClient, apiUrl);
        }

        public HttpClient GetHttpClient()
        {
            HttpClientHandler insecureHandler = GetInsecureHandler();

            HttpClient client = new HttpClient(insecureHandler);

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }
        
        // bypass used only for development purposes,
        // mobile dont permit self signed certificates
        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
