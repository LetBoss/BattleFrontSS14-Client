using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedSpaceNinjaSystem : EntitySystem
{
	[Dependency]
	protected SharedNinjaSuitSystem Suit;

	[Dependency]
	protected SharedPopupSystem Popup;

	public EntityQuery<SpaceNinjaComponent> NinjaQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		NinjaQuery = ((EntitySystem)this).GetEntityQuery<SpaceNinjaComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SpaceNinjaComponent, AttackedEvent>((EntityEventRefHandler<SpaceNinjaComponent, AttackedEvent>)OnNinjaAttacked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpaceNinjaComponent, MeleeAttackEvent>((EntityEventRefHandler<SpaceNinjaComponent, MeleeAttackEvent>)OnNinjaAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpaceNinjaComponent, ShotAttemptedEvent>((EntityEventRefHandler<SpaceNinjaComponent, ShotAttemptedEvent>)OnShotAttempted, (Type[])null, (Type[])null);
	}

	public bool IsNinja([NotNullWhen(true)] EntityUid? uid)
	{
		return NinjaQuery.HasComp(uid);
	}

	public void AssignSuit(Entity<SpaceNinjaComponent> ent, EntityUid? suit)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? suit2 = ent.Comp.Suit;
		EntityUid? val = suit;
		if (suit2.HasValue != val.HasValue || (suit2.HasValue && !(suit2.GetValueOrDefault() == val.GetValueOrDefault())))
		{
			ent.Comp.Suit = suit;
			((EntitySystem)this).Dirty(Entity<SpaceNinjaComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	public void AssignGloves(Entity<SpaceNinjaComponent> ent, EntityUid? gloves)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gloves2 = ent.Comp.Gloves;
		EntityUid? val = gloves;
		if (gloves2.HasValue != val.HasValue || (gloves2.HasValue && !(gloves2.GetValueOrDefault() == val.GetValueOrDefault())))
		{
			ent.Comp.Gloves = gloves;
			((EntitySystem)this).Dirty(Entity<SpaceNinjaComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	public void BindKatana(Entity<SpaceNinjaComponent?> ent, EntityUid katana)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (NinjaQuery.Resolve(Entity<SpaceNinjaComponent>.op_Implicit(ent), ref ent.Comp, false) && !ent.Comp.Katana.HasValue)
		{
			ent.Comp.Katana = katana;
			((EntitySystem)this).Dirty(Entity<SpaceNinjaComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	public virtual bool TryUseCharge(EntityUid user, float charge)
	{
		return false;
	}

	private void OnNinjaAttacked(Entity<SpaceNinjaComponent> ent, ref AttackedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryRevealNinja(ent, disable: true);
	}

	private void OnNinjaAttack(Entity<SpaceNinjaComponent> ent, ref MeleeAttackEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryRevealNinja(ent, disable: false);
	}

	private void TryRevealNinja(Entity<SpaceNinjaComponent> ent, bool disable)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? suit = ent.Comp.Suit;
		if (suit.HasValue)
		{
			EntityUid uid = suit.GetValueOrDefault();
			NinjaSuitComponent suit2 = default(NinjaSuitComponent);
			if (((EntitySystem)this).TryComp<NinjaSuitComponent>(ent.Comp.Suit, ref suit2))
			{
				Suit.RevealNinja(Entity<NinjaSuitComponent>.op_Implicit((uid, suit2)), Entity<SpaceNinjaComponent>.op_Implicit(ent), disable);
			}
		}
	}

	private void OnShotAttempted(Entity<SpaceNinjaComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Popup.PopupClient(base.Loc.GetString("gun-disabled"), Entity<SpaceNinjaComponent>.op_Implicit(ent), Entity<SpaceNinjaComponent>.op_Implicit(ent));
		args.Cancel();
	}
}
