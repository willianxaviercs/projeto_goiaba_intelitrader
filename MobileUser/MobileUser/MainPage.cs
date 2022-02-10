using System;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using MobileUser;
using System.Diagnostics;
using System.Threading.Tasks;

public class MainPage : ContentPage
{
    Button CreateButton;

    Label FirstNameLabel;
    Entry FirstName;

    Entry SurName;

    Label AgeEntryLabel;
    Entry  AgeEntry;

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

        StackLayout Panel = new StackLayout {
            Spacing = 15,
        };

        Panel.Children.Add(new Label {
            Text = "SIGN UP",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            TextColor = Color.FromHex("#4287f5"),
            FontSize = 20
            });

        Panel.Children.Add(FirstNameLabel = new Label {
            Text = "Required",
            TextColor = Color.FromHex("#ff0000"),
            FontSize = 12,
            Padding = 0,
        });

        Panel.Children.Add(FirstName = new Entry {
            Placeholder = "Enter your firstname",
        });

        Panel.Children.Add(SurName = new Entry {
            Placeholder = "Enter your surname",
        });

        Panel.Children.Add(AgeEntryLabel = new Label {
            Text = "Required",
            TextColor = Color.FromHex("#ff0000"),
            FontSize = 12,
        });

        Panel.Children.Add(AgeEntry = new Entry
        {
            Placeholder = "Enter your age",
            Keyboard = Keyboard.Numeric,
        });

        Panel.Children.Add(CreateButton = new Button {
            Text = "CREATE USER",
            TextColor = Color.FromHex("#eeeeee"),
            BackgroundColor = Color.FromHex("#4287f5"),
        });

        // STUDY: operator overload ?? 
        CreateButton.Clicked += OnCreateBtn;
        AgeEntry.TextChanged += AgeEntryTextChange;
        FirstName.TextChanged += NameEntryTextChange;

        this.Content = Panel;
    }

    async void NameEntryTextChange(object sender, TextChangedEventArgs e)
    {
        var NewText = e.NewTextValue;

        if (string.IsNullOrEmpty(NewText))
        {
            FirstNameLabel.IsVisible = true;
        }
        else
        {
            FirstNameLabel.IsVisible = false;
        }
    }

    bool StringIsNumeric(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
            {
                return false;
            }
        }
        return true;
    }

    async void AgeEntryTextChange(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue)) return;

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
            AgeEntryLabel.IsVisible = false;
        }
    }

    // validate input fields
    bool ValidateInput()
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

        if (string.IsNullOrWhiteSpace(AgeEntry.Text))
        {
            Result = false;
        }

        return Result;
    }

    async void OnCreateBtn(object sender, EventArgs e)
    {
        // validade entrys?
        if (ValidateInput())
        {
            CreateUser();
        }
        else
        {
            // TODO: alert
            await DisplayAlert("Alert", "Required fields!", "OK");
        }
    }
    async void CreateUser()
    {        
        // TODO: check for internet status
        // set and faster timeout to better user feedback, or
        // check for server availability on app startup

        User.FirstName = FirstName.Text;
        User.SurName = SurName.Text;
        //User.Age = CalculateAge(DateTime.Now, AgePicker.Date);
        User.Age = Convert.ToInt32(AgeEntry.Text);
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
