using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{

    public interface IEventTracker<T>
    {
        public void TrackStatistics(string eventName, object serializableAttributes = null);
        public void TrackInfo(string eventName, string message, object serializableAttributes = null);
        public void TrackWarning(string eventName, string message, object serializableAttributes=null);
        public void TrackError(string eventName, string message, object serializableAttributes = null);
        public void TrackFatal(string eventName, string message, object serializableAttributes = null);
    }

}
