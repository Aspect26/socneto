using System.Diagnostics;

namespace Socneto.DataAcquisition.Domain
{
    public class UniPost
    {
        private UniPost(string text, string source, string user)
        {
            Text = text;
            User = user;
            Source= source;
        }
        public string Text { get; set; }
        public string Source { get; set; }
        public string User { get; set; }

        public static UniPost FromValues(string text, string source, string user)
        {
            return new UniPost(text, source, user);
        }

    }
}
