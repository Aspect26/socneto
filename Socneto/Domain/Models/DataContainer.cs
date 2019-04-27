using System.Collections.Generic;

namespace Socneto.Coordinator.Domain.Models
{
    public class DataContainer
    {

        public IList<UserData> UserDataList { get; set; }
        public IList<PostData> PostDataList { get; set; }

    }
}