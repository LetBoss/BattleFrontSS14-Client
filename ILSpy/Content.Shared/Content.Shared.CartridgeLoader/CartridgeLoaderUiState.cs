using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Serializable]
[Virtual]
[NetSerializable]
public class CartridgeLoaderUiState : BoundUserInterfaceState
{
	public NetEntity? ActiveUI;

	public List<NetEntity> Programs;

	public CartridgeLoaderUiState(List<NetEntity> programs, NetEntity? activeUI)
	{
		Programs = programs;
		ActiveUI = activeUI;
	}
}
