using CustomWebServer;
using HandlebarsDotNet;
using Markdig.Syntax.Inlines;
using Stratus;

namespace DemoProject;

public static class Program {
	public static async Task<int> Main(string[] _) {
		Handlebars.RegisterHelper("setActive", (context, arguments) => {
			string? currentPage = arguments[0]?.ToString();
			string? linkPage = arguments[1]?.ToString();

			if (currentPage == null || linkPage == null) {
				return "";
			}

			return currentPage == linkPage ? "active" : "";
		});

		Server server = new() {
			Name = "Stratus"
		};

		DocsHelper docs = new(server.RootPath("Docs"));
		docs.RouteDocs(server);

		server.Router.Get("/", (context, parameters) => {
			return server.Renderer.RenderPage("Home", new {
			}, 200, "Stratus");
		});

		server.Router.Get("/about", (context, parameters) => {
			return server.Renderer.RenderPage("About", new {
			}, 200);
		});

		var (titles, paths) = docs.GetStaticRoutes();
		object staticRoutes = titles.Zip(paths, (title, path) => new {
			title,
			path
		});


		server.Router.Post("/search", (context, parameters) => {
			return server.BakeJson(new { results = staticRoutes }, 200);
		});

		await server.Start();
		Console.WriteLine("Server died");
		return 0;
	}
}
