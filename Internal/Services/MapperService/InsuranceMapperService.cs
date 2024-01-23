using AutoMapper;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;

namespace BHYT_BE.Internal.Services.MapperService
{
    public class InsuranceMapperService : Profile
    {
        public InsuranceMapperService()
        {
            CreateMap<Insurance, InsuranceDTO>();
            CreateMap<InsuranceHistory, InsuranceHistoryDTO>();
            CreateMap<InsurancePaymentHistory, InsurancePaymentHistoryDTO>();


            CreateMap<InsuranceDTO, InsuranceResponse>();
            CreateMap<InsurancePaymentHistoryDTO, InsurancePaymentHistoryResponse>();
            CreateMap<InsuranceHistoryDTO, InsuranceHistoryResponse>();


        }
    }
}
