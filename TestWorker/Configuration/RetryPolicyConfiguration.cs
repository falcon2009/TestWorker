namespace TestWorker.Configuration
{
    public interface IRetryPolicyConfiguration
    {
        int RetryCount { get; }
    }

    public class RetryPolicyConfiguration : IRetryPolicyConfiguration
    {
        public int RetryCount { get; set;  }
    }
}
