using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Domain
{
    public class RequestListenerHostedService:IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class RequestListener: IRequestListener
    {

    }

    public interface IRequestListener
    {

    }

    public class RegistrationRequestMessage
    {

    }
}
