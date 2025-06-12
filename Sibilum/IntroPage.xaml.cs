
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
        await Task.Delay(1200); // Pausa inicial

        await ShowTextGradually(MessageLabel, "Un susurro te espera...", 100);
        await Task.Delay(1800);

        await MessageLabel.FadeTo(0, 800, Easing.CubicOut);
        await Task.Delay(800);

        await ShowTextGradually(QuestionLabel, "¿Quién eres?", 100);
        await Task.Delay(1800);

        await NameEntry.FadeTo(1, 1200, Easing.CubicIn);
        await Task.Delay(2000);

        await ContinueButtonContainer.FadeTo(1, 1200, Easing.CubicInOut);
    }

    private async Task ShowTextGradually(Label label, string text, int delayPerChar)
    {
        label.Text = "";
        label.Opacity = 1;

        foreach (char c in text)
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
