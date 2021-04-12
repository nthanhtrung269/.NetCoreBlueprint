using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern
{
    /// <summary>
    /// The DisableFormValueModelBindingAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    {
        /// <summary>
        /// The OnResourceExecuting.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var formValueProviderFactory = context.ValueProviderFactories
                .OfType<FormValueProviderFactory>()
                .FirstOrDefault();
            if (formValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(formValueProviderFactory);
            }

            var jqueryFormValueProviderFactory = context.ValueProviderFactories
                .OfType<JQueryFormValueProviderFactory>()
                .FirstOrDefault();
            if (jqueryFormValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(jqueryFormValueProviderFactory);
            }
        }

        /// <summary>
        /// The OnResourceExecuted.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Do nothing
        }
    }
}