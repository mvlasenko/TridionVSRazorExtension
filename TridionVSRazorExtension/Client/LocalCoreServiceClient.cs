using System.ServiceModel;
using System.ServiceModel.Channels;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension.Client
{
    public class LocalCoreServiceClient : CoreServiceClient, ICoreService
    {
        public LocalCoreServiceClient() : base()
        {
        }

        public LocalCoreServiceClient(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        {
        }
    }
}
