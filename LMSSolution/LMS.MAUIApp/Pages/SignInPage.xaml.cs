using LMS.MAUIApp.Helper;

namespace LMS.MAUIApp.Pages;

public partial class SignInPage : ContentPage
{
	public SignInPage()
	{
		InitializeComponent();
	}

    private void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        // Login Logic

        // After Succcessful Login - Redirected to HomePage
        AppNavigator.SetRoot(new CustomTabbedPage());
    }

    private void OnLblSignUpTapped(object sender, EventArgs e)
    {
        AppNavigator.SetRoot(new SignUpPage());
    }

    private async void OnLblPasswordTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Terms", "Open Terms Of Service", "OK");
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
}