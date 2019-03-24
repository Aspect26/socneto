using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace SAnalyser.DataAcquisition.Domain
{
    public class StuffDoer
    {
        private readonly ISocialNetwork _socialNetwork;

        public StuffDoer(ISocialNetwork socialNetwork)
        {
            _socialNetwork = socialNetwork;
        }
        public async Task<IList<UniPost>> DoSomeRealWork(string searchTerm)
        {
            return await _socialNetwork.SearchAsync(searchTerm);
        }
    }

    public interface ISocialNetwork
    {
        Task<IList<UniPost>> SearchAsync(string searchTerm);
    }
}
