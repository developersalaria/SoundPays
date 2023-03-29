using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class CampaignProfile : Profile
    {
        public CampaignProfile()
        {
            CreateMap<CampaignViewModel, Campaign>();
            CreateMap<Campaign, CampaignViewModel>();
        }
    }
}
