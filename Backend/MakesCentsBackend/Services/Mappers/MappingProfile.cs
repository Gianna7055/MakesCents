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

            // Envelope Maps
            CreateMap<GetEnvelopeEntityModel, GetEnvelopeDTOModel>();
            CreateMap<GetEnvelopeEntityResponse, GetEnvelopeDTOResponse>()
                .ForMember(dest => dest.EnvelopeDTO, opt => opt.MapFrom(src => src.EnvelopeEntity));

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
