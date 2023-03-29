using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Data
{
    public class SoundpaysAddContext : DbContext
    {
        public SoundpaysAddContext(DbContextOptions<SoundpaysAddContext> options) : base(options)
        {
            // this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Add> Adds { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AddAttachment> AddAttachments { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Advertiser> Advertisers { get; set; }
        public virtual DbSet<ApiUser> ApiUsers { get; set; }
        public virtual DbSet<ApiToken> ApiTokens { get; set; }
        public virtual DbSet<Segment> Segments { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignSegment> CampaignSegments { get; set; }
        public virtual DbSet<SoundCode> SoundCodes { get; set; }

    }

}
