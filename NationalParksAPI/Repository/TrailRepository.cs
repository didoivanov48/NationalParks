
using TrailsAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using NationalParksAPI.Repository.IRepository;
using NationalParksAPI.Data;
using NationalParksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TrailsAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext dB;
        public TrailRepository(ApplicationDbContext db)
        {
            dB = db;
        }
        public bool CreateTrail(Trail trail)
        {
            dB.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            dB.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return dB.Trails.Include(c => c.NationalPark).FirstOrDefault(a => a.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return dB.Trails.Include(c => c.NationalPark).OrderBy(a => a.Name).ToList();
        }

        public bool TrailExists(string name)
        {
            bool value = dB.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int id)
        {
            return dB.Trails.Any(a => a.Id == id);
        }

        public bool Save()
        {
            return dB.SaveChanges() >= 0 ? true : false;

        }

        public bool UpdateTrail(Trail trail)
        {
            dB.Trails.Update(trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return dB.Trails.Include(c => c.NationalPark).Where(c => c.NationalParkId == npId).ToList();
        }
    }
}
