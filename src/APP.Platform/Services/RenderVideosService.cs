using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Platform.Services
{
    public static class RenderVideosService
    {
        public static async Task<string> RenderVideos<TModel>(
            string viewName,
            TModel model,
            IRazorViewEngine _viewEngine,
            PageContext pageContext,
            ITempDataProvider _tempDataProvider
        )
        {
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(pageContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException(
                        $"{viewName} does not match any available view"
                    );
                }

                var viewDictionary = new ViewDataDictionary<TModel>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()
                )
                {
                    Model = model,
                };

                var viewContext = new ViewContext(
                    pageContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(pageContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                var renderedHtml = writer.GetStringBuilder().ToString();

                // Verificar se o HTML gerado está vazio
                if (string.IsNullOrWhiteSpace(renderedHtml))
                {
                    return "";
                }

                return renderedHtml;
            }
        }
    }
}
