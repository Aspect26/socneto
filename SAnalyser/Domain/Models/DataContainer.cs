using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class DataContainer
    {

        public IList<UserData> UserDataList { get; set; }
        public IList<PostData> PostDataList { get; set; }

    }
}
