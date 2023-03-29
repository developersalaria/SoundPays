using Microsoft.AspNetCore.Mvc;
using SoundpaysAdd.Core.Interfaces;

namespace SoundpaysAdd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUserController : ControllerBase
    {
        #region Properties
        private readonly IApiUserAsync _apiUserRepository;
        #endregion


        #region Ctor
        public ApiUserController(IApiUserAsync apiUserRepository)
        {
            _apiUserRepository = apiUserRepository;
        }
        #endregion


        #region Methods

        /// <summary>
        /// Get All ApiUser
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetApiUserList()
        {

            return Ok(await _apiUserRepository.GetAllAsync());
        }

        [HttpGet]
        [Route("getById")]
        public async Task<ActionResult> GetApiUserById(int Id)
        {
            return Ok();
        }
        #endregion 
    }
}

