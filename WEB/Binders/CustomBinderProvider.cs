using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Binders
{
    /// <summary>
    /// Custom binder provider for custom  <c>DateTime</c> binding
    /// </summary>
    public class CustomBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //check if a property type is DateTime or DateTime? and return custom date binder
            if (context.Metadata.ModelType == typeof(DateTime?) || context.Metadata.ModelType == typeof(DateTime))
            {
                return new CustomDateBinder();
            }

            return null;
        }
    }
}
