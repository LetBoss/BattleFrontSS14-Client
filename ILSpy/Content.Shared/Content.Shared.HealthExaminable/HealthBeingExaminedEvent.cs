using Robust.Shared.Utility;

namespace Content.Shared.HealthExaminable;

public sealed class HealthBeingExaminedEvent
{
	public FormattedMessage Message;

	public HealthBeingExaminedEvent(FormattedMessage message)
	{
		Message = message;
	}
}
