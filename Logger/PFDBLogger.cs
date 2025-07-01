using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace PFDB.Logging;



/// <summary>
/// Defines the logging class and associated actions.
/// </summary>
public class PFDBLogger
{

	private IConfigurationRoot ConfigurationRoot { get; set; }
	private static bool disableSourceCaller = false;
	private string configFileName = "PFDBconfig.ini";

	/// <summary>
	/// Constructs the logging class. 
	/// </summary>
	/// <param name="logFileName">Specifies the file name to write to. By default set to .pfdblog</param>
	public PFDBLogger(string logFileName = ".pfdblog")
	{
		if (File.Exists(configFileName))
		{
			IEnumerable<string> lines = File.ReadLines(configFileName);
			foreach (string line in lines)
			{
				Console.WriteLine(line);
				if (line.Contains("disablesourcecalling", StringComparison.CurrentCultureIgnoreCase))
				{
					if (line.Contains("true", StringComparison.CurrentCultureIgnoreCase))
					{
						disableSourceCaller = true;
					}
					else if (line.Contains("false", StringComparison.CurrentCultureIgnoreCase))
					{
						disableSourceCaller = false;
					}
				}
			}
		}


		IConfigurationBuilder configuration = new ConfigurationBuilder();
		configuration.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().AddJsonFile("appsettings.json");
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration.Build())
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.WriteTo.File(logFileName, shared: true)
			.CreateLogger();

		Log.Logger.Information("Application start. Logging has been activated.");

		ConfigurationRoot = configuration.Build();
	}

	/// <summary>
	/// Logs the stopwatch passed into this function. Note that the stopwatch is forcibly stopped.
	/// </summary>
	/// <param name="stopwatch">The stopwatch that has been measuring the elapsed time.</param>
	/// <param name="message">Custom messge identifying the purpose of the stopwatch.</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogStopwatch(Stopwatch stopwatch, string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		stopwatch.Stop();
		ArgumentNullException.ThrowIfNull(caller);

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t\t Elapsed time: {stopwatch.Elapsed.ToString(@"mm\:ss\.ffffff")}");
		stringBuilder.Append($"\t Message: {message}");
		if (parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Information(stringBuilder.ToString(), parameter);
	}

	/// <summary>
	/// Logs a debug message.
	/// </summary>
	/// <param name="message">Custom messge identifying the purpose of the debug mesaage</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogDebug(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		ArgumentNullException.ThrowIfNull(caller);

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t Message: {message} ");
		if (parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Debug(stringBuilder.ToString(), parameter);
	}


	/// <summary>
	/// Logs an informational message.
	/// </summary>
	/// <param name="message">Custom messge identifying the purpose of the information mesaage</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogInformation(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		if (caller is null)
		{
			throw new ArgumentNullException(nameof(caller));
		}

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t Message: {message} ");
		if (parameter?.Length != 0 && parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Information(stringBuilder.ToString(), parameter);
	}

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">Custom messge identifying the purpose of the warning mesaage</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogWarning(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		if (caller is null)
		{
			throw new ArgumentNullException(nameof(caller));
		}

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t Message: {message} ");
		if (parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Warning(stringBuilder.ToString(), parameter);
	}

	/// <summary>
	/// Logs am error message.
	/// </summary>
	/// <param name="message">Custom messge identifying the purpose of the error mesaage</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogError(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		if (caller is null)
		{
			throw new ArgumentNullException(nameof(caller));
		}

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t Message: {message} ");
		if (parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Error(stringBuilder.ToString(), parameter);
	}

	/// <summary>
	/// Logs a fatal error message.
	/// </summary>
	/// <param name="message">Custom messge identifying the purpose of the fatal error mesaage</param>
	/// <param name="caller">Blank argument to identify the caller function. DO NOT POPULATE.</param>
	/// <param name="parameter">Optional parameter to log.</param>
	public static void LogFatal(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
	{
		if (caller is null)
		{
			throw new ArgumentNullException(nameof(caller));
		}

		Assembly invokingAssembly = Assembly.GetCallingAssembly();
		var mth = new StackTrace().GetFrame(1)?.GetMethod();
		var cls = mth?.ReflectedType?.Name;

		StringBuilder stringBuilder = new StringBuilder();
		if (disableSourceCaller == false) stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \n\t");
		stringBuilder.Append($"\t Message: {message} ");
		if (parameter?.Length != 0 && parameter != null) stringBuilder.Append($"\t [Parameter: {{parameter}}]");

		Log.Fatal(stringBuilder.ToString(), parameter);
	}
}

