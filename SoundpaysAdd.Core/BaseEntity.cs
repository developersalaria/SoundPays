using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class BaseEntity : ISoftDelete
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        /// 

        [Key]
        public int Id { get; set; }
        // [JsonHelper]
        public DateTime? CreatedOn { get; set; }
        // [JsonHelper]
        public string? CreatedBy { get; set; }
        // [JsonHelper]
        public DateTime? ModifiedOn { get; set; }
        //[JsonHelper]
        public string? ModifiedBy { get; set; }
        //[JsonHelper]
        public bool IsDeleted { get; set; }


        //[JsonHelper]
        public bool IsActive { get; set; }

        /// <summary>
        /// Is transient
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }
        protected BaseEntity()
        {
            CreatedOn = DateTime.Now;
        }
    }

    public abstract class BaseModel : ISoftDelete
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        /// 

        //   [JsonHelper]
        public int Id { get; set; }
        //  [JsonHelper]
        public DateTime? CreatedOn { get; set; }
        //  [JsonHelper]
        public string CreatedBy { get; set; }
        //  [JsonHelper]
        public DateTime? ModifiedOn { get; set; }
        // [JsonHelper]
        public string ModifiedBy { get; set; }
        // [JsonHelper]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Is transient
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }
        protected BaseModel()
        {
            CreatedOn = DateTime.UtcNow;
        }
    }
}
