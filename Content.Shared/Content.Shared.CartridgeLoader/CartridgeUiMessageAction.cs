using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Serializable]
[NetSerializable]
public enum CartridgeUiMessageAction
{
	Activate,
	Deactivate,
	Install,
	Uninstall,
	UIReady
}
