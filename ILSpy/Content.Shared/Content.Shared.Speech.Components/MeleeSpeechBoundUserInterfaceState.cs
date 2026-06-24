using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Speech.Components;

[Serializable]
[NetSerializable]
public sealed class MeleeSpeechBoundUserInterfaceState : BoundUserInterfaceState
{
	public string CurrentBattlecry { get; }

	public MeleeSpeechBoundUserInterfaceState(string currentBattlecry)
	{
		CurrentBattlecry = currentBattlecry;
	}
}
