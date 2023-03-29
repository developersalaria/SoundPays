using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Identity.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Enums;

namespace SoundpaysAdd.UI.Pages.Account.Advertiser
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        #region Properties
        private readonly IAdvertiserAsync _advertiser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IApiUserAsync _apiUser;
        [BindProperty]
        public bool IsAdmin => _currentUserService.IsSuperAdmin;
        #endregion
        #region Ctor
        public IndexModel(
            IAdvertiserAsync advertiser,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IApiUserAsync apiUser,
            IMapper mapper
        )
        {
            _advertiser = advertiser;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _apiUser = apiUser;
            _mapper = mapper;
        }
        #endregion

        public void OnGet()
        {
            ViewData["Title"] = "Advertiser";
        }

        /// <summary>
        /// On Get All Advertiser 
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        #region Methods
        public async Task<JsonResult> OnGetAllAdvertiserDataTableAsync(jQueryDataTableParamModel parms)
        {

            parms.IsSuperAdmin = _currentUserService.IsSuperAdmin;
            parms.OwnerId = _currentUserService.AdvertiserId.ToString();
            var response = await _advertiser.GetAllDatatableAsync(parms);
            return new JsonResult(new
            {
                aaData = response.Item1,
                iTotalRecords = response.Item3,
                iTotalDisplayRecords = response.Item3
            });
        }

        /// <summary>
        /// On Get  Advertiser ById
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrEditAsync(int id = 0, bool readOnly = false)
        {
            AdvertiserViewModel advertiserViewModel = new AdvertiserViewModel();
            ViewData["IsReadOnly"] = readOnly;
            ViewData["Title"] = (id > 0) ? "Edit Advertiser" : "Add Advertiser";
            try
            {
                if (id > 0)
                {
                    var advertiser = await _advertiser.GetByIdAsync(id);
                    if (advertiser != null)
                    {
                        advertiserViewModel = _mapper.Map<AdvertiserViewModel>(advertiser);
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "_CreateOrEdit",
                    ViewData = new ViewDataDictionary<AdvertiserViewModel>(ViewData, advertiserViewModel)
                };
            }
            catch (Exception ex)
            {
                return new PartialViewResult
                {
                    ViewName = "_CreateOrEdit",
                    ViewData = new ViewDataDictionary<AdvertiserViewModel>(ViewData, advertiserViewModel)
                };
            }
        }
        /// <summary>
        /// Create Or Edit Advertiser
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrEditAsync(AdvertiserViewModel advertiserViewModel)
        {
            try
            {
                if (advertiserViewModel.Id > 0)
                {
                    ModelState.Remove("Password");
                }

                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }
                if (await _advertiser.IsDuplicateEmailAsync(advertiserViewModel.Email, id: advertiserViewModel.Id) ||
                    await _userManager.FindByEmailAsync(advertiserViewModel.Email) is not null)
                {
                    return new JsonResult(new { success = false, message = Constants.EmailAlreadyExist });
                }

                bool isNewUser = advertiserViewModel.Id <= 0;
                // identity user add or updated
                var addIdentityResult = isNewUser ? await AddIdentityUser(advertiserViewModel) : await UpdateIdentityUser(advertiserViewModel);
                // adding advertiser 
                if (addIdentityResult.Succeeded && addIdentityResult.Data && isNewUser)
                {
                    advertiserViewModel.UserId = Guid.NewGuid().ToString();
                    advertiserViewModel.IsActive = false;
                    advertiserViewModel.IsPaused = false;
                    advertiserViewModel.IsDeleted = false;
                    advertiserViewModel.CreatedBy = _currentUserService.UserId ?? "";
                    await _advertiser.AddAsync(_mapper.Map<Core.Models.Advertiser>(advertiserViewModel));
                }
                else
                {
                    var entityAdvertiser = await _advertiser.GetByIdAsync(advertiserViewModel.Id);
                    if (entityAdvertiser != null)
                    {
                        entityAdvertiser.ShortName = advertiserViewModel.ShortName;
                        entityAdvertiser.LongName = advertiserViewModel.LongName;
                        entityAdvertiser.Email = advertiserViewModel.Email;
                        entityAdvertiser.ModifiedOn = DateTime.UtcNow;
                        entityAdvertiser.ModifiedBy = _currentUserService.UserId ?? "";
                        await _advertiser.UpdateAsync(entityAdvertiser);
                    }
                }
                return new JsonResult(new { success = true, message = isNewUser ? "Advertiser added" : "Advertiser updated" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { sucess = false, message = Constants.SomeThingWrong });

            }
        }

        /// <summary>
        /// Delete Adviser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var adevertiser = await _advertiser.GetByIdAsync(id);
                if (adevertiser == null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                var adevertiserIdentity = await _userManager.FindByIdAsync(adevertiser.UserId);
                if (adevertiserIdentity == null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                await _advertiser.DeleteAsync(adevertiser);
                await _userManager.DeleteAsync(adevertiserIdentity);

                return new JsonResult(new { success = true, message = Constants.DeleteSuccess });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Activate Advertiser ById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostActivateAsync(int id)
        {
            try
            {
                var advertiser = await _advertiser.GetByIdAsync(id);
                if (advertiser is null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                var resp = await _advertiser.ActivateAsync(id);
                if (resp.Succeeded) await LockoutUserAsync(advertiser.UserId, false);

                return new JsonResult(new { success = resp.Data, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Dectivate Advertiser By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeactivateAsync(int id)
        {
            try
            {
                var advertiser = await _advertiser.GetByIdAsync(id);
                if (advertiser is null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                var resp = await _advertiser.ActivateAsync(id, false);
                if (resp.Succeeded) await LockoutUserAsync(advertiser.UserId);

                return new JsonResult(new { success = resp.Data, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Pause Advertiser By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostPauseAsync(int id)
        {
            try
            {
                var advertiser = await _advertiser.GetByIdAsync(id);
                if (advertiser is null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                var resp = await _advertiser.PauseAsync(id);
                if (resp.Succeeded) await LockoutUserAsync(advertiser.UserId);

                return new JsonResult(new { success = resp.Data, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Resume Advertiser By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostResumeAsync(int id)
        {
            try
            {
                var advertiser = await _advertiser.GetByIdAsync(id);
                if (advertiser is null) return new JsonResult(new { success = false, message = Constants.AdvertiserNotExists });

                var resp = await _advertiser.PauseAsync(id, false);
                if (resp.Succeeded) await LockoutUserAsync(advertiser.UserId, false);

                return new JsonResult(new { success = resp.Data, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        #endregion


        #region Identity
        /// <summary>
        /// Add new identity user when a new advertiser add operation is perform
        /// </summary>
        /// <param name="advertiserViewModel"></param>
        /// <returns></returns>
        private async Task<Core.Wrappers.Response<bool>> AddIdentityUser(AdvertiserViewModel advertiserViewModel)
        {
            try
            {
                var user = new ApplicationUser
                {

                    Email = advertiserViewModel.Email,
                    UserName = advertiserViewModel.Email,
                    TwoFactorEnabled = false,
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(user, advertiserViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Advertiser.ToString());
                    advertiserViewModel.UserId = user.Id;
                    return new Core.Wrappers.Response<bool>(true);
                }
                else
                {
                    return new Core.Wrappers.Response<bool>(false, result.Errors.First().Description);
                }
            }
            catch (Exception ex)
            {
                return new Core.Wrappers.Response<bool>(false, Constants.SomeThingWrong);
            }
        }
        /// <summary>
        /// Update identity user when a advertiser update operation is perform
        /// </summary>
        /// <param name="advertiserViewModel"></param>
        /// <returns></returns>

        private async Task<Core.Wrappers.Response<bool>> UpdateIdentityUser(AdvertiserViewModel advertiserViewModel)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(advertiserViewModel.UserId);
                if (user != null)
                {
                    user.Email = advertiserViewModel.Email;
                    user.UserName = advertiserViewModel.Email;
                    var result = await _userManager.UpdateAsync(user);
                }
                return new Core.Wrappers.Response<bool>(true);

            }
            catch (Exception ex)
            {
                return new Core.Wrappers.Response<bool>(false, Constants.SomeThingWrong);
            }

        }


        #region API keys
        /// <summary>
        /// Create Or View Advertiser API
        /// </summary>
        /// <param name="id">Advertiser id</param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrViewAPIAsync(int id)
        {
            ApiUserViewModel apiUserViewModel = new ApiUserViewModel
            {
                AdvertiserId = id,
            };
            try
            {
                ViewData["IsKeysGenrated"] = false;
                if (id > 0)
                {
                    var apiUser = await _apiUser.GetKeyByUserIdAsync(id);
                    if (apiUser != null)
                    {
                        apiUserViewModel = _mapper.Map<ApiUserViewModel>(apiUser);
                        ViewData["IsKeysGenrated"] = true;
                    }
                }
                return new PartialViewResult
                {
                    ViewName = "_ApiKey",
                    ViewData = new ViewDataDictionary<ApiUserViewModel>(ViewData, apiUserViewModel)
                };
            }
            catch (Exception ex)
            {
                return new PartialViewResult
                {
                    ViewName = "_ApiKey",
                    ViewData = new ViewDataDictionary<ApiUserViewModel>(ViewData, apiUserViewModel)
                };
            }
        }

        /// <summary>
        /// Create Or View Advertiser API
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Advertisers"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrViewAPIAsync(ApiUserViewModel apiUserViewModel)
        {
            try
            {
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }

                bool isNewApiKey = apiUserViewModel.Id <= 0;
                if (isNewApiKey)
                {
                    apiUserViewModel.IsActive = false;
                    apiUserViewModel.IsDeleted = false;
                    apiUserViewModel.CreatedBy = _currentUserService.UserId ?? "";
                    apiUserViewModel.ClientId = PasswordGenerator.GenerateRandomPassword();
                    apiUserViewModel.ClientSecret = PasswordGenerator.GenerateRandomPassword();
                    apiUserViewModel.ApiKey = PasswordGenerator.GenerateRandomPassword();

                    //secret:clientSecret+apikey:ApiKey //here small apikey is a word
                    string formattedClientKey = $"secret:{apiUserViewModel.ClientSecret}apikey:{apiUserViewModel.ApiKey}";
                    apiUserViewModel.ClientKey = DataEncryption.EncryptSHA512(formattedClientKey);
                    await _apiUser.AddAsync(_mapper.Map<ApiUser>(apiUserViewModel));
                }

                return new JsonResult(new { success = true, message = Constants.ActionSuccess, data = apiUserViewModel });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { sucess = false, message = Constants.SomeThingWrong });
            }
        }

        #endregion

        #endregion

        #region private methods
        /// <summary>
        ///  Lock Unlock User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<bool> LockoutUserAsync(string userId, bool lockAcc = true)
        {
            bool status = true;
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    DateTime? date = lockAcc ? DateTime.Now.AddYears(100) : null;
                    user.LockoutEnabled = lockAcc;
                    user.LockoutEnd = date;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        #endregion

    }
}
