using System.Windows;

namespace StudioPet
{
    public class ImageShellConfiguration
    {
        public double Width { get; set; } = 200;
        public double Height { get; set; } = 200;
        public double Opacity { get; set; } = 0.4;
        public double Margin { get; set; } = 10;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Right;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Bottom;

        public string StudioPetFolder { get; set; }
    }
}
