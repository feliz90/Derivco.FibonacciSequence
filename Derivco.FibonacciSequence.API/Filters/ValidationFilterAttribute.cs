using System.Linq;
using Derivco.FibonacciSequence.API.Models.FibonacciSequence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Derivco.FibonacciSequence.API.Filters
{
    public class RequestValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = (FibonacciSequenceRequest)context.ActionArguments.Values.FirstOrDefault(a => a.GetType() == typeof(FibonacciSequenceRequest));

            if (request is null)
            {
                context.Result = new BadRequestObjectResult("Request is null. Something went wrong");
                return;
            }
            
            if (request.FirstIndex >= request.LastIndex)  
            {
                context.Result = new BadRequestObjectResult("The First index greater or equal than last. Change the order of indexes.");
                return;
            }
            
            
            if(!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}