using WebServer;

namespace WebServerFromScratch;

public static class Program {
	public static int Main(string[] args) {
		Server server = new();

		server.Error = server.RootAsText("error.html");
		/*server.Router.Get("/hello", (context, parameters) => {
			string greetings = context.Request.QueryString["grettings"] ?? "world";
			return $"<HTML><BODY> Hello {greetings}!</BODY></HTML>";
		});

		server.Router.Get("/goodbye", (context, parameters) => {
			string greetings = context.Request.QueryString["grettings"] ?? "world";
			return $"<HTML><BODY> Goodbye {greetings}!</BODY></HTML>";
		});*/

		server.Router.Get("/", (context, parameters) => {
			return server.Root("index.html");
		});

		server.Router.Get("/wwwroot/{item}", (context, parameters) => {
			if(parameters.ContainsKey("item")) {
				return server.Root(parameters["item"]);
			}
			return null;
		});

		server.Start();
		Console.WriteLine("Server died");
		return 0;
	}
}
