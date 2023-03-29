using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class SegmentProfile : Profile
    {
        public SegmentProfile()
        {
            CreateMap<SegmentViewModel, Segment>();
            CreateMap<Segment, SegmentViewModel>();
        }
    }
}
