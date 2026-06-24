using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Speech.Components;

[Serializable]
[NetSerializable]
public sealed class MeleeSpeechBattlecryChangedMessage : BoundUserInterfaceMessage
{
	public string Battlecry { get; }

	public MeleeSpeechBattlecryChangedMessage(string battlecry)
	{
		Battlecry = battlecry;
	}
}
