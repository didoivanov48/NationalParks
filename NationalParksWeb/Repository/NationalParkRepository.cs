using NationalParksWeb.Models;
using NationalParksWeb.Repository.IRepository;
using System.Net.Http;

namespace NationalParksWeb.Repository
{
    public class NationalParkRepository : Repository<NationalPark>, INationalParkRepository
    {
        private readonly IHttpClientFactory clientFactory;
        public NationalParkRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.clientFactory = clientFactory;
        }
    }
}
