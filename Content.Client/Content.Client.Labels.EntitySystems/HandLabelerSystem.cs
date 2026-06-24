using System;
using Content.Client.Labels.UI;
using Content.Shared.Labels;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.GameObjects;

namespace Content.Client.Labels.EntitySystems;

public sealed class HandLabelerSystem : SharedHandLabelerSystem
{
	protected override void UpdateUI(Entity<HandLabelerComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (UserInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)HandLabelerUiKey.Key, ref val) && val is HandLabelerBoundUserInterface handLabelerBoundUserInterface)
		{
			handLabelerBoundUserInterface.Reload();
		}
	}
}
