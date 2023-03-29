using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class ApiUserProfile : Profile
    {
        public ApiUserProfile()
        {
            CreateMap<ApiUserViewModel, ApiUser>();
            CreateMap<ApiUser, ApiUserViewModel>();
        }
    }
}
