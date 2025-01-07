namespace Wordle_Project_
{ 


    public partial class SettingsPage : ContentPage
    {
     
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage(), true);
        }

        public SettingsPage()
        {
            InitializeComponent();
            SetupThemeButtons();
        }

        private void SetupThemeButtons()
        {
            DarkModeButton.Clicked += (s, e) =>
            {
                ThemeManager.IsDarkMode = true;
                ThemeManager.ApplyTheme();
                UpdateThemeButtonStyles();
            };

            LightModeButton.Clicked += (s, e) =>
            {
                ThemeManager.IsDarkMode = false;
                ThemeManager.ApplyTheme();
                UpdateThemeButtonStyles();
            };

            UpdateThemeButtonStyles();
        }

        private void UpdateThemeButtonStyles()
        {
            DarkModeButton.BackgroundColor = ThemeManager.IsDarkMode ?
                Color.FromArgb("#9a9a9a") : Color.FromArgb("#454545");
            LightModeButton.BackgroundColor = !ThemeManager.IsDarkMode ?
                Color.FromArgb("#9a9a9a") : Color.FromArgb("#454545");
        }

    }
   
}
