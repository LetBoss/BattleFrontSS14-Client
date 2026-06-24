using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole;

[Serializable]
[NetSerializable]
public enum UiButton : byte
{
	Clone,
	Eject
}
