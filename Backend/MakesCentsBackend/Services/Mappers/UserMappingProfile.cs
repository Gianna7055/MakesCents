/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;

namespace MakesCentsBackend.Services.Mappers
{
    public class UserMappingProfile : Profile
    {
        /// <summary>
        /// Default constructor for the User Mapping Profile
        /// </summary>
        public UserMappingProfile()
        {
            CreateMap<UserEntity, UserDTO>();
            CreateMap<UserEntityResponse, UserDTOResponse>();
        }
    }
}
