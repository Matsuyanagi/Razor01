using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Razor01
{
	class Program
	{
		static int Main(string[] args)
		{

			if ( AppDomain.CurrentDomain.IsDefaultAppDomain() ) {
				// RazorEngine cannot clean up from the default appdomain...
				// Console.WriteLine( "Switching to secound AppDomain, for RazorEngine..." );
				AppDomainSetup adSetup = new AppDomainSetup();
				adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
				var current = AppDomain.CurrentDomain;
				// You only need to add strongnames when your appdomain is not a full trust environment.
				var strongNames = new StrongName[0];

				var domain = AppDomain.CreateDomain(
					"MyMainDomain", null,
					current.SetupInformation, new PermissionSet( PermissionState.Unrestricted ),
					strongNames );
				var exitCode = domain.ExecuteAssembly( Assembly.GetExecutingAssembly().Location );
				// RazorEngine will cleanup. 
				AppDomain.Unload( domain );
				return exitCode;
			}

			// Continue with your code.
//			string template = "Hello @Model.Name, welcome to RazorEngine! by string";
/* 			var result =
				Engine.Razor.RunCompile( template, "templateKey", null, new { Name = "World" } );
 */
			var config = new TemplateServiceConfiguration();
			config.Debug = true;
			// .. configure your instance

			var service = RazorEngineService.Create(config);
			var result = "";
			// string template = "Hello @Model.Name, welcome to RazorEngine!";
			string templateFile = "sample/sample.cshtml";
			var template = new LoadedTemplateSource( File.ReadAllText( templateFile ), templateFile );
            result =
            	Engine.Razor.RunCompile( template, "templateKey1", null, new { Name = "World" });
            // result = service.RunCompile(template, "templateKey", null, new { Name = "World" });

			Console.WriteLine( result );

			return 0;
		}
	}
}
