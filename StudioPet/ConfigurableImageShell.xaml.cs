#pragma warning disable VSTHRD100 // Avoid async void methods

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace StudioPet
{
    public partial class ConfigurableImageShell : IImageShell
    {
        public ImageShellConfiguration Configuration { get; }

        private readonly DispatcherTimer _emotionTimer;
        private readonly Random _random = new Random();
        private readonly Dictionary<Emotion, BitmapImage> _images = new Dictionary<Emotion, BitmapImage>();

        private Action ExpressEmotionAfterBlinking;

        public ConfigurableImageShell(ImageShellConfiguration configuration)
        {
            InitializeComponent();

            Configuration = configuration;

            Opacity = Configuration.Opacity;
            Width = Configuration.Width;
            Height = Configuration.Height;

            _images.Add(Emotion.Normal, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Normal.png"), UriKind.Absolute)));
            _images.Add(Emotion.Blink1, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Blink1.png"), UriKind.Absolute)));
            _images.Add(Emotion.Blink2, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Blink2.png"), UriKind.Absolute)));
            _images.Add(Emotion.Happy, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Happy.png"), UriKind.Absolute)));
            _images.Add(Emotion.Happy1, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Happy1.png"), UriKind.Absolute)));
            _images.Add(Emotion.Happy2, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Happy2.png"), UriKind.Absolute)));
            _images.Add(Emotion.Sad, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Sad.png"), UriKind.Absolute)));
            _images.Add(Emotion.Sad1, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Sad1.png"), UriKind.Absolute)));
            _images.Add(Emotion.Sad2, new BitmapImage(new Uri(Path.Combine(configuration.StudioPetFolder, "Sad2.png"), UriKind.Absolute)));

            TheImage.Source = _images[Emotion.Normal];

            _emotionTimer = new DispatcherTimer();
            _emotionTimer.Tick += EmotionTimer_Tick;

            RestartTimer();
        }

        private void RestartTimer()
        {
            _emotionTimer.Interval = TimeSpan.FromSeconds(_random.Next(2, 30));
            _emotionTimer.Start();
        }

        private void StopTimer()
        {
            _emotionTimer.Stop();
        }


        private async void EmotionTimer_Tick(object sender, EventArgs e)
        {
            await ExpressEmotionAsync();
        }

        public async void ExpressEmotion(Emotion emotion)
        {
            // While blinking, execute again after blinking is completed
            if (!_emotionTimer.IsEnabled) // blinking
            {                
                ExpressEmotionAfterBlinking = () => ExpressEmotion(emotion);
                return;
            }

            ExpressEmotionAfterBlinking = null;

            StopTimer();

            switch (emotion)
            {
                case Emotion.Happy:
                    await ShowEmotionAsync(new[] {Emotion.Happy, Emotion.Happy1, Emotion.Happy2}, 100);
                    break;
                case Emotion.Sad:
                    await ShowEmotionAsync(new[] {Emotion.Sad, Emotion.Sad1, Emotion.Sad2}, 100);
                    break;
            }

            RestartTimer();
        }
        
        public async Task ExpressEmotionAsync()
        {
            StopTimer();
            var blinked = false;

            if (_random.Next(0, 10) < 8)
            {
                // Blink
                blinked = true;
                await ShowEmotionAsync(new[] {Emotion.Blink1, Emotion.Blink2}, 100);
            }
            else
            {
                TheImage.Source = _images[Emotion.Normal];
            }

            RestartTimer();

            if (blinked)
            {
                ExpressEmotionAfterBlinking?.Invoke();
            }
        }

        private async Task ShowEmotionAsync(IEnumerable<Emotion> emotions, double delayInMs)
        {
            foreach (var emotion in emotions)
            {
                await Task
                    .Delay(TimeSpan.FromMilliseconds(delayInMs))
                    .ContinueWith(_ => TheImage.Source = _images[emotion], TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

    }
}

#pragma warning restore VSTHRD100 // Avoid async void methods
