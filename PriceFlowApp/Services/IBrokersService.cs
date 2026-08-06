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

        Broker Add(AddBrokerRequest request);

        BrokerResponse Update(int id, UpdateBrokerRequest request);

        Broker Delete(int id);
    }
}
