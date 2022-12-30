using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using BullyingBoi.Properties;
using YamlDotNet.Serialization;

namespace BullyingBoi;

internal static class Config
{
	internal static string token = "";
	internal static LogLevel logLevel = LogLevel.Information;
	internal static string presenceType = "Playing";
	internal static string presenceText = "";
	internal static ulong bullyingID = 0;
	internal static string emoteName = "nerd";

	public static void LoadConfig()
	{
		// Writes default config to file if it does not already exist
		if (!File.Exists("./config.yml"))
		{
			File.WriteAllText("./config.yml", Encoding.UTF8.GetString(Resources.default_config));
		}

		// Reads config contents into FileStream
		FileStream stream = File.OpenRead("./config.yml");

		// Converts the FileStream into a YAML object
		IDeserializer deserializer = new DeserializerBuilder().Build();
		object yamlObject = deserializer.Deserialize(new StreamReader(stream)) ?? "";

		// Converts the YAML object into a JSON object as the YAML ones do not support traversal or selection of nodes by name
		ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
		JObject json = JObject.Parse(serializer.Serialize(yamlObject));

		// Sets up the bot
		token = json.SelectToken("bot.token")?.Value<string>() ?? "";
		string stringLogLevel = json.SelectToken("bot.console-log-level")?.Value<string>() ?? "";

		if (!Enum.TryParse(stringLogLevel, true, out logLevel))
		{
			logLevel = LogLevel.Information;
			Logger.Warn("Log level '" + stringLogLevel + "' invalid, using 'Information' instead.");
		}

		presenceType = json.SelectToken("bot.presence-type")?.Value<string>() ?? "Playing";
		presenceText = json.SelectToken("bot.presence-text")?.Value<string>() ?? "";
		bullyingID =json.SelectToken("bot.bullying-target")?.Value<ulong>() ?? 0;
		emoteName = json.SelectToken("bot.emote-name")?.Value<string>() ?? "nerd";
	}
}