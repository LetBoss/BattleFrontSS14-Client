using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Serializable]
[NetSerializable]
public enum InstallationStatus
{
	Cartridge,
	Installed,
	Readonly
}
