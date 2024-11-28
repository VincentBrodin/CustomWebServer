using HandlebarsDotNet;
using Stratus;

namespace CustomWebServer;

public static class Program {
	public static int Main(string[] args) {
		Handlebars.RegisterHelper("setActive", (context, arguments) => {
			string? currentPage = arguments[0]?.ToString();
			string? linkPage = arguments[1]?.ToString();

			if (currentPage == null || linkPage == null) {
				return "";
			}

			return currentPage == linkPage ? "active" : "";
		});

		Server server = new();

		DocsHelper docs = new(server.RootPath("Docs"));
		docs.RouteDocs(server);

		server.Error = server.RootAsText("error.html");

		server.Router.Get("/", (context, parameters) => {
			return server.Renderer.RenderPage("Home", new {
			}, 200, "Docs");
		});


		server.Router.Get("/about", (context, parameters) => {
			return server.Renderer.RenderPage("About", new {
			}, 200);
		});


		server.Start();
		Console.WriteLine("Server died");
		return 0;
	}
}
