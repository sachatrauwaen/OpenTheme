namespace Satrabel.PersonaBar.ThemeSettings.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Xml;

    using Dnn.PersonaBar.Library;
    using Dnn.PersonaBar.Library.Attributes;
    //using Dnn.PersonaBar.Themes.Components;
    //using Dnn.PersonaBar.Themes.Components.DTO;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.Web.Api;
    using LibSassHost;
    using NUglify;
    using OpenTheme;

    public class Constants
    {
        public const string MenuName = "Dnn.ThemeSettings";
        public const string Edit = "EDIT";
    }

    [MenuPermission(MenuName = Constants.MenuName)]
    public class ThemeSettingsController : PersonaBarApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ThemeSettingsController));
        //private IThemesController _controller = Components.ThemesController.Instance;

        [HttpGet]
        public HttpResponseMessage GetCurrentTheme(string language)
        {
            try
            {

                return this.Request.CreateResponse(HttpStatusCode.OK, this.GetCurrentThemeObject());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetSettings()
        {
            string ascx = GetAscx();
            try
            {

                var ascxSchema = ascx.Replace(".ascx", ".ascx.schema.json");
                var filenameSchema = Globals.ApplicationMapPath + ascxSchema;
                var ascxOptions = ascx.Replace(".ascx", ".ascx.options.json");
                var filenameOptions = Globals.ApplicationMapPath + ascxOptions;
                var ascxData = ascx.Replace(".ascx", ".ascx.data.json");
                var filenameData = Globals.ApplicationMapPath + ascxData;

                var css = Path.GetDirectoryName(ascx) + "/skin.css";
                var cssSchema = css.Replace(".css", ".css.schema.json");
                var filenameCssSchema = Globals.ApplicationMapPath + cssSchema;
                var cssOptions = css.Replace(".css", ".css.options.json");
                var filenameCssOptions = Globals.ApplicationMapPath + cssOptions;
                var cssData = css.Replace(".css", ".css.data.json");
                var filenameCssData = Globals.ApplicationMapPath + cssData;
                var res = new SettingsDto()
                {
                    File = ascx,
                    Global = new FormDto
                    {
                        Exist = File.Exists(filenameCssSchema),
                        Schema = ReadJson(filenameCssSchema),
                        Options = ReadJson(filenameCssOptions),
                        Settings = ReadJson(filenameCssData)
                    },
                    Skin = new FormDto
                    {
                        Exist = File.Exists(filenameSchema),
                        Schema = ReadJson(filenameSchema),
                        Options = ReadJson(filenameOptions),
                        Settings = ReadJson(filenameData)
                    }
                };


                return this.Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                Logger.Error("ascx :" + ascx, ex);
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string GetAscx()
        {
            var ascx = PortalSettings.ActiveTab.SkinSrc;
            if (string.IsNullOrEmpty(ascx))
            {
                ascx = PortalSettings.DefaultPortalSkin;
            }
            ascx = SkinController.FormatSkinSrc(ascx, PortalSettings);
            return ascx;
        }

        private static string ReadJson(string filename)
        {
            return File.Exists(filename) ? File.ReadAllText(filename) : "{}";
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AdvancedPermission(MenuName = Components.Constants.MenuName, Permission = Components.Constants.Edit)]
        //public HttpResponseMessage RestoreTheme(string language)
        //{
        //    try
        //    {
        //        SkinController.SetSkin(SkinController.RootSkin, this.PortalId, SkinType.Portal, "");
        //        SkinController.SetSkin(SkinController.RootContainer, this.PortalId, SkinType.Portal, "");
        //        SkinController.SetSkin(SkinController.RootSkin, this.PortalId, SkinType.Admin, "");
        //        SkinController.SetSkin(SkinController.RootContainer, this.PortalId, SkinType.Admin, "");
        //        DataCache.ClearPortalCache(this.PortalId, true);

        //        return this.Request.CreateResponse(HttpStatusCode.OK, this.GetCurrentThemeObject());
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]

        public SaveSettingsResultDto SaveSettings(SettingsDto input)
        {
            try
            {
                var razor = new RazorTemplateProcessor();
                string ascx = GetAscx();

                var ascxSchema = ascx.Replace(".ascx", ".ascx.schema.json");
                var filenameSchema = Globals.ApplicationMapPath + ascxSchema;

                var css = Path.GetDirectoryName(ascx) + "/skin.css";
                var cssSchema = css.Replace(".css", ".css.schema.json");
                var filenameCssSchema = Globals.ApplicationMapPath + cssSchema;

                var dynamicSkin = System.Web.Helpers.Json.Decode(input.Skin.Settings);
                var dynamicGlobal = System.Web.Helpers.Json.Decode(input.Global.Settings);
                dynamic dynamicObject = new System.Dynamic.ExpandoObject();

                dynamicObject.Global = dynamicGlobal;
                dynamicObject.Skin = dynamicSkin;

                if (File.Exists(filenameCssSchema))
                {
                    var ascxData = ascx.Replace(".ascx", ".ascx.data.json");
                    var filenameData = Globals.ApplicationMapPath + ascxData;
                    File.WriteAllText(filenameData, input.Skin.Settings);
                    Generate(razor, ascx, dynamicObject);
                }
                if (File.Exists(filenameSchema))
                {
                    //var css = Path.GetDirectoryName(ascx) + "/skin.css";
                    var cssData = css.Replace(".css", ".css.data.json");
                    var filenameCssData = Globals.ApplicationMapPath + cssData;
                    File.WriteAllText(filenameCssData, input.Global.Settings);
                    Generate(razor, ascx.Replace(".ascx", ".css"), dynamicObject);
                    //Generate(razor, Path.GetDirectoryName(ascx) + "/skin.css", dynamicGlobal);
                    GenerateSass(razor, Path.GetDirectoryName(ascx) + "/skin.css", dynamicGlobal);
                }
                HostController.Instance.IncrementCrmVersion(true);
                DataCache.ClearPortalCache(PortalSettings.PortalId, true);
            }
            catch (Exception ex)
            {
                return new SaveSettingsResultDto
                {
                    Succes = false,
                    Message = ex.Message
                };
            }
            return new SaveSettingsResultDto
            {
                Succes = true,
            };
        }

        private void Generate(RazorTemplateProcessor razor, string ascxOutput, dynamic dynamicObject)
        {
            var ascxTemplate = ascxOutput + ".cshtml";
            if (File.Exists(Globals.ApplicationMapPath + ascxTemplate))
            {
                var res = razor.Render("~/" + ascxTemplate, dynamicObject);

                var themePath = SkinController.FormatSkinSrc(PortalSettings.ActiveTab.SkinPath, PortalSettings);
                var user = UserController.Instance.GetCurrentUserInfo();

                if (!user.IsSuperUser && themePath.IndexOf("\\portals\\_default\\", StringComparison.OrdinalIgnoreCase) != Null.NullInteger)
                {
                    throw new SecurityException("NoPermission");
                }
                //return Globals.ApplicationMapPath;
                //var filename = Path.Combine(Globals.ApplicationMapPath,ascxTemplate.Replace("/", "\\"));
                var filename = Globals.ApplicationMapPath + ascxOutput;
                File.WriteAllText(filename, res);
            }
        }

        private void GenerateSass(RazorTemplateProcessor razor, string ascxOutput, dynamic dynamicObject)
        {
            var sassTemplate = ascxOutput.Replace(".css", ".scss");
            var ascxTemplate = ascxOutput + ".cshtml";
            if (File.Exists(Globals.ApplicationMapPath + ascxTemplate))
            {
                var res = razor.Render("~/" + ascxTemplate, dynamicObject);
                var themePath = SkinController.FormatSkinSrc(PortalSettings.ActiveTab.SkinPath, PortalSettings);
                var user = UserController.Instance.GetCurrentUserInfo();
                if (!user.IsSuperUser && themePath.IndexOf("\\portals\\_default\\", StringComparison.OrdinalIgnoreCase) != Null.NullInteger)
                {
                    throw new SecurityException("NoPermission");
                }
                //return Globals.ApplicationMapPath;
                //var filename = Path.Combine(Globals.ApplicationMapPath,ascxTemplate.Replace("/", "\\"));

                var filename = Globals.ApplicationMapPath + sassTemplate;
                File.WriteAllText(filename, res);
                if (File.Exists(Globals.ApplicationMapPath + ascxOutput))
                {
                    //var _filesDirectoryPath = Globals.ApplicationMapPath + Path.GetDirectoryName(ascxOutput);
                    string inputFilePath = Globals.ApplicationMapPath + sassTemplate;
                    string outputFilePath = Globals.ApplicationMapPath + ascxOutput;
                    try
                    {
                        var options = new CompilationOptions { SourceMap = true, Precision = 6 };
                        CompilationResult result = SassCompiler.CompileFile(inputFilePath, outputFilePath, options: options);
                        File.WriteAllText(outputFilePath, result.CompiledContent);
                        var min = Uglify.Css(result.CompiledContent);
                        File.WriteAllText(outputFilePath.Replace(".css", ".min.css"), min.Code);
                        //WriteOutput(result);
                    }
                    catch (SassException e)
                    {
                        throw new Exception(e.Message, e);
                        //WriteError("During compilation of SCSS file an error occurred.", e);
                    }
                }
            }
        }

        private object GetCurrentThemeObject()
        {
            var cultureCode = LocaleController.Instance.GetCurrentLocale(this.PortalId).Code;
            var siteLayout = PortalController.GetPortalSetting("DefaultPortalSkin", this.PortalId, Host.DefaultPortalSkin, cultureCode);
            var siteContainer = PortalController.GetPortalSetting("DefaultPortalContainer", this.PortalId, Host.DefaultPortalContainer, cultureCode);
            var editLayout = PortalController.GetPortalSetting("DefaultAdminSkin", this.PortalId, Host.DefaultAdminSkin, cultureCode);
            var editContainer = PortalController.GetPortalSetting("DefaultAdminContainer", this.PortalId, Host.DefaultAdminContainer, cultureCode);

            var currentTheme = new
            {
                //SiteLayout = this._controller.GetThemeFile(this.PortalSettings, siteLayout, ThemeType.Skin),
                //SiteContainer = this._controller.GetThemeFile(this.PortalSettings, siteContainer, ThemeType.Container),
                //EditLayout = this._controller.GetThemeFile(this.PortalSettings, editLayout, ThemeType.Skin),
                //EditContainer = this._controller.GetThemeFile(this.PortalSettings, editContainer, ThemeType.Container),

            };

            return currentTheme;
        }
    }
}