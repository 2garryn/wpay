using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.RequestService
{
    public class RequestServiceDefinition: IServiceDefinition
    {
        public string Label() => "RequestService";
        public void Configure(IConfigurator conf)
        {
            conf.Command<ResponseServiceDefinition, CreateCommand>();
            conf.Command<ResponseServiceDefinition, UpdateCommand>();
            conf.ConsumeEvent<ResponseServiceDefinition, CreatedEvent>();
            conf.ConsumeEvent<ResponseServiceDefinition, UpdatedEvent>("myflag" );
            conf.ConsumeEvent<ResponseServiceDefinition, CreateError>();
        }
    }
}