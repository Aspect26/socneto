using System.Collections.Generic;

namespace Infrastructure.StaticData
{
    public interface IStaticDataProvider
    {
        IEnumerator<UniPostStaticData> GetEnumerator();
    }

}
