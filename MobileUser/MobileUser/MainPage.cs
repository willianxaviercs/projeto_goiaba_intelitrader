using System;
using Xamarin.Forms;
using System.Net.Http;
using MobileUser;
using Newtonsoft.Json;
using System.Text;

public class MainPage : ContentPage
{
    Button CreateButton;
    Label ResponseField;
    Entry FirstName;
    Entry SurName;
    DatePicker AgePicker;

    HttpClient _client;
    static string _apiUrl;
    User user;

    public MainPage(HttpClient httpClient, string apiUrl)
    {
        _client = httpClient;
        _apiUrl = apiUrl;

        user = new User();

        this.Padding = new Thickness(20, 20, 20, 20);
        this.BackgroundColor = Color.FromHex("#eeeeee");

        StackLayout panel = new StackLayout
        {
            Spacing = 15,
        };

        panel.Children.Add(new Label {
            Text = "SIGN UP",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            TextColor = Color.FromHex("#4287f5"),
            FontSize = 20
            });

        panel.Children.Add(FirstName = new Entry {
            Placeholder = "Firstname",
        });

        panel.Children.Add(SurName = new Entry {
            Placeholder = "Surname",
        });

        panel.Children.Add(new Label {
            Text = "Birthdate:",
        });

        panel.Children.Add(AgePicker = new DatePicker
        {
            MaximumDate = DateTime.Now,
        });

        panel.Children.Add(CreateButton = new Button {
            Text = "CREATE USER",
            TextColor = Color.FromHex("#eeeeee"),
            BackgroundColor = Color.FromHex("#4287f5"),
        });

        // debug only
        panel.Children.Add(ResponseField = new Label {
            Text = "Response",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        });

        // STUDY: operator overload ?? 
        CreateButton.Clicked += OnCreateBtn;

        this.Content = panel;
    }

    // validate input fields
    bool InputIsValid()
    {
        bool result = true;

        // firstname required
        if (string.IsNullOrWhiteSpace(FirstName.Text))
        {
            result = false;
        }
        
        // surname not required
        if(SurName.Text != null)
        {
            if (string.IsNullOrWhiteSpace(SurName.Text))
            {
                SurName.Text = "";
            }
        }

        // age required greater than 0, less then 129
        int age = CalculateAge(DateTime.Now, AgePicker.Date);
        if (age == 0)
        {
            result = false;
        }

        return result;
    }

    int CalculateAge(DateTime now, DateTime birthdate)
    {
        int age = 0;

        // age relative to year
        int relativeAge = now.Year - birthdate.Year;

        if (relativeAge > 0)
        {
            age += (relativeAge - 1);
            
            // made anniversary
            if (now.Month > birthdate.Month)
            {
                age += 1;
            }
            else if (now.Month == birthdate.Month)
            {
                if (birthdate.Day >= now.Day)
                {
                    // made anniversary
                    age += 1;
                }
            }
        }
        return age;      
    }

    async void OnCreateBtn(object sender, EventArgs e)
    {
        // validade entrys?
        if (InputIsValid())
        {
            CreateUser();
        }
        else
        {
            // TODO: alert
            await DisplayAlert("Alert", "Invalid input!", "OK");
        }
    }

    async void CreateUser()
    {        
        // TODO: check for connection status
        // check if server is running
        // check response code
        // check for exceptions

        // STUDY: HttpRequestException, TaskCanceledException

        user.FirstName = FirstName.Text;
        user.SurName = SurName.Text;
        user.Age = CalculateAge(DateTime.Now, AgePicker.Date);

        StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_apiUrl, jsonContent);

        string responseStr = await response.Content.ReadAsStringAsync();

        ResponseField.Text = responseStr;

    }

}