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

    HttpClient HttpClient;
    static string ApiUrl;
    User User;

    public MainPage(HttpClient httpClient, string apiUrl)
    {
        HttpClient = httpClient;
        ApiUrl = apiUrl;

        User = new User();

        this.Padding = new Thickness(20, 20, 20, 20);
        this.BackgroundColor = Color.FromHex("#eeeeee");

        StackLayout Panel = new StackLayout
        {
            Spacing = 15,
        };

        Panel.Children.Add(new Label {
            Text = "SIGN UP",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            TextColor = Color.FromHex("#4287f5"),
            FontSize = 20
            });

        Panel.Children.Add(FirstName = new Entry {
            Placeholder = "Firstname",
        });

        Panel.Children.Add(SurName = new Entry {
            Placeholder = "Surname",
        });

        Panel.Children.Add(new Label {
            Text = "Birthdate:",
        });

        Panel.Children.Add(AgePicker = new DatePicker
        {
            MaximumDate = DateTime.Now,
        });

        Panel.Children.Add(CreateButton = new Button {
            Text = "CREATE USER",
            TextColor = Color.FromHex("#eeeeee"),
            BackgroundColor = Color.FromHex("#4287f5"),
        });

        // debug only
        Panel.Children.Add(ResponseField = new Label {
            Text = "Response",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        });

        // STUDY: operator overload ?? 
        CreateButton.Clicked += OnCreateBtn;

        this.Content = Panel;
    }

    // validate input fields
    bool InputIsValid()
    {
        bool Result = true;

        // firstname required
        if (string.IsNullOrWhiteSpace(FirstName.Text))
        {
            Result = false;
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
        int Age = CalculateAge(DateTime.Now, AgePicker.Date);
        if (Age == 0)
        {
            Result = false;
        }

        return Result;
    }

    int CalculateAge(DateTime now, DateTime birthDate)
    {
        int Age = 0;

        // age relative to year
        int RelativeAge = now.Year - birthDate.Year;

        if (RelativeAge > 0)
        {
            Age += (RelativeAge - 1);
            
            // made anniversary
            if (now.Month > birthDate.Month)
            {
                Age += 1;
            }
            else if (now.Month == birthDate.Month)
            {
                if (birthDate.Day >= now.Day)
                {
                    // made anniversary
                    Age += 1;
                }
            }
        }
        return Age;
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

        User.FirstName = FirstName.Text;
        User.SurName = SurName.Text;
        User.Age = CalculateAge(DateTime.Now, AgePicker.Date);
        User.Age = -1;

        StringContent SerializedData = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");

        var Response = await HttpClient.PostAsync(ApiUrl, SerializedData);

        string ResponseStr = await Response.Content.ReadAsStringAsync();

        ResponseField.Text = ResponseStr;

    }

}