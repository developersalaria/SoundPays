using AutoMapper;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.AutoMapperProfile
{
    public class AddProfile : Profile
    {
        public AddProfile()
        {
            CreateMap<AddViewModel, Add>();
            CreateMap<Add, AddViewModel>();
            CreateMap<AttachmentViewModel, Attachment>();
            CreateMap<Attachment, AttachmentViewModel>();
        }
    }
}
