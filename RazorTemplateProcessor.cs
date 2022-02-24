using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using System.Web;
using DotNetNuke.Web.Razor;
//using DotNetNuke.Web.Razor;

namespace OpenTheme
{
    internal class RazorTemplateProcessor
    {
        public string Render(string TemplateVirtualPath, dynamic model)
        {
                //dynamic model = new ExpandoObject();
                //model.Source = source;
                //model.DNNPath = resolver.Resolve("/", PathResolver.RelativeTo.Dnn);
                //model.ManifestPath = resolver.Resolve("/", PathResolver.RelativeTo.Manifest);
                //model.PortalPath = resolver.Resolve("/", PathResolver.RelativeTo.Portal);
                //model.SkinPath = resolver.Resolve("/", PathResolver.RelativeTo.Skin);
                //var modelDictionary = model as IDictionary<string, object>;
                //liveDefinition.TemplateArguments.ForEach(a => modelDictionary.Add(a.Name, a.Value));
                return this.RenderTemplate(TemplateVirtualPath, model);
        }
        private string RenderTemplate(string virtualPath, dynamic model)
        {
            var page = WebPageBase.CreateInstanceFromVirtualPath(virtualPath);
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var pageContext = new WebPageContext(httpContext, page, model);

            var writer = new StringWriter();

            //if (page is WebPage)
            //{
            //    page.ExecutePageHierarchy(pageContext, writer);
            //}
            //else
            {
                var razorEngine = new RazorEngine(virtualPath, null, null);
                razorEngine.Render<dynamic>(writer, model);
            }

            return writer.ToString();
        }
    }
}
