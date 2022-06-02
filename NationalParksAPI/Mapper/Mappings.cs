using AutoMapper;
using NationalParksAPI.Models;
using NationalParksAPI.Models.DTOs;

namespace NationalParksAPI.Mapper
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<NationalPark, NationalParkDTO>().ReverseMap();
            CreateMap<Trail, TrailDTO>().ReverseMap();
            CreateMap<Trail, TrailCreateDTO>().ReverseMap();
            CreateMap<Trail, TrailUpdateDTO>().ReverseMap();
        }
    }
}
