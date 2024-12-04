using HandlebarsDotNet;
using System.Text;

namespace Stratus;
public class Renderer(Server connectedServer) {
	public Server Server { get; set; } = connectedServer;
	private string layoutTemplate = "";
	private readonly Dictionary<string, string> pagesTemplates = [];

	public Payload RenderPage(string pageName, object pageData, int status = 200, string? title = null) {
		var layout = Handlebars.Compile(layoutTemplate);

		if(!pagesTemplates.TryGetValue(pageName, out string? template)) {
			Console.WriteLine($"Page {pageName} does not exist");
			return new Payload(404);
		}

		var page = Handlebars.Compile(template!);

		var context = new {
			title = title ?? $"{Server.Name} | {pageName}",
			page = pageName,
			body = page(pageData),
		};

		byte[] data = Encoding.UTF8.GetBytes(layout(context));

		return new Payload(data, "text/html", status);
	}

	public void RegisterPartials() {
		string? layout = Server.RootAsText("views/layout/Main.hbs");
		if (layoutTemplate == null) {
			Console.WriteLine($"Missing views/layout/Main.hbs file from wwwroot.");
			return;
		}
		else {
			layoutTemplate = layout!;
		}

		if (Directory.Exists(Server.RootPath("views/pages"))) {
			string[] pathToPages = Directory.GetFiles(Server.RootPath("views/pages"));
			foreach (string path in pathToPages) {
				string pageName = Path.GetFileNameWithoutExtension(path);
				string html = File.ReadAllText(path);
				pagesTemplates[pageName] = html;
			}

		}
		else {
			Console.WriteLine("Missing views/pages from wwwroot");
		}

		if (Directory.Exists(Server.RootPath("views/partials"))) {
			string[] pathToPartials = Directory.GetFiles(Server.RootPath("views/partials"));
			foreach (string path in pathToPartials) {
				string partialsName = Path.GetFileNameWithoutExtension(path);
				string html = File.ReadAllText(path);
				Handlebars.RegisterTemplate(partialsName, html);
			}
		}
		else {
			Console.WriteLine("Missing views/partials from wwwroot");
		}


	}
}
