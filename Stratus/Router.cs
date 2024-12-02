using System.Net;

namespace Stratus;

public enum Methods {
	GET,
	POST
}

public delegate Payload HandlerMethod(HttpListenerContext context, Dictionary<string, string> parameters);

public class Route(string path, Methods method, HandlerMethod handler) {
	public string Path { get; set; } = path;
	public Methods Method { get; set; } = method;
	public HandlerMethod Handler { get; set; } = handler;
}

public class Router {
	private readonly List<Route> routes = [];

	public bool HandleStaticFiles = false;
	public string StaticFilesRoot { get; set; } = "/wwwroot";
	public HandlerMethod StaticFilesHandler { get; set; } = (context, parameters) => {
		return new Payload(404);
	};

	public void Post(string path, HandlerMethod handler) {
		routes.Add(new Route(path, Methods.POST, handler));
	}

	public void Get(string path, HandlerMethod handler) {
		routes.Add(new Route(path, Methods.GET, handler));
	}

	public Payload MatchRoute(HttpListenerContext context) {
		HttpListenerRequest request = context.Request;

		if (request.Url == null) {
			return new Payload(404);
		}

		string path = request.Url.AbsolutePath;
		foreach (Route route in routes) {
			if (route.Method.ToString() != request.HttpMethod) {
				continue;
			}

			//Serve static files
			if (HandleStaticFiles
				&& path.GetFirst(StaticFilesRoot.Length) == StaticFilesRoot) {
				Dictionary<string, string> parameters = [];
				parameters.Add("path", path.GetEndFromStart(StaticFilesRoot.Length + 1));
				return StaticFilesHandler(context, parameters);
			}
			else {
				var (match, parameters) = ExtractRouteParams(path, route.Path);
				if (route.Method.ToString() == request.HttpMethod && match) {
					return route.Handler(context, parameters);
				}
			}
		}

		return new Payload(404);
	}

	private static (bool, Dictionary<string, string>) ExtractRouteParams(string requestPath, string routePath) {
		Dictionary<string, string> parameters = [];

		string[] requestSegments = requestPath.Trim('/').Split('/');
		string[] routeSegments = routePath.Trim('/').Split('/');


		if (requestSegments.Length != routeSegments.Length) {
			return (false, parameters);
		}

		for (int i = 0; i < routeSegments.Length; i++) {
			string requestSegment = requestSegments[i];
			string routeSegment = routeSegments[i];

			if (routeSegment.StartsWith("{") && routeSegment.EndsWith("}")) {
				string parameterName = routeSegment.Trim('{', '}');
				parameters[parameterName] = requestSegment;
			}
			// If segments do not match exactly, return empty params (no match)
			else if (requestSegment != routeSegment) {
				return (false, parameters);
			}
		}

		return (true, parameters);
	}
}
