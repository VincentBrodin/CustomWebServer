using System.Net;

namespace Stratus;

public class Session(string sessionId) {
	public string SessionId { get; set; } = sessionId;
	public Dictionary<string, object> Values { get; } = [];
}

public class SessionManager {
	private readonly object Lock = new();
	private readonly Dictionary<string, Session> Sessions = [];

	public string GetOrCreateSessionId(HttpListenerContext context) {
		string? sessionId = null;
		foreach (Cookie cookie in context.Request.Cookies) {
			if (cookie.Name == "SessionId") {
				sessionId = cookie.Value;
				break;
			}
		}

		lock (Lock) {
			if (string.IsNullOrEmpty(sessionId) || !Sessions.ContainsKey(sessionId)) {
				sessionId = Guid.NewGuid().ToString();
				Sessions[sessionId] = new Session(sessionId);
				Cookie cookie = new("SessionId", sessionId) {
					HttpOnly = true,
					Expires = DateTime.Now.AddMinutes(30),
				};
				context.Response.AppendCookie(cookie);
			}
		}

		return sessionId;
	}

	public Session? GetSession(HttpListenerContext context) {
		string sessionId = GetOrCreateSessionId(context);
		Sessions.TryGetValue(sessionId, out Session? session);
		return session;
	}
}
