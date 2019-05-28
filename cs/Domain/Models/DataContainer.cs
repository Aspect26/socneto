using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class DataContainer
    {

        public IList<UserData> UserDataList { get; set; }
        public IList<PostData> PostDataList { get; set; }

    }
}