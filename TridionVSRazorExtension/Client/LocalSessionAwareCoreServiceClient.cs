using System.ServiceModel;
using System.ServiceModel.Channels;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension.Client
{
    public class LocalSessionAwareCoreServiceClient : SessionAwareCoreServiceClient, ICoreService
    {
        public LocalSessionAwareCoreServiceClient() : base()
        {
        }

        public LocalSessionAwareCoreServiceClient(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        {
        }
    }
}
