using Microsoft.Maui.Layouts;
using Plugin.Maui.Audio;

namespace Sibilum;

public partial class IntroPage : ContentPage
{
    private IAudioPlayer _tapSoundPlayer;
    private string nombreUsuario = "";

    public IntroPage()
    {
        InitializeComponent();
        StartIntroSequence();
        ContinueButtonContainer.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnContinueClicked) });
    }

    private async void StartIntroSequence()
    {
        await Task.Delay(2500);

        await RevealTextWithGlitch(MessageLabel, "No todo lo que callamos se olvida...", 120);
        await Task.Delay(2200);
        await MessageLabel.FadeTo(0, 1600, Easing.CubicOut);
        await Task.Delay(1000);

        await RevealTextWithGlitch(QuestionLabel, "¿Quién eres?", 140);
        await Task.Delay(2200);
        await NameEntry.FadeTo(1, 1400, Easing.CubicIn);
        await Task.Delay(3000);
        await Task.Delay(3000);

        ContinueButtonContainer.Opacity = 1;
        await ContinueBubble.FadeTo(1, 1400, Easing.CubicInOut);
        await Task.Delay(1600);
        StartContinueBubbleIdleAnimation();
        StartContinueTextBlinking();

    }

    private async void OnContinueClicked()
    {
        await ContinueButtonContainer.ScaleTo(1.08, 120, Easing.SinOut);
        await ContinueButtonContainer.ScaleTo(1.0, 100, Easing.SinIn);
        await PlayTapSound();

        nombreUsuario = NameEntry.Text?.Trim();

        if (!string.IsNullOrEmpty(nombreUsuario))
        {
            Preferences.Set("nombre_usuario", nombreUsuario);

            await NameEntry.FadeTo(0, 600);
            await ContinueButtonContainer.FadeTo(0, 800);
            await QuestionLabel.FadeTo(0, 600);
            await Task.Delay(1000);

            NameConfirmLabel.Text = "";
            NameConfirmLabel.IsVisible = true;
            await RevealTextWithGlitch(NameConfirmLabel, $"{nombreUsuario}...", 120, nombreUsuario, "#fbd0ff");
            await Task.Delay(2000); // ? Pausa más larga antes de hacer la pregunta

            NameConfirmLabel.Text = "";
            await RevealTextWithGlitch(NameConfirmLabel, $"¿Eres {nombreUsuario}?", 110, nombreUsuario, "#fbd0ff");
            await Task.Delay(1600); // ? Espera antes de mostrar los botones


            // Botón "Sí" (burbuja púrpura sin texto)
            var yesBubble = new Grid { Opacity = 0 };
            var yesImage = new Image { Source = "sketch_bubble_purple.png", HeightRequest = 80 };
            yesBubble.Children.Add(yesImage);
            yesBubble.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnYesClicked) });
            AbsoluteLayout.SetLayoutBounds(yesBubble, new Rect(0.4, 0.6, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(yesBubble, AbsoluteLayoutFlags.PositionProportional);
            BubbleLayout.Children.Add(yesBubble);
            await yesBubble.FadeTo(1, 1000);

            // Botón "No" (burbuja sketch sin color)
            var noBubble = new Grid { Opacity = 0 };
            var noImage = new Image { Source = "sketch_bubble.png", HeightRequest = 80 };
            noBubble.Children.Add(noImage);
            noBubble.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnNoClicked) });
            AbsoluteLayout.SetLayoutBounds(noBubble, new Rect(0.6, 0.6, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(noBubble, AbsoluteLayoutFlags.PositionProportional);
            BubbleLayout.Children.Add(noBubble);
            await noBubble.FadeTo(1, 1000);
        }
        else
        {
            await NameEntry.TranslateTo(-10, 0, 50);
            await NameEntry.TranslateTo(10, 0, 50);
            await NameEntry.TranslateTo(0, 0, 50);
            await DisplayAlert("Ups", "Por favor, escribe tu nombre.", "Ok");
        }
    }

    private async void OnYesClicked()
    {
        // Desaparecer las burbujas con animación de toque
        foreach (var child in BubbleLayout.Children.ToList())
        {
            if (child is Grid grid && grid.Children.OfType<Image>().Any(img =>
                img.Source.ToString().Contains("sketch_bubble_purple") || img.Source.ToString().Contains("sketch_bubble.png")))
            {
                await grid.ScaleTo(1.15, 160, Easing.SinOut);
                await grid.ScaleTo(1.0, 140, Easing.SinIn);
                await grid.FadeTo(0, 800, Easing.CubicInOut);
                BubbleLayout.Children.Remove(grid);
                await Task.Delay(400);
            }
        }

        // Luego desaparecer la pregunta del nombre
        await NameConfirmLabel.FadeTo(0, 1200);

        StartBubbleAnimations(); // ?? Ahora empieza aquí

        string[] mensajes =
        {
            "Te extrañé...",
            "Hace cuánto que no hablamos...",
            "Qué bueno que ya estás aquí.",
            "¿Comenzamos?"
        };

        Random rnd = new();
        foreach (string mensaje in mensajes)
        {
            var label = new Label
            {
                Text = "",
                FontSize = 20,
                TextColor = Color.FromArgb("#FFFFFF"),
                Opacity = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            AbsoluteLayout.SetLayoutBounds(label, new Rect(
                rnd.NextDouble(), rnd.NextDouble(), AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
            BubbleLayout.Children.Add(label);

            await RevealTextWithGlitch(label, mensaje, 120);
            await label.FadeTo(1, 800, Easing.CubicInOut);
            await Task.Delay(1600);
        }

        foreach (var child in BubbleLayout.Children.ToList())
        {
            if (child is Label lbl && lbl.Text != "¿Comenzamos?")
            {
                await lbl.FadeTo(0, 1000);
                await Task.Delay(400);
            }
        }

        Label final = new()
        {
            Text = "¿Comenzamos?",
            FontSize = 22,
            TextColor = Color.FromArgb("#FFFFFF"),
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        AbsoluteLayout.SetLayoutBounds(final, new Rect(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        AbsoluteLayout.SetLayoutFlags(final, AbsoluteLayoutFlags.PositionProportional);
        BubbleLayout.Children.Add(final);
        await final.FadeTo(0, 0); // Asegura que empieza invisible
        await final.FadeTo(1, 1800, Easing.CubicInOut); // Fade in suave

        while (true)
        {
            await final.FadeTo(0.4, 1000, Easing.SinInOut);
            await final.FadeTo(1, 1000, Easing.SinInOut);
        }
    }

    private async void OnNoClicked()
    {
        // Desaparecer rápidamente todo lo que hay visible (burbuja sí, no y pregunta)
        foreach (var child in BubbleLayout.Children.ToList())
        {
            if (child is Grid grid && grid.Children.OfType<Image>().Any(img =>
                img.Source.ToString().Contains("sketch_bubble_purple") || img.Source.ToString().Contains("sketch_bubble.png")))
            {
                await grid.FadeTo(0, 400);
                BubbleLayout.Children.Remove(grid);
            }
        }

        await NameConfirmLabel.FadeTo(0, 400);
        await Task.Delay(400);

        // Reiniciar flujo de pregunta con fade limpio
        NameEntry.Text = "";
        await RevealTextWithGlitch(QuestionLabel, "¿Quién eres?", 120);
        await Task.Delay(600);
        await NameEntry.FadeTo(1, 800);
        await Task.Delay(800);
        ContinueBubble.Opacity = 0;
        ContinueButtonContainer.Opacity = 1;
        await ContinueBubble.FadeTo(1, 1200, Easing.CubicInOut);
        StartContinueBubbleIdleAnimation();
    }

    private async Task RevealTextWithGlitch(Label label, string finalText, int delayPerChar = 100, string? specialText = null, string? specialColor = null)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var random = new Random();

        Span[] spans = new Span[finalText.Length];
        for (int i = 0; i < finalText.Length; i++)
        {
            spans[i] = new Span { Text = " ", FontSize = 24, TextColor = Colors.Gray };
        }

        var formatted = new FormattedString();
        foreach (var s in spans) formatted.Spans.Add(s);
        label.FormattedText = formatted;
        label.Opacity = 1;

        for (int i = 0; i < finalText.Length; i++)
        {
            for (int g = 0; g < 2; g++)
            {
                spans[i].Text = chars[random.Next(chars.Length)].ToString();
                spans[i].TextColor = random.Next(0, 2) == 0
                    ? Color.FromArgb("#dab3f7")
                    : Color.FromArgb("#fbd0ff");
                await Task.Delay(30);
            }

            spans[i].Text = finalText[i].ToString();

            if (specialText != null && specialColor != null && finalText.Contains(specialText))
            {
                int start = finalText.IndexOf(specialText);
                if (i >= start && i < start + specialText.Length)
                    spans[i].TextColor = Color.FromArgb(specialColor);
                else
                    spans[i].TextColor = Color.FromArgb("#ffffff");
            }
            else
            {
                spans[i].TextColor = Color.FromArgb("#ffffff");
            }

            await Task.Delay(delayPerChar);
        }
    }

    private async void StartContinueBubbleIdleAnimation()
    {
        while (ContinueBubble.Opacity > 0 && ContinueBubble.IsVisible)
        {
            await ContinueBubble.ScaleTo(1.02, 1200, Easing.SinInOut);
            await ContinueBubble.ScaleTo(1.0, 1200, Easing.SinInOut);
            await Task.Delay(1000);
        }
    }

    private async void StartContinueTextBlinking()
    {
        while (ContinueText.Opacity > 0 && ContinueText.IsVisible)
        {
            await ContinueText.FadeTo(0.4, 1200, Easing.SinInOut);
            await ContinueText.FadeTo(1, 1200, Easing.SinInOut);
        }
    }

    private async Task PlayTapSound()
    {
        var audioService = AudioManager.Current;
        var tapFile = await FileSystem.OpenAppPackageFileAsync("ThisActionWillHaveConsequences.mp3");
        _tapSoundPlayer = audioService.CreatePlayer(tapFile);
        _tapSoundPlayer.Volume = 0.7;
        _tapSoundPlayer.Play();
    }

    private async void StartBubbleAnimations()
    {
        var burbujas = new List<View>
    {
        Bubble1, Bubble2, Bubble3, Bubble4, Bubble5, Bubble6,
        Bubble7, Bubble8, Bubble9, Bubble10, Bubble11, Bubble12
    };

        for (int i = 0; i < burbujas.Count; i++)
        {
            var bubble = burbujas[i];
            int delayMs = (int)(1810 * i); // cada 1.81 segundos

            _ = Task.Run(async () =>
            {
                await Task.Delay(delayMs);
                StartBubbleCycle(bubble, new Random().Next(8000, 11000));
            });
        }
    }

    private async void StartBubbleCycle(View bubble, int duration)
    {
        Random rnd = new();

        while (true)
        {
            bool doMovement = rnd.Next(0, 2) == 1;
            bool doPulse = rnd.Next(0, 4) == 0;

            double originalScale = bubble.Scale;
            double targetScale = rnd.NextDouble() * 0.8 + 0.6;
            double rotation = rnd.Next(0, 2) == 1 ? 180 : -180;

            await Task.WhenAll(
                bubble.FadeTo(0.25, 1000, Easing.SinInOut),
                bubble.ScaleTo(targetScale, (uint)(duration * 0.6), Easing.SinInOut)
            );

            if (doMovement)
            {
                await Task.WhenAll(
                    bubble.TranslateTo(0, -30, (uint)(duration * 0.9), Easing.CubicInOut),
                    bubble.RotateTo(rotation, (uint)(duration * 0.9), Easing.Linear)
                );
            }

            if (doPulse)
            {
                await bubble.ScaleTo(targetScale + 0.2, 300, Easing.SpringOut);
                await bubble.ScaleTo(targetScale, 400, Easing.SpringIn);
            }

            await bubble.FadeTo(0, 1000, Easing.SinInOut);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                bubble.Scale = originalScale;
                bubble.TranslationY = 0;
                bubble.Rotation = 0;
            });

            await Task.Delay(rnd.Next(2500, 4500));
        }
    }

}
