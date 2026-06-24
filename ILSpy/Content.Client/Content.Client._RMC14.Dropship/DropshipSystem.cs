using System.Collections.Generic;
using Content.Shared._RMC14.Dropship;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Dropship;

public sealed class DropshipSystem : SharedDropshipSystem
{
	public readonly List<DropshipNavigationBui> Uis = new List<DropshipNavigationBui>();

	public override void FrameUpdate(float frameTime)
	{
		foreach (DropshipNavigationBui ui in Uis)
		{
			((BoundUserInterface)ui).Update();
		}
	}
}
