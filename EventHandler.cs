using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;

namespace BullyingBoi;

internal static class EventHandler
{
	internal static Task OnReady(DiscordClient client, ReadyEventArgs e)
	{
		Logger.Log("Client is ready to process events.");

		// Checking activity type
		if (!Enum.TryParse(Config.presenceType, true, out ActivityType activityType))
		{
			Logger.Log("Presence type '" + Config.presenceType + "' invalid, using 'Playing' instead.");
			activityType = ActivityType.Playing;
		}

		client.UpdateStatusAsync(new DiscordActivity(Config.presenceText, activityType), UserStatus.Online);
		return Task.CompletedTask;
	}

	internal static Task OnGuildAvailable(DiscordClient _, GuildCreateEventArgs e)
	{
		Logger.Log("Guild available: " + e.Guild.Name);
		return Task.CompletedTask;
	}

	internal static Task OnClientError(DiscordClient _, ClientErrorEventArgs e)
	{
		Logger.Error("Client exception occured:\n" + e.Exception);
		switch (e.Exception)
		{
			case BadRequestException ex:
				Logger.Error("JSON Message: " + ex.JsonMessage);
				break;
		}
		return Task.CompletedTask;
	}

	internal static async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
	{
		Logger.Log("TEST" + Config.bullyingID + " " + e.Message.Author.Id);
		if (e.Message.Author.Id == Config.bullyingID)
		{
			Logger.Log("TEST2");
			await e.Message.CreateReactionAsync(DiscordEmoji.FromName(client, Config.emoteName));
		}
	}
}