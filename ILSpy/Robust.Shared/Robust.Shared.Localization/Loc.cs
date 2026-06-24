using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.IoC;

namespace Robust.Shared.Localization;

public static class Loc
{
	private static ILocalizationManager LocalizationManager => IoCManager.Resolve<ILocalizationManager>();

	public static string GetString(string messageId)
	{
		return LocalizationManager.GetString(messageId);
	}

	[Obsolete("Use ILocalizationManager")]
	public static bool TryGetString(string messageId, [NotNullWhen(true)] out string? message)
	{
		return LocalizationManager.TryGetString(messageId, out message);
	}

	public static string GetString(string messageId, params (string, object)[] args)
	{
		return LocalizationManager.GetString(messageId, args);
	}

	[Obsolete("Use ILocalizationManager")]
	public static bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] args)
	{
		return LocalizationManager.TryGetString(messageId, out value, args);
	}
}
