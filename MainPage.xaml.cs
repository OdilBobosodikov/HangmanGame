using System.ComponentModel;
using Microsoft.Maui.Controls.Shapes;


namespace HangmanGame;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    #region UI properties
    public string Spotlight
    {
        get => spotlight; 
        set
        {
            spotlight = value;
            OnPropertyChanged();
        }
    }
    public List<char> Letters
    {
        get => letters; 
        set
        {
            letters = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get => message; 
        set
        {
            message = value;
            OnPropertyChanged();
        }
    }
    public string GameStatus
    {
        get => gameStatus;
        set
        {
            gameStatus = value;
            OnPropertyChanged();
        }
    }
    #endregion
    #region Fields
    private List<string> words = new();
    private string answer;
    private string spotlight;
    private List<char> guessed = new();
    private List<char> letters = new();
    private string message;
    private int mistakes = 0;
    int maxAttempts = 6;
    private string gameStatus;
    private List<Ellipse> lives;
    private Color availableButton = Color.FromHex("#3094ff");
    private Color disabledButton = Color.FromHex("#c2c2c2");
    #endregion

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMauiAsset();
        IdentifyWord();
        CalculateWord(answer, guessed);
    }

    async Task LoadMauiAsset()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("words.txt");
        using var reader = new StreamReader(stream);

        while (reader.Peek() != -1)
        {
            words.Add(reader.ReadLine());
        }
    }

    private void IdentifyWord()
    {
        answer = words[new Random().Next(0, words.Count)];
    }

    private void CalculateWord(string answer, List<char> guessed)
    {
        var temp = answer.Select(x => (guessed.IndexOf(x) >= 0 ? x : '_'))
                   .ToArray();
        Spotlight = string.Join(' ', temp);
    }

    public MainPage()
	{
		InitializeComponent();
        Letters.AddRange("abcdefghijklmnopqrstuvwxyz");
        BindingContext = this;
        GameStatus = $"Errors: {mistakes} of {maxAttempts}";
        lives = new List<Ellipse>() {sign1, sign2, sign3, sign4, sign5, sign6};
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn != null)
        {
            var letter = btn.Text;
            btn.IsEnabled = false;
            btn.BackgroundColor = new Color(194, 194, 194);
            btn.TextColor = new Color(0,0,0);
            HandleGuess(letter[0]);
        }
    }

    private void HandleGuess(char letter)
    {
        if (guessed.IndexOf(letter) == -1)
        {
            guessed.Add(letter);
        }
        if (answer.IndexOf(letter) >= 0)
        {
            CalculateWord(answer, guessed);
            CheckIfWin();
        }
        else if (answer.IndexOf(letter) == -1)
        {
            mistakes++;
            UpdateStatus();
            CheckIfLost();
        }
    }

    private void CheckIfLost()
    {
        if (mistakes == maxAttempts)
        {
            Message = $"Chosen word was {answer}";
            DisableLetters();
        }
    }

    private void CheckIfWin()
    {
        if (spotlight.Replace(" ", "").Equals(answer))
        {
            DisableLives();
            Message = "You win!";
            DisableLetters();
        }
    }

    private void DisableLetters()
    {
        foreach (var letter in LettersContainer.Children)
        {
            var btn = letter as Button;
            
            if (letter == null)
            {
                continue;
            }

            btn.IsEnabled = false;
            btn.BackgroundColor = disabledButton;
            btn.TextColor = new Color(0, 0, 0);
        }
    }

    private void EnableLetters()
    {
        foreach (var letter in LettersContainer.Children)
        {
            var btn = letter as Button;

            if (letter == null)
            {
                continue;
            }

            btn.IsEnabled = true;
            btn.BackgroundColor = availableButton;
            btn.TextColor = new Color(255, 255, 255);
        }
    }

    private void UpdateStatus()
    {
        GameStatus = $"Errors: {mistakes} of {maxAttempts}";
        if (mistakes > 0)
        {
            foreach (var live in lives)
            {
                if (live.IsVisible == true)
                {
                    live.IsVisible = false;
                    break;
                }
            }
        }
    }
    public void DisableLives()
    {
        foreach (var live in lives)
        {
            live.IsVisible = false;
        }
    }
    public void EnableLives()
    {
        foreach (var live in lives)
        {
            live.IsVisible = true;
        }
    }
    private void Reset_clicked(object sender, EventArgs e)
    {
        mistakes = 0;
        guessed = new();
        EnableLives();
        IdentifyWord();
        CalculateWord(answer, guessed);
        Message = "";
        UpdateStatus();
        EnableLetters();
    }
}

