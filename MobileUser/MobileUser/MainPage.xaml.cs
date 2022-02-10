using System;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using MobileUser;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MobileUser
{
    public partial class MainPage : ContentPage
    {
        HttpClient HttpClient;
        static string ApiUrl;
        User User;

        public MainPage(HttpClient httpClient, string apiUrl)
        {
            InitializeComponent();

            HttpClient = httpClient;
            ApiUrl = apiUrl;

            User = new User();
        }
        
        async void NameTextChanged(object sender, TextChangedEventArgs e)
        {
            var NewText = e.NewTextValue;

            if (string.IsNullOrEmpty(NewText))
            {
                CreateButton.IsEnabled = false;
            }
            else
            {
                if (StringIsNumeric(AgeText.Text))
                {
                    CreateButton.IsEnabled = true;
                }
            }
        }

        void AgeTextChanged(object sender, TextChangedEventArgs e)
        {   
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                CreateButton.IsEnabled = false;
                return;
            }

            var OldText = e.OldTextValue;
            var NewText = e.NewTextValue;

            var NumericText = StringIsNumeric(NewText);

            if (!NumericText)
            {
                Entry Entry = (Entry)sender;
                Entry.Text = OldText;
            }
            else if (NumericText)
            {
                int Age = Convert.ToInt32(NewText);
                if ( Age > 128 || Age < 1)
                {
                    Entry Entry = (Entry)sender;
                    Entry.Text = OldText;
                }
                if (!string.IsNullOrEmpty(FirstNameText.Text))
                {
                    CreateButton.IsEnabled = true;
                }
            }
        }
        
         bool StringIsNumeric(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }
        
        void CreateButtonClicked(object sender, EventArgs e)
        {  
            CreateUser();
        }

        async void CreateUser()
        {
            // TODO: check for internet status
            // set and faster timeout to better user feedback, or
            // check for server availability on app startup

            User User = new User();

            User.FirstName = FirstNameText.Text;
            User.SurName = SurNameText.Text;
            User.Age = Convert.ToInt32(AgeText.Text);

            StringContent SerializedData = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");

            try
            {
                var Response = await HttpClient.PostAsync(ApiUrl, SerializedData);
            
                if (Response.IsSuccessStatusCode)
                {
                    string ContentStr = await Response.Content.ReadAsStringAsync();

                    // NOTE: when deserializing the response, the default handling of DateTime is set to "Local",
                    // which relies on the machine to convert to a specific timezone.
                    // If the machine is 'wrong' about the timezone offset, the app shows the wrong time.
                    // Maybe try and get the correct timezone offset in a NTP server?

                    User ResponseUser = JsonConvert.DeserializeObject<User>(ContentStr, new JsonSerializerSettings {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                        });

                    Application.Current.MainPage = new DisplayPage(ResponseUser);
                }
                else
                {
                    await DisplayAlert("Debug", "Request failure", "OK");
                }
            }
            catch (HttpRequestException)
            {
                Debug.Write("HttpRequestException");
                await DisplayAlert("Debug", "Http request failure, try again", "OK");
            }
            catch (TaskCanceledException)
            {
                Debug.Write("TaskCancelledException");
                await DisplayAlert("Debug", "Task timeout, try again", "OK");
            }
        }
    }
}
