using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class SoundCodeProfile : Profile
    {
        public SoundCodeProfile()
        {
            CreateMap<SoundCodeViewModel, SoundCode>();
            CreateMap<SoundCode, SoundCodeViewModel>();
        }
    }
}
