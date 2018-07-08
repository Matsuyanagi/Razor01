using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Razor01
{
	class Program
	{
		static int Main( string[] args )
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

			var filenameInputJson = "sample/defines.json";
			var defineJsonText = File.ReadAllText( filenameInputJson );
			var defines = DynamicJson.Parse( defineJsonText );

			var config = new TemplateServiceConfiguration
			{
				Debug = true,
				EncodedStringFactory = new RawStringFactory()		// 出力で HTML エンコードを行わない '>' '<' などをそのまま出力する。デフォルトでは html エンコード '>' -> '&gt;' として出力される
			};

			var service = RazorEngineService.Create( config );
			var result = "";

			string templateFile = "sample/sample.cshtml";
			var template = new LoadedTemplateSource( File.ReadAllText( templateFile ), templateFile );
			result =
				service.RunCompile( template, "templateKey1", typeof( DynamicObject ), ( object ) defines );


			Console.WriteLine( result );

			return 0;
		}
	}
}