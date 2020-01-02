using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace StudioPet
{
    public partial class ConfigurableImageShell : IImageShell, IDisposable
    {
        public ImageShellConfiguration Configuration { get; }

        private readonly DispatcherTimer _emotionTimer = new DispatcherTimer();
        private readonly Random _random = new Random();
        private readonly Dictionary<Emotion, List<BitmapImage>> _images = new Dictionary<Emotion, List<BitmapImage>>();

        private Action _expressEmotionAfterAnimation;

        public ConfigurableImageShell(ImageShellConfiguration configuration)
        {
            InitializeComponent();

            Configuration = configuration;

            Opacity = Configuration.Opacity;
            Width = Configuration.Width;
            Height = Configuration.Height;

            foreach (Emotion emotion in Enum.GetValues(typeof(Emotion)))
            {
                _images.Add(emotion, new List<BitmapImage>());

                foreach (var file in new DirectoryInfo(configuration.ImageFolder).GetFiles($"{emotion}*.png"))
                {
                    _images[emotion].Add(new BitmapImage(new Uri(file.FullName, UriKind.Absolute)));
                }
                
            }

            _emotionTimer.Tick += EmotionTimer_Tick;

            TheImage.Source = _images[Emotion.Normal][_random.Next(_images[Emotion.Normal].Count)];
            _emotionTimer.Interval = GetRandomTimeSpan(Emotion.Normal);
            
            _emotionTimer.Start();
        }

        
        public void Dispose()
        {
            _emotionTimer.Tick -= EmotionTimer_Tick;
        }


        private void EmotionTimer_Tick(object sender, EventArgs e)
        {
            ExpressEmotion(Emotion.Normal);
        }

        public void ExpressEmotion(Emotion emotion)
        {
            if (!_emotionTimer.IsEnabled)
            {
                _expressEmotionAfterAnimation = () => ExpressEmotion(emotion);
                return;
            }

            _expressEmotionAfterAnimation = null;

            _emotionTimer.Stop();

            TheImage.Source = _images[emotion][_random.Next(_images[emotion].Count)];
            _emotionTimer.Interval = GetRandomTimeSpan(emotion);

            _expressEmotionAfterAnimation?.Invoke();
            _emotionTimer.Start();
        }

        private TimeSpan GetRandomTimeSpan(Emotion emotion)
        {
            switch (emotion)
            {
                case Emotion.Happy:
                    return TimeSpan.FromSeconds(_random.Next(Configuration.ShowHappyMinSeconds, Configuration.ShowHappyMaxSeconds));
                case Emotion.Sad:
                    return TimeSpan.FromSeconds(_random.Next(Configuration.ShowSadMinSeconds, Configuration.ShowSadMaxSeconds));
                default:
                    return TimeSpan.FromSeconds(_random.Next(Configuration.ShowNormalMinSeconds, Configuration.ShowNormalMaxSeconds));
            }
        }
    }
}
