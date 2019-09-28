namespace Domain.Models
{
    public class CreateChannelResult
    {
        public CreateChannelResult(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; }
    }
}