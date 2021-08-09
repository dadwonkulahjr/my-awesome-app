using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleEmpty.UI.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        [Route("Error/{statusCode}")]
        public IActionResult HandleHttpStatusCode(int statusCode)
        {
            var statusCodeResults = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "The resource you are looking for cannot be found at this moment. Please contact your application developer!";
                    _logger.LogWarning($"404 Error occured. Path = {statusCodeResults.OriginalPath}" +
                        $"The QueryStrings value = {statusCodeResults.OriginalQueryString}");
                    break;
                default:
                    break;
            }
            return View("NotFound");
        }
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionObj = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError($"The following path = {exceptionObj.Path} thrown an exception" +
                $"The exception error message = {exceptionObj.Error.Message} and the exception stack trace = {exceptionObj.Error.StackTrace}");
            return View("Error");
        }
    }
}
