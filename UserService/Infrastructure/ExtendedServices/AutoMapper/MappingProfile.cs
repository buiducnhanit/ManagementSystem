using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using AutoMapper;

namespace Infrastructure.ExtendedServices.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserRequest, User>();

            CreateMap<User, UserProfile>();
        }
    }
}
