
using System.Collections.Generic;
using NationalParksAPI.Models;

namespace TrailsAPI.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrails();
        ICollection<Trail> GetTrailsInNationalPark(int npId);
        Trail GetTrail(int trailId);

        bool TrailExists(string name);
        bool TrailExists(int id);

        bool CreateTrail(Trail trail);
        bool UpdateTrail(Trail trail);

        bool DeleteTrail(Trail trail);

        bool Save();
    }
}
