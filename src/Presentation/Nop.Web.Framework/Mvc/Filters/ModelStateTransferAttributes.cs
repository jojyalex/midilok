using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Nop.Web.Framework.Mvc.Filters
{
    public partial class ModelStateTransferValue
    {
        public string Key { get; set; }
        public string AttemptedValue { get; set; }
        public object RawValue { get; set; }
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public partial class SetTempDataModelErrorsAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;

            var errorList = controller.ViewData.ModelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            controller.TempData["ModelStateErrors"] = JsonConvert.SerializeObject(errorList);

            await next();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public partial class RestoreModelErrorsFromTempDataAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            if (controller.TempData.TryGetValue("ModelStateErrors", out var value))
            {
                var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>((string)value);

                var modelState = new ModelStateDictionary();
                foreach (var item in errorList)
                {
                    modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                    foreach (var error in item.ErrorMessages)
                    {
                        modelState.AddModelError(item.Key, error);
                    }
                }

                controller.ViewData.ModelState.Merge(modelState);
            }

            await next();
        }
    }
}