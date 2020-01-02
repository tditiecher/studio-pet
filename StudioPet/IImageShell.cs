namespace StudioPet
{
    public interface IImageShell
    {
        ImageShellConfiguration Configuration { get; }

        void ExpressEmotion(Emotion emotion);
    }
}
