using System;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Weapons.Ranged.Magazine;

public sealed class RMCMagazineSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCMagazineVisualsComponent, MapInitEvent>((EntityEventRefHandler<RMCMagazineVisualsComponent, MapInitEvent>)OnMagazineInit, (Type[])null, new Type[1] { typeof(SharedGunSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCMagazineVisualsComponent, GunShotEvent>((EntityEventRefHandler<RMCMagazineVisualsComponent, GunShotEvent>)OnMagazineGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagazineVisualsComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCMagazineVisualsComponent, EntInsertedIntoContainerMessage>)OnMagazineSlotInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMagazineVisualsComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCMagazineVisualsComponent, EntRemovedFromContainerMessage>)OnMagazineSlotRemoved, (Type[])null, (Type[])null);
	}

	private void OnMagazineInit(Entity<RMCMagazineVisualsComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagazine(Entity<RMCMagazineVisualsComponent>.op_Implicit(ent));
	}

	private void OnMagazineGunShot(Entity<RMCMagazineVisualsComponent> ent, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagazine(Entity<RMCMagazineVisualsComponent>.op_Implicit(ent));
	}

	private void OnMagazineSlotInserted(Entity<RMCMagazineVisualsComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagazine(Entity<RMCMagazineVisualsComponent>.op_Implicit(ent));
	}

	private void OnMagazineSlotRemoved(Entity<RMCMagazineVisualsComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagazine(Entity<RMCMagazineVisualsComponent>.op_Implicit(ent));
	}

	public void UpdateMagazine(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			GetAmmoCountEvent ammoCountEvent = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(uid, ref ammoCountEvent, false);
			_appearance.SetData(uid, (Enum)RMCMagazineVisuals.SlideOpen, (object)(ammoCountEvent.Count <= 0), appearance);
		}
	}
}
