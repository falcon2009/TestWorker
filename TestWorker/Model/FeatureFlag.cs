namespace TestWorker.Model
{
    public interface IFeatureFlag
    {
        bool IsAvailable { get; }
    }

    public class EncryptorFeatureFlag : IFeatureFlag
    {
        public bool IsAvailable => true;
    }

    public class DecryptorFeatureFlag : IFeatureFlag
    {
        public bool IsAvailable => false;
    }

    public class ConverterFeatureFlag : IFeatureFlag
    {
        public bool IsAvailable => true;
    }
}
