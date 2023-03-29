using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;

namespace SoundpaysAdd.Services.Repositories
{
    public class AdvertiserRepositoryAsync : GenericRepositoryAsync<Advertiser>, IAdvertiserAsync
    {
        #region Properties
        private readonly DbSet<Advertiser> _advertisers;
        private readonly SoundpaysAddContext _context;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region CTor
        public AdvertiserRepositoryAsync(
            SoundpaysAddContext dbContext,
              ICurrentUserService currentUserService
            ) : base(dbContext)
        {
            _advertisers = dbContext.Set<Advertiser>();
            _context = dbContext;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Advertiser Data table 
        /// </summary>
        /// <param name="jQueryDataTableParamModel"></param>
        /// <returns></returns>
        public async Task<Tuple<List<AdvertiserViewModel>, string, int>> GetAllDatatableAsync(jQueryDataTableParamModel jQueryDataTableParamModel)
        {
            try
            {
                var sortColumnIndex = jQueryDataTableParamModel.iSortCol_0;
                var query = (from advertiser in _context.Advertisers
                             where advertiser.IsDeleted != true
                             select new AdvertiserViewModel
                             {
                                 Id = advertiser.Id,
                                 ShortName = advertiser.ShortName,
                                 LongName = advertiser.LongName,
                                 UserId = advertiser.UserId,
                                 Email = advertiser.Email,
                                 IsActive = advertiser.IsActive,
                                 IsDeleted = advertiser.IsDeleted,
                                 IsPaused = advertiser.IsPaused
                             });
                //Get total count
                var totalCount = query.Count();
                #region Searching
                if (!string.IsNullOrEmpty(jQueryDataTableParamModel.sSearch))
                {
                    var toSearch = jQueryDataTableParamModel.sSearch.ToLower();
                    query = query.Where(c => c.ShortName.ToLower().Contains(toSearch) ||
                    c.LongName.ToLower().Contains(toSearch) ||
                    c.Email.ToLower().Contains(toSearch));

                }
                #endregion
                #region  Sorting
                string sortOrder = jQueryDataTableParamModel.sSortDir_0;

                query = sortColumnIndex switch
                {
                    0 => sortOrder switch
                    {
                        "desc" => query.OrderByDescending(a => a.ShortName),
                        "asc" => query.OrderBy(a => a.ShortName),
                        _ => query.OrderBy(a => a.ShortName),
                    },
                    1 => sortOrder switch
                    {
                        "desc" => query.OrderByDescending(a => a.LongName),
                        "asc" => query.OrderBy(a => a.LongName),
                        _ => query.OrderBy(a => a.LongName)
                    },
                    2 => sortOrder switch
                    {
                        "desc" => query.OrderByDescending(a => a.Email),
                        "asc" => query.OrderBy(a => a.Email),
                        _ => query.OrderBy(a => a.Email)
                    },
                    _ => query.OrderBy(a => a.ShortName)
                };

                #endregion
                #region  Paging
                if (jQueryDataTableParamModel.iDisplayLength != -1)
                    query = query.Skip(jQueryDataTableParamModel.iDisplayStart).Take(jQueryDataTableParamModel.iDisplayLength);

                #endregion
                return new Tuple<List<AdvertiserViewModel>, string, int>(
                   await query.ToListAsync(),
                   jQueryDataTableParamModel.sEcho,
                    totalCount);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deactivate or Activae an Advertiser 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Core.Wrappers.Response<bool>> ActivateAsync(int id, bool activate = true)
        {
            try
            {
                var advertiser = await _context.Advertisers.FirstOrDefaultAsync(x => x.Id == id);
                if (advertiser is null) return new Core.Wrappers.Response<bool>(false, Core.Helpers.Constants.AdvertiserNotExists);

                advertiser.IsActive = activate;
                advertiser.ModifiedBy = _currentUserService.UserId;
                advertiser.ModifiedOn = DateTime.Now;
                bool status = _context.SaveChanges() > 0;

                return new Core.Wrappers.Response<bool>(status, status ? Core.Helpers.Constants.UpdateSuccess : Core.Helpers.Constants.SomeThingWrong);

            }
            catch (Exception ex)
            {
                return new Core.Wrappers.Response<bool>(false, Core.Helpers.Constants.SomeThingWrong);
            }
        }

        /// <summary>
        /// Pause or Resume an Advertiser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Core.Wrappers.Response<bool>> PauseAsync(int id, bool paused = true)
        {
            try
            {
                var advertiser = await _context.Advertisers.FirstOrDefaultAsync(a => a.Id == id);
                if (advertiser is null) return new Core.Wrappers.Response<bool>(false, Core.Helpers.Constants.AdvertiserNotExists);

                advertiser.IsPaused = paused;
                advertiser.ModifiedBy = _currentUserService.UserId;
                advertiser.ModifiedOn = DateTime.Now;
                bool status = _context.SaveChanges() > 0;

                return new Core.Wrappers.Response<bool>(status, status ? Core.Helpers.Constants.UpdateSuccess : Core.Helpers.Constants.SomeThingWrong);

            }
            catch (Exception ex)
            {
                return new Core.Wrappers.Response<bool>(false, Core.Helpers.Constants.SomeThingWrong);
            }

        }

        /// <summary>
        /// check if email already taken
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicateEmailAsync(string email, int? id = 0)
        {
            try
            {
                if (id != 0)
                {
                    return await _context.Advertisers.AnyAsync(x => x.Email == email && x.Id != id);
                }
                return await _context.Advertisers.AnyAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}


#endregion
