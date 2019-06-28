using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Binders
{
    /// <summary>
    /// Custom date binder that converts dd-MM-yyyy formatted dates to DateTime
    /// </summary>
    public class CustomDateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if(string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            var result = DateTime.TryParseExact(value, "dd-MM-yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out DateTime resultingDate);
            bindingContext.Result = ModelBindingResult.Success(resultingDate);

            return Task.CompletedTask;
        }
    }
}
