#region Header
////////////////////////////////////////////////////////////////////////////////////
//
//	File Description: Razor Mediator Wrapper Helper
// ---------------------------------------------------------------------------------
//	Date Created	: November 26, 2015
//	Author			: Mark Vlasenko
//
////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TcmDebugger.Extensions;
using TcmDebugger.Misc;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Tridion.ContentManager.Templating.Configuration;
using Tridion.Extensions.Mediators.Razor;

namespace TcmDebugger.Mediators
{
	public class WrappedRazorMediator : IMediator
	{
	    public static Engine Engine;
	    public static Template Template;
	    public static Package Package;


        private readonly RazorMediator mOriginalMediator;
        private static TemplatingLogger mLogger;

        static WrappedRazorMediator()
        {
            mLogger = TemplatingLogger.GetLogger(typeof(WrappedRazorMediator));
        }


        /// <summary>
        /// Initializes the <see cref="WrappedRazorMediator" /> by executing the static constructor
        /// </summary>
        public static void Initialize()
		{
			//
		}
        
        /// <summary>
		/// Initializes a new instance of the <see cref="WrappedRazorMediator"/> class.
        /// </summary>
		public WrappedRazorMediator()
		{
			mOriginalMediator = new RazorMediator();
		}

        /// <summary>
        /// Configures the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
		public void Configure(MediatorElement configuration)
		{
			mOriginalMediator.Configure(configuration);
		}

        /// <summary>
        /// Executes a Tridion template transformation
        /// </summary>
        /// <param name="engine"><see cref="T:Tridion.ContentManager.Templating.Engine" /></param>
        /// <param name="template"><see cref="T:Tridion.ContentManager.CommunicationManagement.Template"/></param>
        /// <param name="package"><see cref="T:Tridion.ContentManager.Templating.Package"/></param>
		public void Transform(Engine engine, Template template, Package package)
		{
            RazorHandler handler = new RazorHandler(template.Id.ToString(), template.WebDavUrl, template.Content, template);
            handler.Initialize();

            string output = handler.CompileAndExecute(template.RevisionDate, engine, package);

            Engine = engine;
            Template = template;
            Package = package;

        }
    }
}
