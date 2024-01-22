using AutoMapper;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;
using BHYT_BE.Internal.Services.UserService;

namespace BHYT_BE.Internal.Services.MapperService
{
    public class InsuranceMapperService : Profile
    {
        public InsuranceMapperService()
        {
            CreateMap<Insurance, InsuranceDTO>();
        }
    }
}
