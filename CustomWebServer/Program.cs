using WebServer;

namespace WebServerFromScratch;

public static class Program {
	public static int Main(string[] args) {
		Server server = new();

		server.Error = server.RootAsText("error.html");

		server.Router.Get("/", (context, parameters) => {
			return server.Renderer.RenderPage("Home", new {
			}, 200);
		});


		server.Router.Get("/about", (context, parameters) => {
			return server.Renderer.RenderPage("About", new {
			}, 200);
		});

		server.Router.Post("/hello", (context, parameters) => {
			string json = server.ReadBody(context.Request);
			return server.BakeJson(json, 200);
		});

		server.Start();
		Console.WriteLine("Server died");
		return 0;
	}
}
