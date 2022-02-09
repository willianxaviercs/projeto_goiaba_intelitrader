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

            string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2" : "https://localhost";
            string HttpsPort = "8001";
            string Route = "users";
            
            string ApiUrl = $"{BaseAddress}:{HttpsPort}/{Route}";

            HttpClient HttpClient = GetHttpClient();

            MainPage = new MainPage(HttpClient, ApiUrl);
        }

        public HttpClient GetHttpClient()
        {
            HttpClientHandler InsecureHandler = GetInsecureHandler();

            HttpClient Client = new HttpClient(InsecureHandler);

            Client.DefaultRequestHeaders.Add("Accept", "application/json");

            return Client;
        }
        
        // bypass used only for development purposes,
        // mobile dont permit self signed certificates
        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler Handler = new HttpClientHandler();
            Handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return Handler;
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
