using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IBrokersService
    {
        Brokeri? FindById(int id);

        Brokeri? FindByCompany(string company);

        IEnumerable<Broker> FindAll();

        IEnumerable<BrokerResponse> GetAll();

        Broker Add(CreateBrokerRequest request);

        BrokerResponse Update(UpdateBrokerRequest request);

        Broker Delete(int id);
    }
}
