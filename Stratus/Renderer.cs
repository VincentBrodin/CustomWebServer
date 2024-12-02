using HandlebarsDotNet;
using System.Text;

namespace Stratus;
public class Renderer(Server connectedServer) {
	public Server Server { get; set; } = connectedServer;
	private string layoutTemplate = "";
	private Dictionary<string, string> pagesTemplates = [];

	public Payload RenderPage(string pageName, object pageData, int status = 200, string? title = null) {
		var layout = Handlebars.Compile(layoutTemplate);
		var page = Handlebars.Compile(pagesTemplates[pageName]);

		var context = new {
			title = title ?? $"{Server.Name} | {pageName}",
			page = pageName,
			body = page(pageData),
		};

		byte[] data = Encoding.UTF8.GetBytes(layout(context));

		return new Payload(data, "text/html", status);
	}

	public void RegisterPartials() {
		layoutTemplate = Server.RootAsText("views/layout/Main.hbs");

		string[] pathToPages = Directory.GetFiles(Server.RootPath("views/pages"));
		foreach (string path in pathToPages) {
			string pageName = Path.GetFileNameWithoutExtension(path);
			string html = File.ReadAllText(path);
			pagesTemplates[pageName] = html;
		}

		string[] pathToPartials = Directory.GetFiles(Server.RootPath("views/partials"));
		foreach (string path in pathToPartials) {
			string partialsName = Path.GetFileNameWithoutExtension(path);
			string html = File.ReadAllText(path);
			Handlebars.RegisterTemplate(partialsName, html);
		}
	}
}
