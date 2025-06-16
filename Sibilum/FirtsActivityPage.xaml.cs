namespace Sibilum;

public partial class FirstActivityPage : ContentPage
{
    private bool isReplying = false;

    public FirstActivityPage()
    {
        InitializeComponent();
        StartIntroAnimation();
    }

    private async void StartIntroAnimation()
    {
        // Fade in carta y texto inicial
        CartaFrame.Opacity = 0;
        CartaText.Opacity = 0;
        ReplyOptions.Opacity = 0;
        ReplyEntryContainer.Opacity = 0;

        await Task.Delay(1000);
        await CartaFrame.FadeTo(1, 1200, Easing.CubicInOut);
        await Task.Delay(500);
        await CartaText.FadeTo(1, 1000, Easing.CubicInOut);

        await Task.Delay(1000);
        await ReplyOptions.FadeTo(1, 1000, Easing.CubicInOut);
    }

    private async void OnReplyClicked(object sender, EventArgs e)
    {
        if (isReplying) return;
        isReplying = true;

        await ReplyOptions.FadeTo(0, 600);
        ReplyOptions.IsVisible = false;

        ReplyEntryContainer.IsVisible = true;
        await ReplyEntryContainer.FadeTo(1, 800);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Guardado", "Este mensaje fue guardado para otro día.", "OK");
        await Shell.Current.GoToAsync("///HomePage");
    }

    private async void OnSendReplyClicked(object sender, EventArgs e)
    {
        string respuesta = ReplyEntry.Text?.Trim();

        if (string.IsNullOrEmpty(respuesta))
        {
            await DisplayAlert("Ups", "Por favor, escribe una respuesta.", "OK");
            return;
        }

        // Aquí se podría guardar la respuesta o procesarla de alguna forma
        await DisplayAlert("Enviado", "Tu respuesta ha sido enviada al silencio...", "OK");
        await Shell.Current.GoToAsync("///HomePage");
    }
}
