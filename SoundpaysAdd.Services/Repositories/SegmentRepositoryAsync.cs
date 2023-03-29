using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Wrappers;
using SoundpaysAdd.Data;
using SoundpaysAdd.Core.Helpers;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace SoundpaysAdd.Services.Repositories
{
    public class SegmentRepositoryAsync : GenericRepositoryAsync<Segment>, ISegmentService
    {
        #region Properties
        private readonly DbSet<Segment> _segments;
        private readonly SoundpaysAddContext _context;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region CTor
        public SegmentRepositoryAsync(SoundpaysAddContext dbContext, ICurrentUserService currentUserService) : base(dbContext)
        {
            _segments = dbContext.Set<Segment>();
            _context = dbContext;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get All Datatable
        /// </summary>
        /// <param name="jQueryDataTableParamModel"></param>
        /// <returns></returns>
        public async Task<Tuple<List<SegmentViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel)
        {
            try
            {
                var sortColumnIndex = jQueryDataTableParamModel.iSortCol_0;
                var segmentList = (from segment in _context.Segments
                                   select new SegmentViewModel
                                   {
                                       Id = segment.Id,
                                       Name = segment.Name,
                                       Description = segment.Description,
                                       IsActive = segment.IsActive,
                                       IsDeleted = segment.IsDeleted,
                                       CreatedBy = segment.CreatedBy,
                                       ModifiedOn = segment.ModifiedOn,
                                       ModifiedBy = segment.ModifiedBy,
                                   });

                //Get total count
                var totalRecords = segmentList.Count();
                #region Searching
                if (!string.IsNullOrEmpty(jQueryDataTableParamModel.sSearch))
                {
                    var toSearch = jQueryDataTableParamModel.sSearch.ToLower();
                    segmentList = segmentList.Where(c => c.Name.ToLower().Contains(toSearch));

                }
                #endregion
                #region  Sorting
                string sortOrder = jQueryDataTableParamModel.sSortDir_0;
                segmentList = sortColumnIndex switch
                {
                    0 => sortOrder switch
                    {
                        "desc" => segmentList.OrderByDescending(a => a.Name),
                        "asc" => segmentList.OrderBy(a => a.Name),
                        _ => segmentList.OrderBy(a => a.Name),
                    },
                    1 => sortOrder switch
                    {
                        "desc" => segmentList.OrderByDescending(a => a.Description),
                        "asc" => segmentList.OrderBy(a => a.Description),
                        _ => segmentList.OrderBy(a => a.Description),
                    },
                };

                #endregion
                #region  Paging
                if (jQueryDataTableParamModel.iDisplayLength != -1)
                    segmentList = segmentList.Skip(jQueryDataTableParamModel.iDisplayStart).Take(jQueryDataTableParamModel.iDisplayLength);
                #endregion
                return new Tuple<List<SegmentViewModel>, string, int>(
                   await segmentList.ToListAsync(),
                   jQueryDataTableParamModel.sEcho,
                    totalRecords);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response<bool>> PauseResumeAsyncs(int id, bool pause)
        {
            try
            {
                var segment = await _context.Segments.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (segment is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                segment.ModifiedBy = _currentUserService.UserId;
                segment.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.SaveSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        public async Task<Response<bool>> DeActivateAsyncs(int id, bool activate)
        {
            try
            {
                var segment = await _context.Segments.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (segment is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                segment.IsActive = activate;
                segment.ModifiedBy = _currentUserService.UserId;
                segment.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.SaveSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        public async Task<bool> IsDuplicateameAsync(string name, int? id = 0)
        {
            try
            {
                if (id != 0)
                {
                    return await _context.Segments.AnyAsync(x => x.Name == name && x.Id != id);
                }
                return await _context.Segments.AnyAsync(x => x.Name == name);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
