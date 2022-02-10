using System;
using System.Globalization;
using Xamarin.Forms;

namespace MobileUser
{
    public class DisplayPage : ContentPage
    {
        public DisplayPage(User user)
        {
            this.Padding = new Thickness(20, 20, 20, 20);
            this.BackgroundColor = Color.FromHex("#eeeeee");

            StackLayout Panel = new StackLayout
            {
                Spacing = 15,
            };

            string SurName = user.SurName == null ? "" : user.SurName;
            string Fullname = $"{user.FirstName} {SurName}";
            string Age = $"{Convert.ToString(user.Age)}";
            string Date = user.CreationTime.ToString(
                "f",
                CultureInfo.CreateSpecificCulture("pt-BR")
                );
            string Id = user.Id.ToString();

            Panel.Children.Add(new Label {
                Text = "USER CREATED!",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.FromHex("#1fa121"),
                //BackgroundColor = Color.FromHex(""),
                FontSize = 24
            });

            // Full Name
            Panel.Children.Add(new Label { Text = "Name:"});

            Panel.Children.Add(new Label {
                Text = Fullname,
                TextColor = Color.FromHex("#121412"),
                FontSize = 18
            });

            // Age
            Panel.Children.Add(new Label { Text = "Age:"});

            Panel.Children.Add(new Label {
                Text = Age,
                TextColor = Color.FromHex("#121412"),
                FontSize = 18
            });

            // guid
            Panel.Children.Add(new Label { Text = "Id:"});
            Panel.Children.Add(new Label {
                Text = Id,
                TextColor = Color.FromHex("#121412"),
                FontSize = 18
            });

            // date
            Panel.Children.Add(new Label { Text = "Date:"});
            Panel.Children.Add(new Label {
                Text = $"{Date} UTC",
                TextColor = Color.FromHex("#121412"),
                FontSize = 18
            });
            
            this.Content = Panel;
        }
    }
}
