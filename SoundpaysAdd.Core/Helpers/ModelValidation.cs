using Microsoft.AspNetCore.Mvc.ModelBinding;
using SoundpaysAdd.Core.Wrappers;

namespace SoundpaysAdd.Core.Helpers
{
    public static class ModelValidation
    {
        public static Response<bool> Check(ModelStateDictionary modelState)
        {
            modelState.Remove("IsDeleted");
            modelState.Remove("ModifiedBy");
            modelState.Remove("ModifiedOn");
            modelState.Remove("CreatedBy");
            modelState.Remove("CreatedOn");

            if (modelState.IsValid)
                return new Response<bool>(succeeded: true, message: "Valid Form");

            var errors = modelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            int errCount = 1;
            string message = "";
            foreach (var error in errors)
            {
                message += $"{errCount}. {error?.FirstOrDefault()?.ErrorMessage} </br>";
                errCount++;
            }
            return new Response<bool>(succeeded: false, message: message);

        }
    }
}
