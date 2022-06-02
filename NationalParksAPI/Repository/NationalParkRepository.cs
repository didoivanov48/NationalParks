using NationalParksAPI.Data;
using NationalParksAPI.Models;
using NationalParksAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace NationalParksAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext dB;
        public NationalParkRepository(ApplicationDbContext db)
        {
            dB = db;
        }
        public bool CreateNationalPark(NationalPark nationalPark)
        {
            dB.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark nationalPark)
        {
            dB.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int nationalParkId)
        {
            return dB.NationalParks.FirstOrDefault(a => a.Id == nationalParkId);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return dB.NationalParks.OrderBy(a => a.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            bool value = dB.NationalParks.Any(a => a.Name.ToLower().Trim()==name.ToLower().Trim());
            return value;
        }

        public bool NationalParkExists(int id)
        {
            return dB.NationalParks.Any(a => a.Id == id);
        }

        public bool Save()
        {
            return dB.SaveChanges() >= 0 ? true : false;

        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            dB.NationalParks.Update(nationalPark);
            return Save();
        }
    }
}
