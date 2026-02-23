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
            CreateMap<UserEntity, UserDTO>();
            CreateMap<UserEntityResponse, UserDTOResponse>();

            // Envelope Maps
            CreateMap<GetEnvelopeEntity, GetEnvelopeDTO>();
            CreateMap<GetEnvelopeEntityResponse, GetEnvelopeDTOResponse>();

            // Bank Account Maps
            CreateMap<GetBankAccountEntity, GetBankAccountDTO>();
            CreateMap<GetBankAccountEntityResponse, GetBankAccountDTOResponse>();

            // Transaction Maps
            CreateMap<SummaryTransactionEntityModel, SummaryTransactionDTOModel>();
            CreateMap<GetAllTransactionsEntityResponse, GetAllTransactionsDTOResponse>();

            // Transfer transaction Maps
            CreateMap<GetTransferTransactionEntityResponse, GetTransferTransactionDTOResponse>();
            CreateMap<GetTransferTransactionEntityModel, GetTransferTransactionDTOModel>();
        }
    }
}
