using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using SimpleWcf.Contracts;

namespace SimpleWcf.Console
{
	class Program
	{
		static void Main()
		{
			// The URI to serve as base address
			const int httpPort = 8099;
			const int tcpPort = 8091;
			const string urlFormat = "{0}://localhost:{1}/Service";

			var httpAddress = new Uri(string.Format(urlFormat, "http", httpPort));
			var tcpAddress = new Uri(string.Format(urlFormat, "net.tcp", tcpPort));
			var addresses = new[] { httpAddress, tcpAddress };

			var host = CreateService(addresses, typeof(Service), typeof(IService));

			// Start the Service
			try
			{
				host.Open();

				System.Console.WriteLine("Service is listening at: {0}", string.Join(" and ", addresses.Select(a => a.AbsoluteUri)));
			}
			catch (AddressAlreadyInUseException exc)
			{
				throw;
			}
			catch (AddressAccessDeniedException)
			{
				System.Console.WriteLine("Woooha! That didn't work!");
				System.Console.WriteLine("Oh well, let's be nice to the user and automatically run 'netsh'");
				System.Console.WriteLine("I guess the problem is that this application can't use the port '" + httpPort + "'" + Environment.NewLine);

				RunNetsh(httpPort);

				host.Open();	// Let's give it one more try...
			}

			Process.Start(httpAddress + "/rest/getdata/Hello%20world!");

			var key = new ConsoleKeyInfo();

			while (key.KeyChar != 'q')
			{
				System.Console.WriteLine("Press 'q' to quit." + Environment.NewLine);
				key = System.Console.ReadKey();
			}
		}

		static ServiceHost CreateService(Uri[] serviceAddresses, Type serviceType, Type serviceContract)
		{
			// Create ServiceHost
			var host = new ServiceHost(serviceType, serviceAddresses);
			// var host = new WebServiceHost(serviceType, serviceAddresses);

			// Add service endpoint(s)
			// http://wcftutorial.net/WCF-Types-of-Binding.aspx
			// host.AddServiceEndpoint(serviceContract, new WSHttpBinding(), "/ws");
			// host.AddServiceEndpoint(serviceContract, new BasicHttpBinding(), "/basic");
			// host.AddServiceEndpoint(serviceContract, new NetTcpBinding(), "/tcp");
			// host.AddServiceEndpoint(serviceContract, new NetNamedPipeBinding(), "/pipe");
			// host.AddServiceEndpoint(serviceContract, new NetPeerTcpBinding(), "/peer");
			// host.AddServiceEndpoint(serviceContract, new WebHttpBinding(), "/web");

			var endpoint = host.AddServiceEndpoint(serviceContract, new WebHttpBinding(), "/rest");
			var webBehavior = new WebHttpBehavior
								{
									AutomaticFormatSelectionEnabled = true,
									DefaultBodyStyle = WebMessageBodyStyle.Bare,
									DefaultOutgoingRequestFormat = WebMessageFormat.Json,
									DefaultOutgoingResponseFormat = WebMessageFormat.Json,
									FaultExceptionEnabled = true,
									HelpEnabled = true
								};
			endpoint.Behaviors.Add(webBehavior);
			/*
			*/

			// Enable metadata exchange
			var metadataBehavior = new ServiceMetadataBehavior { HttpGetEnabled = true };
			// metadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			host.Description.Behaviors.Add(metadataBehavior);

			// Adding metadata exchange endpoint
			var mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
			host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "mex");

			return host;
		}

		static void RunNetsh(int port)
		{
			var proc = new ProcessStartInfo
				{
					UseShellExecute = true,
					WorkingDirectory = Environment.SystemDirectory,
					FileName = "netsh",
					Arguments = "http add urlacl url=http://+:" + port + "/ user=" + Environment.UserName,
					Verb = "runas"
				};

			try
			{
				Process.Start(proc);
			}
			catch(Exception)
			{
				// The user refused the elevation.
				System.Console.WriteLine("Argh! Since you didn't trust me you have to manually run 'netsh'.");
				System.Console.WriteLine(Environment.NewLine + "Press <enter> to exit.");
				System.Console.ReadLine();

				// Quit
				Environment.Exit(-1);
			}
		}
	}
}
