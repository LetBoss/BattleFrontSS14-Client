using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Holopad;

[Serializable]
[NetSerializable]
public enum HolopadUiKey : byte
{
	InteractionWindow,
	InteractionWindowForAi,
	AiActionWindow,
	AiRequestWindow
}
