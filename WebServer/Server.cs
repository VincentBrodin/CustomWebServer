using System.Net;
using System.Text;
using System.Text.Json;

namespace WebServer;
public struct Payload {
	public byte[]? Content { get; set; }
	public string? ContentType { get; set; }
	public int Status { get; set; }

	public Payload(byte[]? content, string? contentType, int status = 200) {
		Content = content;
		ContentType = contentType;
		Status = status;
	}
	public Payload(int status = 200) {
		Content = null;
		ContentType = null;
		Status = status;
	}

	public bool Empty() => Content == null || ContentType == null;
}

public class Server {
	public HttpListener Listener { get; } = new();
	public Router Router { get; } = new();
	public Renderer Renderer { get; }

	/// <summary>
	/// The state of the server
	/// </summary>
	public bool Running { get; set; }

	/// <summary>
	/// By defualt the base url is http://localhost:8080/
	/// </summary>
	public string BaseUrl { get; set; } = "http://localhost:8080/";

	private readonly string wwwroot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

	/// <summary>
	/// The html that will be sent if an error occurs
	/// </summary>
	public string Error { get; set; } = "<HTML><BODY>404 Not Found</BODY></HTML>";

	public Server() {
		Router.HandleStaticFiles = true;
		Router.StaticFilesHandler = (context, parameters) => {
			return Root(parameters["path"]);
		};

		Renderer = new Renderer(this);
	}

	/// <summary>
	/// Takes in a path to a file and returns the content type
	/// </summary>
	/// <param name="path">Path to any file, the file does not need to exist</param>
	/// <returns>A string in the form of {value}/{value}</returns>
	private string GetContentType(string path) {
		string extension = Path.GetExtension(path).ToLower();

		return extension switch {
			".html" => "text/html",
			".css" => "text/css",
			".js" => "application/javascript",
			".json" => "application/json",
			".png" => "image/png",
			".jpg" => "image/jpeg",
			".jpeg" => "image/jpeg",
			".gif" => "image/gif",
			".svg" => "image/svg+xml",
			".ico" => "image/x-icon",
			_ => "application/octet-stream", // default for unknown files
		};
	}

	/// <summary>
	/// Returns the given root path ready to serve (Will work for any type)
	/// </summary>
	/// <param name="path">The path to the item (in the wwwroot folder)</param>
	/// <returns>Null or a tuple of the content in bytes and the content type as a string</returns>
	public Payload Root(string path) {
		string rootPath = Path.Combine(wwwroot, path);
		Console.WriteLine($"Looking for {path} in wwwroot ({rootPath})");
		if (File.Exists(rootPath)) {
			string contentType = GetContentType(rootPath);
			return new Payload(File.ReadAllBytes(rootPath), contentType, 200);
		}
		Console.WriteLine($"{path} is not in wwwroot ({rootPath})");
		return new Payload(null, null, 404);
	}

	/// <summary>
	/// Returns the given root path as plain text
	/// </summary>
	/// <param name="path">The path to the item (in the wwwroot folder)</param>
	/// <returns>The content as text</returns>
	public string RootAsText(string path) {
		string rootPath = Path.Combine(wwwroot, path);
		if (File.Exists(rootPath)) {
			return File.ReadAllText(rootPath);
		}
		return Error;
	}

	/// <summary>
	/// Returns the system path to the file in the wwwroot directory
	/// </summary>
	/// <param name="path">The path to the item (in the wwwroot folder)</param>
	/// <returns>String to the path (does not check if the file exists)</returns>
	public string RootPath(string path) {
		return Path.Combine(wwwroot, path);
	}

	#region Baking
	public Payload BakeHtml(string? html, int status) {
		if (html == null) {
			return new Payload(null, null, 404);
		}
		return new Payload(Encoding.UTF8.GetBytes(html), "text/html", status);
	}

	public Payload BakeJson(string? json, int status) {
		if (json == null) {
			return new Payload(null, null, 404);
		}
		return new Payload(Encoding.UTF8.GetBytes(json), "application/json", status);
	}


	public Payload BakeJson(object data, int status) {
		string json = JsonSerializer.Serialize(data);
		return new Payload(Encoding.UTF8.GetBytes(json), "application/json", status);
	}
	#endregion

	#region Reader
	public string ReadBody(HttpListenerRequest request) {
		using StreamReader reader = new(request.InputStream, request.ContentEncoding);
		return reader.ReadToEnd();
	}
	#endregion

	/// <summary>
	/// Starts the server
	/// </summary>
	public void Start() {
		Renderer.RegisterPartials();
		Running = true;

		Console.WriteLine($"Starting server @ {Tooling.TerminalURL(BaseUrl, BaseUrl)}");

		Listener.Prefixes.Clear();
		Listener.Prefixes.Add(BaseUrl);
		Listener.Start();
		Console.WriteLine($"Server is running @ {Tooling.TerminalURL(BaseUrl, BaseUrl)}");
		while (Running) {
			//Task.Run(() => { HandleRequest(Listener.GetContext()); }); //Seems to leak????
			HandleRequest(Listener.GetContext());
		}

		Listener.Stop();
	}

	private void HandleRequest(HttpListenerContext context) {
		try {
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			Console.WriteLine($"Request URL: {request.Url}");
			Console.WriteLine($"Request Raw URL: {request.RawUrl}");

			Payload routeOutput = Router.MatchRoute(context);

			if (routeOutput.Empty()) {
				byte[] buffer = Encoding.UTF8.GetBytes(Error);
				response.ContentLength64 = buffer.Length;
				response.ContentType = "text/html";

				response.StatusCode = routeOutput.Status;

				Stream output = response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();
			}
			else {
				byte[] buffer = routeOutput.Content!;
				response.ContentLength64 = buffer.Length;
				response.ContentType = routeOutput.ContentType!;
				response.StatusCode = routeOutput.Status;

				Stream output = response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();
			}
		}
		catch (Exception exception) {
			// Handle errors here (e.g., log to a file, etc.)
			Console.WriteLine($"Error while handling request: {exception.Message}");
		}
	}

	public void Stop() {
		Running = false;
	}
}

