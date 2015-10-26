using System.ServiceModel;
using System.ServiceModel.Channels;
using Tridion.ContentManager.CoreService.Client;

namespace SDL.TridionVSRazorExtension
{
    public class LocalCoreServiceClient : CoreServiceClient, ILocalClient
    {
        public LocalCoreServiceClient() : base()
        {
        }

        public LocalCoreServiceClient(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        {
        }
    }
}
