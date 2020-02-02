using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.EventTracking
{

    public interface IEventTracker<T>
    {
        void TrackStatistics(string eventName, object serializableAttributes = null);
        void TrackInfo(string eventName, string message, object serializableAttributes = null);
        void TrackWarning(string eventName, string message, object serializableAttributes=null);
        void TrackError(string eventName, string message, object serializableAttributes = null);
        void TrackFatal(string eventName, string message, object serializableAttributes = null);
    }

}
