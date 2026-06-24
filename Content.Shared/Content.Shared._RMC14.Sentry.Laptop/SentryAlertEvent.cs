using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryAlertEvent : BoundUserInterfaceMessage
{
	public NetEntity Sentry { get; }

	public SentryAlertType AlertType { get; }

	public string Message { get; }

	public string Color { get; }

	public int FontSize { get; }

	public SentryAlertEvent(NetEntity sentry, SentryAlertType alertType, string message, string color, int fontSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Sentry = sentry;
		AlertType = alertType;
		Message = message;
		Color = color;
		FontSize = fontSize;
	}
}
