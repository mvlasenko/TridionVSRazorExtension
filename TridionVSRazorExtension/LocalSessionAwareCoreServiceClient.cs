using System.ServiceModel;
using System.ServiceModel.Channels;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension
{
    public class LocalSessionAwareCoreServiceClient : SessionAwareCoreServiceClient, ILocalClient
    {
        public LocalSessionAwareCoreServiceClient() : base()
        {
        }

        public LocalSessionAwareCoreServiceClient(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        {
        }
    }
}
