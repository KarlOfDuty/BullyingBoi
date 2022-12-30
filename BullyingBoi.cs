using System.Reflection;
using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace BullyingBoi;

internal static class BullyingBoi
{
	// Sets up a dummy client to use for logging
	public static DiscordClient discordClient = new DiscordClient(new DiscordConfiguration { Token = "DUMMY_TOKEN", TokenType = TokenType.Bot, MinimumLogLevel = LogLevel.Debug });

	private static void Main()
	{
		MainAsync().GetAwaiter().GetResult();
	}

	private static async Task MainAsync()
	{
		Logger.Log("Starting " + Assembly.GetEntryAssembly()?.GetName().Name + " version " + GetVersion() + "...");
		try
		{
			Reload();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}
		catch (Exception e)
		{
			Logger.Fatal("Fatal error:\n" + e);
			Console.ReadLine();
		}
	}

	public static string GetVersion()
	{
		Version version = Assembly.GetEntryAssembly()?.GetName().Version;
		return version?.Major + "." + version?.Minor + "." + version?.Build + (version?.Revision == 0 ? "" : "-" + (char)(64 + version?.Revision ?? 0));
	}

	public static async void Reload()
	{
		if (discordClient != null)
		{
			await discordClient.DisconnectAsync();
			discordClient.Dispose();
			Logger.Log("Discord client disconnected.");
		}

		Logger.Log("Loading config \"" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "config.yml\"");
		Config.LoadConfig();

		// Check if token is unset
		if (Config.token is "<add-token-here>" or "")
		{
			Logger.Fatal("You need to set your bot token in the config and start the bot again.");
			throw new ArgumentException("Invalid Discord bot token");
		}

		Logger.Log("Setting up Discord client...");

		// Setting up client configuration
		DiscordConfiguration cfg = new DiscordConfiguration
		{
			Token = Config.token,
			TokenType = TokenType.Bot,
			MinimumLogLevel = Config.logLevel,
			AutoReconnect = true,
			Intents = DiscordIntents.All
		};

		discordClient = new DiscordClient(cfg);

		Logger.Log("Hooking events...");
		discordClient.Ready += EventHandler.OnReady;
		discordClient.GuildAvailable += EventHandler.OnGuildAvailable;
		discordClient.ClientErrored += EventHandler.OnClientError;
		discordClient.MessageCreated += EventHandler.OnMessageCreated;

		Logger.Log("Connecting to Discord...");
		await discordClient.ConnectAsync();
	}
}