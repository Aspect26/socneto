using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class DataContainer
    {

        public IList<User> UserDataList { get; set; }
        public IList<Post> PostDataList { get; set; }

    }
}