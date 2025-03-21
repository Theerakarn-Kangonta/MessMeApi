using AutoMapper;
using MessMeApi.Entities.Dtos;
using MessMeApi.Entities.Models;

namespace MessMeApi.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
