namespace WebServer;
internal static class Tooling {
	public static string TerminalURL(string caption, string url) => $"\u001B]8;;{url}\a{caption}\u001B]8;;\a";

	public static string GetFirst(this string text, int count) {
		count = Math.Min(count, text.Length);
		return text.Substring(0, count);
	}

	public static string GetLast(this string text, int count) {
		count = Math.Min(count, text.Length);
		return text.Substring(text.Length - count, count);
	}


	public static string GetEndFromStart(this string text, int count) {
		if (count >= text.Length)
			return string.Empty;

		count = Math.Min(count, text.Length);
		return text.Substring(count);
	}
}
