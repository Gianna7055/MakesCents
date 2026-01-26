/*
 * Gianna Ross
 * File Created: 1/15/2026
 * File Last Updated: 1/15/2026
 * Makes Cents - Mapping Profile for AutoMapper
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;

namespace MakesCentsBackend.Services.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserEntity, UserDTO>();
            CreateMap<UserEntityResponse, UserDTOResponse>();

        }
    }
}
