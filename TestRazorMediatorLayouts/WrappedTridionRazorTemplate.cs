using System.Collections.Generic;
using System.Web.Mvc;
using TcmDebugger.Mediators;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;
using Tridion.Extensions.Mediators.Razor;
using Tridion.Extensions.Mediators.Razor.Models;
using Tridion.Extensions.Mediators.Razor.Utilities;
using SDL.TestRazorMediatorLayouts.Controllers;

namespace SDL.TestRazorMediatorLayouts
{
    public class WrappedTridionRazorTemplate : WebViewPage
    {
        private readonly TridionRazorTemplate _tridionRazorTemplate;

        public WrappedTridionRazorTemplate()
        {
            _tridionRazorTemplate = new TridionRazorTemplate();
        }

        protected override void InitializePage()
        {
            base.InitializePage();

            var controller = this.ViewContext.Controller as DefaultController;

            if (controller != null)
            {
                _tridionRazorTemplate.Initialize(WrappedRazorMediator.Engine, WrappedRazorMediator.Package, WrappedRazorMediator.Template, null);
            }
        }

        public dynamic Component => _tridionRazorTemplate.Component;

        public List<ComponentPresentationModel> ComponentPresentations => _tridionRazorTemplate.ComponentPresentations;

        public dynamic ComponentTemplate => _tridionRazorTemplate.ComponentTemplate;

        public dynamic Fields => _tridionRazorTemplate.Fields;

        public dynamic Folder => _tridionRazorTemplate.Folder;

        public bool IsComponentTemplate => _tridionRazorTemplate.IsComponentTemplate;

        public bool IsPageTemplate => _tridionRazorTemplate.IsPageTemplate;

        public bool IsSiteEditEnabled => _tridionRazorTemplate.IsSiteEditEnabled;

        public TemplatingLogger Log => _tridionRazorTemplate.Log;

        public dynamic Metadata => _tridionRazorTemplate.Metadata;

        public dynamic MetaData => _tridionRazorTemplate.MetaData;

        public ModelUtilities Models => _tridionRazorTemplate.Models;

        public dynamic Package => _tridionRazorTemplate.Package;

        public dynamic Page => _tridionRazorTemplate.Page;

        public dynamic PageTemplate => _tridionRazorTemplate.PageTemplate;

        public dynamic Publication => _tridionRazorTemplate.Publication;

        public RazorTemplateModel RazorTemplate => _tridionRazorTemplate.RazorTemplate;

        public string RenderMode => _tridionRazorTemplate.RenderMode;

        public dynamic StructureGroup => _tridionRazorTemplate.StructureGroup;

        public Template Template => _tridionRazorTemplate.Template;

        public TridionUtilities TridionHelper => _tridionRazorTemplate.TridionHelper;

        public string Version => _tridionRazorTemplate.Version;

        public string Debug(string message)
        {
            return _tridionRazorTemplate.Debug(message);
        }
    
        public void Dispose()
        {
            _tridionRazorTemplate.Dispose();
        }

        public string Error(string message)
        {
            return _tridionRazorTemplate.Error(message);
        }

        public override void Execute()
        {
            _tridionRazorTemplate.Execute();
        }

        public List<ComponentPresentationModel> GetComponentPresentationsBySchema(params string[] schemaNames)
        {
            return _tridionRazorTemplate.GetComponentPresentationsBySchema(schemaNames);
        }

        public List<ComponentPresentationModel> GetComponentPresentationsByTemplate(params string[] templateNames)
        {
            return _tridionRazorTemplate.GetComponentPresentationsByTemplate(templateNames);
        }

        public string HtmlDecode(string textToDecode)
        {
            return _tridionRazorTemplate.HtmlDecode(textToDecode);
        }

        public string HtmlEncode(string textToEncode)
        {
            return _tridionRazorTemplate.HtmlEncode(textToEncode);
        }

        public string Info(string message)
        {
            return _tridionRazorTemplate.Info(message);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex, string value)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex, value);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex, bool renderTcdlTagOnError)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex, renderTcdlTagOnError);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex, bool htmlEncodeResult, bool resolveHtmlAsRTFContent)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex, htmlEncodeResult, resolveHtmlAsRTFContent);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex, string value, bool renderTcdlTagOnError)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex, value, renderTcdlTagOnError);
        }

        public string RenderComponentField(string fieldExpression, int fieldIndex, bool htmlEncodeResult, bool resolveHtmlAsRTFContent, bool renderTcdlTagOnError)
        {
            return _tridionRazorTemplate.RenderComponentField(fieldExpression, fieldIndex, htmlEncodeResult, resolveHtmlAsRTFContent, renderTcdlTagOnError);
        }

        public string RenderComponentPresentation(string componentID, string templateID)
        {
            return _tridionRazorTemplate.RenderComponentPresentation(componentID, templateID);
        }

        public string RenderComponentPresentation(TcmUri componentID, TcmUri templateID)
        {
            return _tridionRazorTemplate.RenderComponentPresentation(componentID, templateID);
        }

        public string RenderComponentPresentations()
        {
            return _tridionRazorTemplate.RenderComponentPresentations();
        }

        public string RenderComponentPresentationsByTemplate(params string[] templateNames)
        {
            return _tridionRazorTemplate.RenderComponentPresentationsByTemplate(templateNames);
        }

        public string StripHtml(string html)
        {
            return _tridionRazorTemplate.StripHtml(html);
        }

        public string UrlDecode(string textToDecode)
        {
            return _tridionRazorTemplate.UrlDecode(textToDecode);
        }

        public string UrlEncode(string textToEncode)
        {
            return _tridionRazorTemplate.UrlEncode(textToEncode);
        }

        public string Warning(string message)
        {
            return _tridionRazorTemplate.Warning(message);
        }
    }
}