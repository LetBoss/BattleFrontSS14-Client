using System;
using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.PDA.Ringer;

public sealed class RingerSystem : SharedRingerSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RingerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RingerComponent, AfterAutoHandleStateEvent>)OnRingerUpdate, (Type[])null, (Type[])null);
	}

	private void OnRingerUpdate(Entity<RingerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateRingerUi(ent);
	}

	protected override void UpdateRingerUi(Entity<RingerComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RingerUiKey.Key, ref val))
		{
			val.Update();
		}
	}
}
