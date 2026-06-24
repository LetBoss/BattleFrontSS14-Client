using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public enum NewsReaderUiAction
{
	Next,
	Prev,
	NotificationSwitch
}
