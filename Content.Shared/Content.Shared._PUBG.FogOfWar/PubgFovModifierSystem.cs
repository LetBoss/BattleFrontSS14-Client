using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._PUBG.FogOfWar;

public sealed class PubgFovModifierSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgFovModifierComponent, InventoryRelayedEvent<GetPubgFovEvent>>((EntityEventRefHandler<PubgFovModifierComponent, InventoryRelayedEvent<GetPubgFovEvent>>)OnGetFov, (Type[])null, (Type[])null);
	}

	private void OnGetFov(Entity<PubgFovModifierComponent> ent, ref InventoryRelayedEvent<GetPubgFovEvent> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.Args.Modifier += ent.Comp.ModifierDegrees;
	}

	public float GetEffectiveFov(EntityUid uid, PubgFogOfWarComponent? fog = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgFogOfWarComponent>(uid, ref fog, false))
		{
			return 0f;
		}
		GetPubgFovEvent ev = new GetPubgFovEvent(fog.FovDegrees);
		InventoryComponent inventory = default(InventoryComponent);
		if (((EntitySystem)this).TryComp<InventoryComponent>(uid, ref inventory))
		{
			_inventory.RelayEvent(Entity<InventoryComponent>.op_Implicit((uid, inventory)), ref ev);
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent<GetPubgFovEvent>(uid, ev, false);
		}
		return Math.Clamp(ev.BaseFov + ev.Modifier, 1f, 360f);
	}
}
