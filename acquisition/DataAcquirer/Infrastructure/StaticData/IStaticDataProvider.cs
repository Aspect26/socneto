using System.Collections.Generic;

namespace Infrastructure.StaticData
{
    public interface IStaticDataProvider
    {
        IEnumerable<UniPostStaticData> GetEnumerable();
    }

}
