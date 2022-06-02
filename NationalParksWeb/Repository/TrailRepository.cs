using NationalParksWeb.Models;
using NationalParksWeb.Repository.IRepository;
using System.Net.Http;

namespace NationalParksWeb.Repository
{
    public class TrailRepository : Repository<Trail>, ITrailRepository
    {
        private readonly IHttpClientFactory clientFactory;
        public TrailRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.clientFactory = clientFactory;
        }
    }
}

