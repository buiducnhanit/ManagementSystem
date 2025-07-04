using AutoMapper;
using ManagementSystem.Shared.Contracts;
using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Configure
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<CreateUserRequest, User>();
            CreateMap<User, UserProfile>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<UserRegisteredEvent, CreateUserRequest>();
            CreateMap<UpdateUserProfileEvent, UpdateUserRequest>();
            CreateMap<UserProfile, UpdateUserRequest>();
        }
    }
}
