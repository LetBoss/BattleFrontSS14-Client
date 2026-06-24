using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class PlayerPanelFreezeMessage : EuiMessageBase
{
	public readonly bool Mute;

	public PlayerPanelFreezeMessage(bool mute = false)
	{
		Mute = mute;
	}
}
