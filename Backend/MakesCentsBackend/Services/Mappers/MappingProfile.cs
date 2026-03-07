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
            CreateMap<GetEnvelopeEntityResponse, GetEnvelopeDTOResponse>();

            // Bank Account Maps
            CreateMap<GetBankAccountEntityModel, GetBankAccountDTOModel>();
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
