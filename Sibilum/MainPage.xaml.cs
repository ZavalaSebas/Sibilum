using Plugin.Maui.Audio;

namespace Sibilum
{
    public partial class MainPage : ContentPage
    {
        private IAudioPlayer _player;

        public MainPage()
        {
            InitializeComponent();
            PlayBackgroundMusic(); // Agregar la música apenas inicie
            AnimateIntro();
        }

        private async void PlayBackgroundMusic()
        {
            var audioService = AudioManager.Current;
            var audioFile = await FileSystem.OpenAppPackageFileAsync("strangeAmbientLoop.mp3");

            _player = audioService.CreatePlayer(audioFile);
            _player.Loop = true; // Para que se repita infinitamente
            _player.Volume = 0.4; // Volumen suave

            _player.Play();
        }

        private async void AnimateIntro()
        {
            TitleLabel.Opacity = 0;
            SubtitleLabel.Opacity = 0;
            TitleLabel.Scale = 0.8;
            SubtitleLabel.Scale = 0.8;

            await Task.Delay(1000); // Delay inicial
            await Task.WhenAll(
                TitleLabel.FadeTo(1, 1800, Easing.CubicIn),
                TitleLabel.ScaleTo(1, 1800, Easing.CubicOut)
            );

            await Task.Delay(800);

            await Task.WhenAll(
                SubtitleLabel.FadeTo(1, 2200, Easing.CubicOut),
                SubtitleLabel.ScaleTo(1, 2200, Easing.CubicOut)
            );

            StartBubbleAnimations();
        }

        private void StartBubbleAnimations()
        {
            // 🎨 Imágenes sketch
            StartBubbleCycle(Bubble1, 8500);
            StartBubbleCycle(Bubble10, 9500);

            // ✏️ Sketch tipo stroke
            StartBubbleCycle(Bubble11, 9200);
            StartBubbleCycle(Bubble12, 8800);

            // 🎈 Normales
            StartBubbleCycle(Bubble2, 10000);
            StartBubbleCycle(Bubble3, 7500);
            StartBubbleCycle(Bubble4, 9000);
            StartBubbleCycle(Bubble5, 7000);
            StartBubbleCycle(Bubble6, 8500);
            StartBubbleCycle(Bubble7, 9500);
            StartBubbleCycle(Bubble8, 7000);
            StartBubbleCycle(Bubble9, 8800);
        }
        private async void StartBubbleCycle(View bubble, int duration)
        {
            Random rnd = new();

            while (true)
            {
                bool doMovement = rnd.Next(0, 2) == 1;
                bool doPulse = rnd.Next(0, 4) == 0;

                double originalScale = bubble.Scale;
                double targetScale = rnd.NextDouble() * 0.8 + 0.6; // Escala entre 0.6 y 1.4
                double rotation = rnd.Next(0, 2) == 1 ? 180 : -180;

                // Fade in y scale aleatorio
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

                // Fade out antes de reset
                await bubble.FadeTo(0, 1000, Easing.SinInOut);

                // Reset propiedades
                bubble.Scale = originalScale;
                bubble.TranslationY = 0;
                bubble.Rotation = 0;

                await Task.Delay(rnd.Next(2500, 4500));
            }
        }
        private async void OnScreenTapped(object sender, EventArgs e)
        {
            TouchOverlay.IsVisible = false;

            BlackOverlay.IsVisible = true;
            await BlackOverlay.FadeTo(1, 1600, Easing.CubicInOut);

            await Task.Delay(1200);
            IntroPromptLayout.IsVisible = true;

            await MysteryLabel.FadeTo(1, 1600, Easing.CubicInOut);
            await Task.Delay(1600);

            await QuestionLabel.FadeTo(1, 1400, Easing.CubicOut);
            await Task.Delay(1000);

            await NameEntry.FadeTo(1, 1000, Easing.CubicIn);
            await Task.Delay(1500);

            await ContinueButton.FadeTo(1, 1000, Easing.SinInOut);
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            string nombre = NameEntry.Text?.Trim();

            if (!string.IsNullOrEmpty(nombre))
            {
                Preferences.Set("nombre_usuario", nombre); // Se guarda de forma persistente
                await DisplayAlert("Nombre guardado", $"Hola, {nombre}", "Continuar");

                // Aquí puedes hacer la transición a la siguiente página o escena
            }
            else
            {
                await DisplayAlert("Ups", "Por favor, escribe tu nombre antes de continuar.", "Ok");
            }
        }


    }

}
