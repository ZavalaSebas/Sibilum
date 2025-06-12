using Plugin.Maui.Audio;

namespace Sibilum
{
    public partial class MainPage : ContentPage
    {
        private IAudioPlayer _bgPlayer;
        private IAudioPlayer _tapSoundPlayer;
        private bool _firstTitleDone = false;
        private bool userHasTapped = false;
        private bool hintShown = false;


        public MainPage()
        {
            InitializeComponent();
            PlayBackgroundMusic();
            InitializeTitleText("Sibilum");
            AnimateIntro();
        }

        private async void PlayBackgroundMusic()
        {
            var audioService = AudioManager.Current;
            var file = await FileSystem.OpenAppPackageFileAsync("strangeAmbientLoop.mp3");
            _bgPlayer = audioService.CreatePlayer(file);
            _bgPlayer.Loop = true;
            _bgPlayer.Volume = 0.4;
            _bgPlayer.Play();
        }

        private async void AnimateIntro()
        {
            TitleLabel.Opacity = 0;
            SubtitleLabel.Opacity = 0;
            TitleLabel.Scale = 0.8;
            SubtitleLabel.Scale = 0.8;

            await Task.Delay(1000);
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
            _ = StartTitleVariationsAnimated();
        }

        private void StartBubbleAnimations()
        {
            StartBubbleCycle(Bubble1, 8500);
            StartBubbleCycle(Bubble10, 9500);
            StartBubbleCycle(Bubble11, 9200);
            StartBubbleCycle(Bubble12, 8800);
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

                bubble.Scale = originalScale;
                bubble.TranslationY = 0;
                bubble.Rotation = 0;

                await Task.Delay(rnd.Next(2500, 4500));
            }
        }

        private async Task StartTitleVariationsAnimated()
        {
            await Task.Delay(5000); // ⏳ Espera inicial antes de empezar a escribir

            string[] variantes =
            {
                "Sibilum", "sibilum", "𝓢𝓲𝓫𝓲𝓵𝓾𝓶", "Ｓｉｂｉｌｕｍ", "s!bilum", "S🜁bilum", "siB!LUM", "Śïbįlūm"
            };

            int i = 0;
            Random rnd = new();

            while (true)
            {
                string next = variantes[i % variantes.Length];
                bool isFirst = i == 0;
                bool randomized = isFirst || rnd.NextDouble() > 0.75; // Más lento por defecto
                int delay = randomized ? 260 : 180;

                await AnimateTitleLetterByLetter(next, randomized, delay);

                if (!_firstTitleDone)
                {
                    _firstTitleDone = true;
                    await Task.Delay(2000);
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(10000); // Espera 10 segundos

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await SubtitleLabel.FadeTo(0, 500);
                            SubtitleLabel.Text = "Presiona para continuar...";
                            await SubtitleLabel.FadeTo(1, 800);

                            // Animación de parpadeo continua
                            while (true)
                            {
                                await SubtitleLabel.FadeTo(0.2, 1000, Easing.SinInOut);
                                await SubtitleLabel.FadeTo(1, 1000, Easing.SinInOut);
                            }
                        });
                    });
                }

                i++;
                await Task.Delay(4500);
            }
        }

        private async Task AnimateTitleLetterByLetter(string newText, bool randomized, int delayPerChar)
        {
            TitleLabel.FormattedText = new FormattedString();
            var random = new Random();

            foreach (char c in newText)
            {
                var span = new Span
                {
                    Text = c.ToString(),
                    FontSize = randomized ? random.Next(42, 57) : 48,
                    FontAttributes = randomized && random.Next(0, 2) == 1 ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = randomized ?
                        (random.Next(0, 2) == 0 ? Color.FromArgb("#fbd0ff") : Color.FromArgb("#dab3f7"))
                        : (Color)Application.Current.Resources["PrimaryText"]
                };

                TitleLabel.FormattedText.Spans.Add(span);
                await Task.Delay(delayPerChar);
            }
        }

        private void InitializeTitleText(string text)
        {
            TitleFormatted.Spans.Clear();

            foreach (char c in text)
            {
                var span = new Span
                {
                    Text = c.ToString(),
                    FontSize = 48,
                    TextColor = (Color)Application.Current.Resources["PrimaryText"]
                };

                TitleFormatted.Spans.Add(span);
            }
        }

        private async void OnScreenTapped(object sender, EventArgs e)
        {
            if (!_firstTitleDone) return; // ⛔ Bloquea el tap hasta que se haya escrito el primer título

            TouchOverlay.IsVisible = false;
            await PlayTapSound();

            BlackOverlay.IsVisible = true;
            await BlackOverlay.FadeTo(1, 1600, Easing.CubicInOut);

            await Task.Delay(1500);
            await Shell.Current.GoToAsync("///IntroPage");
        }

        private async Task PlayTapSound()
        {
            var audioService = AudioManager.Current;
            var tapFile = await FileSystem.OpenAppPackageFileAsync("ThisActionWillHaveConsequences.mp3");
            _tapSoundPlayer = audioService.CreatePlayer(tapFile);
            _tapSoundPlayer.Volume = 0.7;
            _tapSoundPlayer.Play();
        }
    }
}
