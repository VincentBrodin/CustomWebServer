using System.Net;

namespace Stratus;

public class Session(string sessionId, DateTime expirationTime) {
	public string SessionId { get; } = sessionId;
	public DateTime ExpirationTime { get; } = expirationTime;
	private readonly Dictionary<string, object?> values = [];

	public T? Get<T>(string key) {
		values.TryGetValue(key, out object? value);
		if (value == null) {
			return default;
		}
		return (T)Convert.ChangeType(value, typeof(T));
	}

	public void Set<T>(string key, T value) {
		values[key] = value;
	}

	public bool HasKey(string key) {
		return values.ContainsKey(key);
	}
}


public class SessionManager {
	private readonly object Lock = new();
	private readonly Dictionary<string, Session> Sessions = [];
	private readonly Queue<string> ExpirationQueue = [];

	public string GetOrCreateSessionId(HttpListenerContext context) {
		CleanUpSessions();
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
				DateTime expirationTime = DateTime.Now.AddSeconds(10);
				Sessions[sessionId] = new Session(sessionId, expirationTime);
				ExpirationQueue.Enqueue(sessionId);

				Cookie cookie = new("SessionId", sessionId) {
					HttpOnly = true,
					Expires = expirationTime,
				};
				context.Response.AppendCookie(cookie);
				Console.WriteLine($"Created new session {sessionId}");
			}
		}
		return sessionId;
	}

	public Session? GetSession(HttpListenerContext context) {
		string sessionId = GetOrCreateSessionId(context);
		Sessions.TryGetValue(sessionId, out Session? session);
		return session;
	}

	//O(1) At best and O(k) at worst (k <= n)
	private void CleanUpSessions() {
		lock(Lock) {
			while(ExpirationQueue.Count > 0) {
				string sessionId = ExpirationQueue.Peek();
				Session session = Sessions[sessionId];
				if(DateTime.Now >= session.ExpirationTime) {
					ExpirationQueue.Dequeue();
					Console.WriteLine($"Cleaned up {sessionId}");
				}
				else {
					break;
				}
			}
		}
	}
}
