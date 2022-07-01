using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSN.Resa.Vns.Commons.ModelBinders
{
    // TODO: [Y.Mirzaie] In general, is this sorting method suitable for use in other projects?
    public class SortParameterCollectionModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var sortParameters = new List<SortParameter>();

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            foreach (string value in valueProviderResult.Values)
            {
                string[] segments = value.Split(':');
                if (segments.Length != 2 || segments.Any(P => string.IsNullOrWhiteSpace(P)))
                {
                    bindingContext.ModelState.TryAddModelError(nameof(SortParameter), "Invalid format.");
                    return Task.CompletedTask;
                }

                segments[1] = string.Equals(segments[1], "ASC", StringComparison.OrdinalIgnoreCase) ? "Ascending" :
                                            string.Equals(segments[1], "DESC", StringComparison.OrdinalIgnoreCase) ? "Descending" :
                                            segments[1];

                if (!Enum.TryParse(segments[1], out SortDirection direction))
                {
                    bindingContext.ModelState.TryAddModelError(nameof(SortDirection), "Invalid format.");
                    return Task.CompletedTask;
                }

                sortParameters.Add(new SortParameter()
                {
                    Key = segments[0],
                    Direction = direction
                });
            }

            bindingContext.Result = ModelBindingResult.Success(sortParameters);
            return Task.CompletedTask;
        }
    }
}