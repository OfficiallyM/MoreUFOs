using System.IO;
using TLDLoader;

namespace MoreUFOs.Modules
{
	internal static class Logger
	{
		private static string logFile = "";
		private static bool initialised = false;
		public enum LogLevel
		{
			Debug,
			Info,
			Warning,
			Error,
			Critical
		}

		public static void Init()
		{
			if (!initialised)
			{
				// Create logs directory.
				if (Directory.Exists(ModLoader.ModsFolder))
				{
					Directory.CreateDirectory(Path.Combine(ModLoader.ModsFolder, "Logs"));
					logFile = ModLoader.ModsFolder + $"\\Logs\\{MoreUFOs.Mod.ID}.log";
					File.WriteAllText(logFile, $"{MoreUFOs.Mod.Name} v{MoreUFOs.Mod.Version} initialised\r\n");
					initialised = true;
				}
			}
		}

		/// <summary>
		/// Log messages to a file.
		/// </summary>
		/// <param name="msg">The message to log</param>
		public static void Log(string msg, LogLevel logLevel = LogLevel.Info)
		{
			// Don't print debug messages outside of debug mode.
			if (!MoreUFOs.Debug && logLevel == LogLevel.Debug) return;

			if (logFile != string.Empty)
				File.AppendAllText(logFile, $"[{logLevel}] {msg}\r\n");
		}
	}
}
