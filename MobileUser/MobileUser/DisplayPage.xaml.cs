using System;
using System.Globalization;
using Xamarin.Forms;

namespace MobileUser
{
    public partial class DisplayPage : ContentPage
    {
        public DisplayPage(User user)
        {
            InitializeComponent();

            string SurName = user.SurName == null ? "" : user.SurName;
            string Fullname = $"{user.FirstName} {SurName}";
            string Age = $"{Convert.ToString(user.Age)}";
            string DateStr = user.CreationTime.ToString(
                "f",
                CultureInfo.CreateSpecificCulture("pt-BR")
                );
            string Id = user.Id.ToString();

            NameDisplay.Text = Fullname;
            AgeDisplay.Text = Age;
            IdDisplay.Text = Id;
            DateDisplay.Text  = $"{DateStr} UTC";
        }
    }
}
