/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;

namespace MakesCentsBackend.Services.Mappers
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Default constructor for the User Mapping Profile
        /// </summary>
        public MappingProfile()
        {
            // User Maps
            CreateMap<UserEntityModel, UserDTOModel>();
            CreateMap<UserEntityResponse, UserDTOResponse>();

            // Sinking Fund Envelope Maps
            CreateMap<GetSinkingFundEnvelopeEntityModel, GetSinkingFundEnvelopeDTOModel>();
            CreateMap<GetSinkingFundEnvelopeEntityResponse, GetSinkingFundEnvelopeDTOResponse>()
                .ForMember(dest => dest.SinkingFundEnvelopeDTO, opt => opt.MapFrom(src => src.SinkingFundEnvelope));

            // Rollover Envelope Maps
            CreateMap<GetRolloverEnvelopeEntityModel, GetRolloverEnvelopeDTOModel>();
            CreateMap<GetRolloverEnvelopeEntityResponse, GetRolloverEnvelopeDTOResponse>()
                .ForMember(dest => dest.RolloverEnvelopeDTO, opt => opt.MapFrom(src => src.RolloverEnvelope));

            // Bank Account Maps
            CreateMap<GetBankAccountEntityModel, GetBankAccountDTOModel>();
            CreateMap<GetBankAccountEntityResponse, GetBankAccountDTOResponse>();

            // Debt Account Maps
            CreateMap<GetDebtAccountEntityModel, GetDebtAccountDTOModel>();
            CreateMap<GetDebtAccountEntityResponse, GetDebtAccountDTOResponse>();

            // Investment Account Maps
            CreateMap<GetInvestmentAccountEntityModel, GetInvestmentAccountDTOModel>();
            CreateMap<GetInvestmentAccountEntityResponse, GetInvestmentAccountDTOResponse>();

            // Transaction Maps
            CreateMap<SummaryTransactionEntityModel, SummaryTransactionDTOModel>()
                .ConvertUsing(src => TransactionMapper.ToSummaryResponse(src));
            CreateMap<GetAllTransactionsEntityResponse, GetAllTransactionsDTOResponse>();

            // Transfer transaction Maps
            CreateMap<GetTransferTransactionEntityResponse, GetTransferTransactionDTOResponse>();
            CreateMap<GetTransferTransactionEntityModel, GetTransferTransactionDTOModel>();
        }
    }
}
