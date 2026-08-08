using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IBrokersService
    {
        BrokerResponse FindById(int id);

        BrokerResponse FindByCompany(string company);

        IEnumerable<BrokerResponse> FindAll();

        BrokerResponse Add(AddBrokerRequest request);

        BrokerResponse Update(int id, UpdateBrokerRequest request);

        BrokerResponse Delete(int id);
    }
}
