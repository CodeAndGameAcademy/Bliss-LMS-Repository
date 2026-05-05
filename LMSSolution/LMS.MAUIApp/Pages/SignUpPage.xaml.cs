using LMS.MAUIApp.Helper;
using System.Threading.Tasks;

namespace LMS.MAUIApp.Pages;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private void OnLblSignInTapped(object sender, EventArgs e)
    {
        AppNavigator.SetRoot(new SignInPage());
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Terms", "Open Terms Of Service", "OK");        
        // await Browser.OpenAsync("https://yourwebsite.com/terms");
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Privacy", "Open Privacy Policy", "OK");        
        // await Browser.OpenAsync("https://yourwebsite.com/privacy");
    }

    private void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        // SignUp Logic

        // After Successful SignUp - Redirected to SignIn
        AppNavigator.SetRoot(new SignInPage());
    }
}