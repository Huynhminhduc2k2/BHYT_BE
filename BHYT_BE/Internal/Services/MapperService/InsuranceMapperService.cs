using AutoMapper;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BHYT_BE.Internal.Services.MapperService
{
    public class InsuranceMapperService : Profile
    {
        public InsuranceMapperService()
        {
            CreateMap<Insurance, InsuranceDTO>();
            CreateMap<InsuranceHistory, InsuranceHistoryDTO>();
            CreateMap<InsurancePaymentHistory, InsurancePaymentHistoryDTO>();


            CreateMap<InsuranceDTO, InsuranceResponse>()
                .ForMember(dest => dest.Type, opt => opt.ToString());
            CreateMap<InsurancePaymentHistoryDTO, InsurancePaymentHistoryResponse>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.ToString());
            CreateMap<InsuranceHistoryDTO, InsuranceHistoryResponse>()
                .ForMember(dest => dest.NewStatus, opt => opt.ToString())
                .ForMember(dest => dest.OldStatus, opt => opt.ToString());


        }
    }
    public class EnumToStringConverter : IValueConverter<InsurancePaymentMethod, string>
    {
        public string Convert(InsurancePaymentMethod source, ResolutionContext context)
        {
            return source.ToString();
        }
    }
}
