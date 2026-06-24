using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.TypingIndicator;

[Prototype(null, 1)]
public sealed class TypingIndicatorPrototype : IPrototype
{
	[DataField("spritePath", false, 1, false, false, null)]
	public ResPath SpritePath = new ResPath("/Textures/Effects/speech.rsi");

	[DataField("typingState", false, 1, true, false, null)]
	public string TypingState;

	[DataField("idleState", false, 1, true, false, null)]
	public string IdleState;

	[DataField("offset", false, 1, false, false, null)]
	public Vector2 Offset = new Vector2(0f, 0f);

	[DataField("shader", false, 1, false, false, null)]
	public string Shader = "shaded";

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
