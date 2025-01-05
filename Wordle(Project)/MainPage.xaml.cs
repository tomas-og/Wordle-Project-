using Microsoft.Maui.Controls.Shapes;


namespace Wordle_Project_
{
    public partial class MainPage : ContentPage
    {
        private string currentWord;
        private int currentRow = 0;
        private int currentColumn = 0;
        private bool gameActive = true;
        private Label debugLabel;
        private bool isDarkMode = true;

        public MainPage()
        {
            InitializeComponent();
            SetupDebugLabel();
            SetupKeyboardEvents();
            Loaded += MainPage_Loaded;
        }

        //Debug so I can see what the word is for testing will be removed later
        private void SetupDebugLabel()
        {
            debugLabel = new Label
            {
                Text = "Current Word: ",
                TextColor = Color.FromArgb("#eeeeee"),
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(10)
            };
            mainLayout.Children.Insert(0, debugLabel);
        }

        //Gets the word from the WordService and sets the game to active
        private async void MainPage_Loaded(object sender, EventArgs e)
        {
            await InitializeGame();
        }

        private async Task InitializeGame()
        {
            currentWord = await WordService.GetRandomWord();
            gameActive = true;
            debugLabel.Text = $"Current Word: {currentWord}";
        }

        private void SetupKeyboardEvents()
        {
            var keyButtons = keyboardGrid.GetVisualTreeDescendants().OfType<Button>();
            foreach (var button in keyButtons)
            {
                button.Clicked += OnKeyPressed;
            }
        }

        //Sets the current column to the button pressed and checks if the button is backspace or enter
        private void OnKeyPressed(object sender, EventArgs e)
        {
            if (!gameActive) return;

            var button = (Button)sender;
            var key = button.Text;

            switch (key)
            {
                case "⌫":
                    if (currentColumn > 0)
                    {
                        currentColumn--;
                        UpdateGridButton(currentRow, currentColumn, "");
                    }
                    break;

                case "ENTER":
                    if (currentColumn == 5)
                        CheckWord();
                    break;

                default:
                    if (currentColumn < 5)
                    {
                        UpdateGridButton(currentRow, currentColumn, key);
                        currentColumn++;
                    }
                    break;
            }
        }

        //Updates the button in the grid with the letter
        private void UpdateGridButton(int row, int col, string letter)
        {
            var stackLayout = (HorizontalStackLayout)grid.Children[row];
            var button = (Button)stackLayout.Children[col];
            button.Text = letter;
        }

        //Checks the word and updates the colors of the buttons
        private async void CheckWord()
        {
            var stackLayout = (HorizontalStackLayout)grid.Children[currentRow];
            var guessedWord = string.Join("", stackLayout.Children.OfType<Button>().Select(b => b.Text));

            if (!await WordService.IsValidWord(guessedWord))
            {
                await DisplayAlert("Invalid Word", "Please enter a valid 5-letter word", "OK");
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                var button = (Button)stackLayout.Children[i];
                if (guessedWord[i] == currentWord[i])
                {
                    button.BackgroundColor = Color.FromArgb("#538d4e");
                    UpdateKeyboardColor(guessedWord[i].ToString(), "#538d4e");
                }
                else if (currentWord.Contains(guessedWord[i]))
                {
                    button.BackgroundColor = Color.FromArgb("#b59f3b");
                    UpdateKeyboardColor(guessedWord[i].ToString(), "#b59f3b");
                }
                else
                {
                    button.BackgroundColor = Color.FromArgb("#3a3a3c");
                    UpdateKeyboardColor(guessedWord[i].ToString(), "#3a3a3c");
                }
            }

            if (guessedWord == currentWord)
            {
                await HandleGameEnd(true);
            }
            else if (currentRow == 5)
            {
                await HandleGameEnd(false);
            }
            else
            {
                currentRow++;
                currentColumn = 0;
            }
        }

        //Updates the color of the keyboard buttons
        private void UpdateKeyboardColor(string letter, string colorHex)
        {
            var keyButton = keyboardGrid.GetVisualTreeDescendants()
                .OfType<Button>()
                .FirstOrDefault(b => b.Text == letter);

            if (keyButton != null)
            {
                var currentColor = keyButton.BackgroundColor.ToHex();
                if (currentColor != "#538d4e")
                {
                    keyButton.BackgroundColor = Color.FromArgb(colorHex);
                }
            }
        }

        //Handles the game ending and displays an alert
        private async Task HandleGameEnd(bool isWin)
        {
            gameActive = false;
            await DisplayAlert(
                isWin ? "Congratulations!" : "Game Over",
                isWin ? "You won!" : $"The word was {currentWord}",
                "OK"
            );
        }

        //Theme code (Not working) Needs to be fixed (Settings button opens a new page with a button to change the theme instead of changing the theme on the current page)
        private void ApplyTheme()
        {
            var theme = isDarkMode ? GetDarkTheme() : GetLightTheme();

            this.BackgroundColor = Color.FromArgb(theme.Background);

            foreach (var label in mainLayout.Children.OfType<Label>())
            {
                label.TextColor = Color.FromArgb(theme.TextColor);
            }

            foreach (HorizontalStackLayout row in grid.Children)
            {
                foreach (Button button in row.Children)
                {
                    if (button.BackgroundColor.ToHex() == "#252525" ||
                        button.BackgroundColor.ToHex() == "#ffffff")
                    {
                        button.BackgroundColor = Color.FromArgb(theme.Background);
                        button.TextColor = Color.FromArgb(theme.TextColor);
                        button.BorderColor = Color.FromArgb(theme.BorderColor);
                    }
                }
            }

            var keyButtons = keyboardGrid.GetVisualTreeDescendants().OfType<Button>();
            foreach (var button in keyButtons)
            {
                if (button.BackgroundColor.ToHex() == "#9a9a9a" ||
                    button.BackgroundColor.ToHex() == "#d3d6da")
                {
                    button.BackgroundColor = Color.FromArgb(theme.KeyboardButton);
                    button.TextColor = Color.FromArgb(theme.KeyboardText);
                }
            }
        }

        private Theme GetDarkTheme()
        {
            return new Theme
            {
                Background = "#252525",
                TextColor = "#eeeeee",
                BorderColor = "#3a3a3c",
                KeyboardButton = "#9a9a9a",
                KeyboardText = "#ffffff"
            };
        }

        private Theme GetLightTheme()
        {
            return new Theme
            {
                Background = "#ffffff",
                TextColor = "#000000",
                BorderColor = "#d3d6da",
                KeyboardButton = "#d3d6da",
                KeyboardText = "#000000"
            };
        }

        private class Theme
        {
            public string Background { get; set; }
            public string TextColor { get; set; }
            public string BorderColor { get; set; }
            public string KeyboardButton { get; set; }
            public string KeyboardText { get; set; }
        }

        //Button to change the theme
        private void Themes()
        {
            var theme = isDarkMode ? GetDarkTheme() : GetLightTheme();

            this.BackgroundColor = Color.FromArgb(theme.Background);

            foreach (var label in mainLayout.Children.OfType<Label>())
            {
                label.TextColor = Color.FromArgb(theme.TextColor);
            }

            foreach (HorizontalStackLayout row in grid.Children)
            {
                foreach (Button button in row.Children)
                {
                    if (button.BackgroundColor.ToHex() == "#252525" ||
                        button.BackgroundColor.ToHex() == "#ffffff")
                    {
                        button.BackgroundColor = Color.FromArgb(theme.Background);
                        button.TextColor = Color.FromArgb(theme.TextColor);
                        button.BorderColor = Color.FromArgb(theme.BorderColor);
                    }
                }
            }

            var keyButtons = keyboardGrid.GetVisualTreeDescendants().OfType<Button>();
            foreach (var button in keyButtons)
            {
                if (button.BackgroundColor.ToHex() == "#9a9a9a" ||
                    button.BackgroundColor.ToHex() == "#d3d6da")
                {
                    button.BackgroundColor = Color.FromArgb(theme.KeyboardButton);
                    button.TextColor = Color.FromArgb(theme.KeyboardText);
                }
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ApplyTheme();   
        }

      
    }
}