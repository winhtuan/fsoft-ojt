using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;

namespace Plantpedia.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorModel() { }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error != null)
            {
                var exception = exceptionHandlerPathFeature.Error;
                var originalPath = exceptionHandlerPathFeature.Path;
                string errorMessage =
                    $"Unhandled exception for Request ID {RequestId} at path {originalPath}";
                LoggerHelper.Error(exception, errorMessage);
            }
            else
            {
                LoggerHelper.Warn(
                    $"The Error page was accessed directly without an exception for Request ID {RequestId}."
                );
            }
        }
    }
}
