using System.Windows;
using Newtonsoft.Json;

namespace StudioPet
{
    public class ImageShellConfiguration
    {
        public double Width { get; set; } = 120;
        public double Height { get; set; } = 120;
        public double Opacity { get; set; } = 0.4;
        public double Margin { get; set; } = 10;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Right;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;

        public int ShowHappyMinSeconds { get; set; } = 5;
        public int ShowHappyMaxSeconds { get; set; } = 5;
        public int ShowSadMinSeconds { get; set; } = 5;
        public int ShowSadMaxSeconds { get; set; } = 5;
        public int ShowNormalMinSeconds { get; set; } = 15;
        public int ShowNormalMaxSeconds { get; set; } = 60;

        public string PictureSet { get; set; } = "Default";

        [JsonIgnore]
        public string ImageFolder { get; set; }
    }
}
