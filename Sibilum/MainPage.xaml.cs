namespace Sibilum
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            AnimateIntro();
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
            StartBubbleCycle(Bubble1, 8000);
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
                double targetScale = 1.1 + rnd.NextDouble() * 0.2;
                double rotation = rnd.Next(0, 2) == 1 ? 180 : -180;

                // APARICIÓN: fade in + scale up
                await Task.WhenAll(
                    bubble.FadeTo(0.25, 1000, Easing.SinInOut),
                    bubble.ScaleTo(targetScale, (uint)(duration * 0.6), Easing.SinInOut)
                );

                // SOLO SI se mueve, se mueve ahora (después de aparecer)
                if (doMovement)
                {
                    await Task.WhenAll(
                        bubble.TranslateTo(0, -30, (uint)(duration * 0.9), Easing.CubicInOut),
                        bubble.RotateTo(rotation, (uint)(duration * 0.9), Easing.Linear)
                    );
                }

                // Pulso solo si no va a desaparecer aún
                if (doPulse)
                {
                    await bubble.ScaleTo(targetScale + 0.3, 300, Easing.SpringOut);
                    await bubble.ScaleTo(targetScale, 400, Easing.SpringIn);
                }

                // DESAPARICIÓN primero, sin nada más
                await bubble.FadeTo(0, 1000, Easing.SinInOut);

                // Luego reinicio sin animación visible
                bubble.Scale = originalScale;
                bubble.TranslationY = 0;
                bubble.Rotation = 0;

                // Delay aleatorio antes del siguiente ciclo
                await Task.Delay(rnd.Next(2500, 4500));
            }
        }

    }

}
