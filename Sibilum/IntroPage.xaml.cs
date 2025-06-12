
namespace Sibilum;

public partial class IntroPage : ContentPage
{
    public IntroPage()
    {
        InitializeComponent();
        StartIntroSequence();
    }

    private async void StartIntroSequence()
    {
        await Task.Delay(2500); // Pausa inicial prolongada

        await ShowScrambledText(MessageLabel, "Un susurro te espera...", 100, 8);
        await Task.Delay(1800);

        await MessageLabel.FadeTo(0, 800, Easing.CubicOut);
        await Task.Delay(800);

        await ShowScrambledText(QuestionLabel, "¿Quién eres?", 120, 6);
        await Task.Delay(1800);

        await NameEntry.FadeTo(1, 1200, Easing.CubicIn);
        await Task.Delay(2000);

        await ContinueButtonContainer.FadeTo(1, 1200, Easing.CubicInOut);
    }

    private async Task ShowScrambledText(Label label, string finalText, int delayPerChar = 70, int mutationRounds = 6)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{};:'\",.<>?";

        label.Text = "";
        label.Opacity = 1;

        Random rnd = new();

        for (int round = 0; round < mutationRounds; round++)
        {
            string display = "";
            for (int i = 0; i < finalText.Length; i++)
            {
                if (rnd.NextDouble() < (double)(mutationRounds - round) / mutationRounds)
                {
                    display += chars[rnd.Next(chars.Length)];
                }
                else
                {
                    display += finalText[i];
                }
            }

            label.Text = display;
            await Task.Delay(delayPerChar * 2);
        }

        // Finalmente escribe el texto correcto lentamente
        label.Text = "";
        foreach (char c in finalText)
        {
            label.Text += c;
            await Task.Delay(delayPerChar);
        }
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        string nombre = NameEntry.Text?.Trim();

        if (!string.IsNullOrEmpty(nombre))
        {
            Preferences.Set("nombre_usuario", nombre);
            await DisplayAlert("Nombre guardado", $"Hola, {nombre}", "Continuar");
            // Aquí iría la navegación al siguiente paso
        }
        else
        {
            await DisplayAlert("Ups", "Por favor, escribe tu nombre.", "Ok");
        }
    }
}
