using System;
using Xamarin.Forms;
using System.Net.Http;

public class MainPage : ContentPage
{
    Button GetButton;
    Label ResponseField;
    HttpClient _client;

    public MainPage(HttpClient httpClient)
    {
        _client = httpClient;

        this.Padding = new Thickness(20, 20, 20, 20);

        StackLayout panel = new StackLayout
        {
            Spacing = 15
        };

        // get button
        GetButton = new Button
        {
            Text = "GET"
        };

        panel.Children.Add(GetButton);
        
        // http response text field
        ResponseField = new Label
        {
            Text = "Response",
            FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
        };

        panel.Children.Add(ResponseField);

        GetButton.Clicked += OnGet;

        this.Content = panel;
    }

    async void OnGet(object sender, EventArgs e)
    {
        // TODO: check for internet connection, check if server is online

        //HttpResponseMessage response = await _client.GetAsync("https://10.0.2.2:8001/users");

        string text = await _client.GetStringAsync("https://10.0.2.2:8001/users");

        ResponseField.Text = text;

    }
}