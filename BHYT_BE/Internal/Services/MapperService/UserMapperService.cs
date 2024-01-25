using AutoMapper;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Identity;

namespace BHYT_BE.Internal.Services.MapperService
{
    public class UserMapperService : Profile
    {
        public UserMapperService()
        {
            CreateMap<UserDTO, UserInfoResponse>();
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}
