using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Wrappers;
using SoundpaysAdd.Data;
using SoundpaysAdd.Core.Helpers;
using System.Xml.Linq;

namespace SoundpaysAdd.Services.Repositories
{
    public class SoundCodeRepositoryAsync : GenericRepositoryAsync<SoundCode>, ISoundCodeService
    {
        #region Properties
        private readonly DbSet<SoundCode> SoundCodes;
        private readonly SoundpaysAddContext _context;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region CTor
        public SoundCodeRepositoryAsync(SoundpaysAddContext dbContext, ICurrentUserService currentUserService) : base(dbContext)
        {
            SoundCodes = dbContext.Set<SoundCode>();
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
        public async Task<Tuple<List<SoundCodeViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel)
        {
            try
            {
                var sortColumnIndex = jQueryDataTableParamModel.iSortCol_0;
                var soundCodeList = (from soundCode in _context.SoundCodes
                                     select new SoundCodeViewModel
                                     {
                                         Id = soundCode.Id,
                                         Code = soundCode.Code,
                                         StartZone = soundCode.StartZone,
                                         EndZone = soundCode.EndZone,
                                         IsActive = soundCode.IsActive,
                                         IsDeleted = soundCode.IsDeleted,
                                         IsPaused = soundCode.IsPaused,
                                         CreatedBy = soundCode.CreatedBy,
                                         ModifiedOn = soundCode.ModifiedOn,
                                         ModifiedBy = soundCode.ModifiedBy,
                                     });

                //Get total count
                var totalRecords = soundCodeList.Count();
                #region Searching
                if (!string.IsNullOrEmpty(jQueryDataTableParamModel.sSearch))
                {
                    var toSearch = jQueryDataTableParamModel.sSearch.ToLower();
                    soundCodeList = soundCodeList.Where(c => c.Code.ToLower().Contains(toSearch));

                }
                #endregion
                #region  Sorting
                string sortOrder = jQueryDataTableParamModel.sSortDir_0;
                soundCodeList = sortColumnIndex switch
                {
                    0 => sortOrder switch
                    {
                        "desc" => soundCodeList.OrderByDescending(a => a.Code),
                        "asc" => soundCodeList.OrderBy(a => a.Code),
                        _ => soundCodeList.OrderBy(a => a.Code),
                    },
                    1 => sortOrder switch
                    {
                        "desc" => soundCodeList.OrderByDescending(a => a.StartZone),
                        "asc" => soundCodeList.OrderBy(a => a.StartZone),
                        _ => soundCodeList.OrderBy(a => a.StartZone),
                    },
                    2 => sortOrder switch
                    {
                        "desc" => soundCodeList.OrderByDescending(a => a.EndZone),
                        "asc" => soundCodeList.OrderBy(a => a.EndZone),
                        _ => soundCodeList.OrderBy(a => a.EndZone),
                    },
                    
                };

                #endregion
                #region  Paging
                if (jQueryDataTableParamModel.iDisplayLength != -1)
                    soundCodeList = soundCodeList.Skip(jQueryDataTableParamModel.iDisplayStart).Take(jQueryDataTableParamModel.iDisplayLength);
                #endregion
                return new Tuple<List<SoundCodeViewModel>, string, int>(
                   await soundCodeList.ToListAsync(),
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
                var soundCode = await _context.SoundCodes.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (soundCode is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                soundCode.IsPaused = pause;
                soundCode.ModifiedBy = _currentUserService.UserId;
                soundCode.ModifiedOn = DateTime.Now;
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
                var soundCode = await _context.SoundCodes.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (soundCode is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                soundCode.IsActive = activate;
                soundCode.ModifiedBy = _currentUserService.UserId;
                soundCode.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.SaveSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }
        public async Task<bool> IsDuplicateCodeAsync(string code, int? id = 0)
        {
            try
            {
                if (id != 0)
                {
                    return await _context.SoundCodes.AnyAsync(x => x.Code == code && x.Id != id);
                }
                return await _context.SoundCodes.AnyAsync(x => x.Code == code);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
