using Domain;
using LinqToTwitter;

namespace SAnalyser.DataAcquisition.Infrastructure
{
    public static class StatusExt
    {
        public static UniPost ToUnifiedPost(this Status status)
        {
            return UniPost.FromValues(status.Text);
        }
    }
}