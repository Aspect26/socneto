using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class UniPost
    {
        private UniPost(string text)
        {
            Text = text;
        }
        public string Text { get;  }

        public static UniPost FromValues(string text)
        {
            return new UniPost(text);
        }

    }
}
