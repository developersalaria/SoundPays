using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class AdvertiserProfile : Profile
    {
        public AdvertiserProfile()
        {
            CreateMap<AdvertiserViewModel, Advertiser>();
            CreateMap<Advertiser, AdvertiserViewModel>();
        }
    }
}
