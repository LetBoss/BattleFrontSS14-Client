using Robust.Shared.GameObjects;

namespace Content.Shared.Chat.V2;

public interface IChatEvent
{
	EntityUid Sender { get; }

	uint Id { get; set; }

	string Message { get; set; }

	MessageType Type { get; }
}
