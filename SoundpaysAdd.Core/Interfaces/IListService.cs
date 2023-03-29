using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface IListService
    {
        Task<List<SelectListItem>> GetAllCampaignsAsync();
        Task<List<SelectListItem>> GetAllSoundCodeAsync();
        Task<List<SelectListItem>> GetAllCampaignCodeAsync();
        Task<MultiSelectList> GetAllSegmentMultiAsync(int[] selected);
        List<SelectListItem> GetAllAddType();
    }
}
