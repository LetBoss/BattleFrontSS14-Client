using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Wieldable.Components;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Wieldable;

public sealed class RMCWieldableSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private UseDelaySystem _useDelaySystem;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedContainerSystem _container;

	private const string WieldUseDelayId = "RMCWieldDelay";

	private static readonly EntProtoId<SkillDefinitionComponent> WieldSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillFirearms");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, GotEquippedHandEvent>((EntityEventRefHandler<WieldableSpeedModifiersComponent, GotEquippedHandEvent>)OnGotEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, GotUnequippedHandEvent>((EntityEventRefHandler<WieldableSpeedModifiersComponent, GotUnequippedHandEvent>)OnGotUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>((EntityEventRefHandler<WieldableSpeedModifiersComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>)OnRefreshMovementSpeedModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, ItemUnwieldedEvent>((EntityEventRefHandler<WieldableSpeedModifiersComponent, ItemUnwieldedEvent>)OnItemUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, ItemWieldedEvent>((EntityEventRefHandler<WieldableSpeedModifiersComponent, ItemWieldedEvent>)OnItemWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableSpeedModifiersComponent, MapInitEvent>((EntityEventRefHandler<WieldableSpeedModifiersComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldDelayComponent, GotEquippedHandEvent>((EntityEventRefHandler<WieldDelayComponent, GotEquippedHandEvent>)OnGotEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldDelayComponent, MapInitEvent>((EntityEventRefHandler<WieldDelayComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldDelayComponent, UseInHandEvent>((EntityEventRefHandler<WieldDelayComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldDelayComponent, ShotAttemptedEvent>((EntityEventRefHandler<WieldDelayComponent, ShotAttemptedEvent>)OnShotAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldDelayComponent, ItemWieldedEvent>((EntityEventRefHandler<WieldDelayComponent, ItemWieldedEvent>)OnItemWieldedWithDelay, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<WieldableSpeedModifiersComponent> wieldable, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent>.op_Implicit((wieldable.Owner, wieldable.Comp)));
	}

	private void OnMapInit(Entity<WieldDelayComponent> wieldable, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		wieldable.Comp.ModifiedDelay = wieldable.Comp.BaseDelay;
		((EntitySystem)this).Dirty<WieldDelayComponent>(wieldable, (MetaDataComponent)null);
	}

	private void OnGotEquippedHand(Entity<WieldableSpeedModifiersComponent> wieldable, ref GotEquippedHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(args.User);
	}

	private void OnGotUnequippedHand(Entity<WieldableSpeedModifiersComponent> wieldable, ref GotUnequippedHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(args.User);
	}

	private void OnRefreshMovementSpeedModifiers(Entity<WieldableSpeedModifiersComponent> wieldable, ref HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Args.ModifySpeed(wieldable.Comp.ModifiedWalk, wieldable.Comp.ModifiedSprint);
	}

	public void RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent?> wieldable)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		wieldable.Comp = ((EntitySystem)this).EnsureComp<WieldableSpeedModifiersComponent>(Entity<WieldableSpeedModifiersComponent>.op_Implicit(wieldable));
		float walkSpeed = wieldable.Comp.Base;
		float sprintSpeed = wieldable.Comp.Base;
		TransformComponent transformComponent = default(TransformComponent);
		if (((EntitySystem)this).TryComp(wieldable.Owner, ref transformComponent))
		{
			EntityUid parentUid = transformComponent.ParentUid;
			RMCArmorSpeedTierUserComponent userComponent = default(RMCArmorSpeedTierUserComponent);
			if (((EntityUid)(ref parentUid)).Valid && ((EntitySystem)this).TryComp<RMCArmorSpeedTierUserComponent>(transformComponent.ParentUid, ref userComponent))
			{
				switch (userComponent.SpeedTier)
				{
				case "light":
					walkSpeed = wieldable.Comp.Light;
					sprintSpeed = wieldable.Comp.Light;
					break;
				case "medium":
					walkSpeed = wieldable.Comp.Medium;
					sprintSpeed = wieldable.Comp.Medium;
					break;
				case "heavy":
					walkSpeed = wieldable.Comp.Heavy;
					sprintSpeed = wieldable.Comp.Heavy;
					break;
				}
			}
		}
		WieldableComponent wieldableComponent = default(WieldableComponent);
		if (!((EntitySystem)this).TryComp<WieldableComponent>(wieldable.Owner, ref wieldableComponent) || !wieldableComponent.Wielded)
		{
			walkSpeed = 1f;
			sprintSpeed = 1f;
		}
		GetWieldableSpeedModifiersEvent ev = new GetWieldableSpeedModifiersEvent(walkSpeed, sprintSpeed);
		((EntitySystem)this).RaiseLocalEvent<GetWieldableSpeedModifiersEvent>(Entity<WieldableSpeedModifiersComponent>.op_Implicit(wieldable), ref ev, false);
		wieldable.Comp.ModifiedWalk = ((ev.Walk > 0f) ? ev.Walk : 0f);
		wieldable.Comp.ModifiedSprint = ((ev.Sprint > 0f) ? ev.Sprint : 0f);
		((EntitySystem)this).Dirty<WieldableSpeedModifiersComponent>(wieldable, (MetaDataComponent)null);
		RefreshModifiersOnParent(wieldable.Owner);
	}

	private void OnItemUnwielded(Entity<WieldableSpeedModifiersComponent> wieldable, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent>.op_Implicit((wieldable.Owner, wieldable.Comp)));
	}

	private void OnItemWielded(Entity<WieldableSpeedModifiersComponent> wieldable, ref ItemWieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent>.op_Implicit((wieldable.Owner, wieldable.Comp)));
	}

	private void RefreshModifiersOnParent(EntityUid wieldableUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(wieldableUid, ref xform))
		{
			return;
		}
		EntityUid parentUid = xform.ParentUid;
		if (!((EntityUid)(ref parentUid)).Valid)
		{
			return;
		}
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(xform.ParentUid));
		if (activeItem.HasValue)
		{
			EntityUid active = activeItem.GetValueOrDefault();
			if (!(active != wieldableUid))
			{
				_movementSpeed.RefreshMovementSpeedModifiers(xform.ParentUid);
			}
		}
	}

	private void OnGotEquippedHand(Entity<WieldDelayComponent> wieldable, ref GotEquippedHandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_useDelaySystem.SetLength(Entity<UseDelayComponent>.op_Implicit(wieldable.Owner), wieldable.Comp.ModifiedDelay, "RMCWieldDelay");
		_useDelaySystem.TryResetDelay(wieldable.Owner, checkDelayed: false, null, "RMCWieldDelay");
	}

	private void OnUseInHand(Entity<WieldDelayComponent> wieldable, ref UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelayComponent = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(wieldable.Owner, ref useDelayComponent) && _useDelaySystem.IsDelayed(Entity<UseDelayComponent>.op_Implicit((wieldable.Owner, useDelayComponent)), "RMCWieldDelay"))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_useDelaySystem.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit((wieldable.Owner, useDelayComponent)), out UseDelayInfo info, "RMCWieldDelay"))
			{
				string time = $"{(info.EndTime - _timing.CurTime).TotalSeconds:F1}";
				_popupSystem.PopupClient(base.Loc.GetString("rmc-wield-use-delay", (ValueTuple<string, object>)("seconds", time), (ValueTuple<string, object>)("wieldable", wieldable.Owner)), args.User, args.User);
			}
		}
	}

	public void RefreshWieldDelay(Entity<WieldDelayComponent?> wieldable)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		wieldable.Comp = ((EntitySystem)this).EnsureComp<WieldDelayComponent>(Entity<WieldDelayComponent>.op_Implicit(wieldable));
		GetWieldDelayEvent ev = new GetWieldDelayEvent(wieldable.Comp.BaseDelay);
		((EntitySystem)this).RaiseLocalEvent<GetWieldDelayEvent>(Entity<WieldDelayComponent>.op_Implicit(wieldable), ref ev, false);
		wieldable.Comp.ModifiedDelay = ((ev.Delay >= TimeSpan.Zero) ? ev.Delay : TimeSpan.Zero);
		((EntitySystem)this).Dirty<WieldDelayComponent>(wieldable, (MetaDataComponent)null);
	}

	private void OnItemWieldedWithDelay(Entity<WieldDelayComponent> wieldable, ref ItemWieldedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan skillModifiedDelay = wieldable.Comp.ModifiedDelay;
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<WieldDelayComponent>.op_Implicit(wieldable), null)), ref container))
		{
			skillModifiedDelay -= TimeSpan.FromSeconds(0.2) * _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(container.Owner), WieldSkill);
		}
		_useDelaySystem.SetLength(Entity<UseDelayComponent>.op_Implicit(wieldable.Owner), skillModifiedDelay, "RMCWieldDelay");
		_useDelaySystem.TryResetDelay(wieldable.Owner, checkDelayed: false, null, "RMCWieldDelay");
	}

	public void OnShotAttempt(Entity<WieldDelayComponent> wieldable, ref ShotAttemptedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelayComponent = default(UseDelayComponent);
		if (wieldable.Comp.PreventFiring && ((EntitySystem)this).TryComp<UseDelayComponent>(wieldable.Owner, ref useDelayComponent) && _useDelaySystem.IsDelayed(Entity<UseDelayComponent>.op_Implicit((wieldable.Owner, useDelayComponent)), "RMCWieldDelay") && _useDelaySystem.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit((wieldable.Owner, useDelayComponent)), out UseDelayInfo info, "RMCWieldDelay"))
		{
			args.Cancel();
			_ = $"{(info.EndTime - _timing.CurTime).TotalSeconds:F1}";
		}
	}
}
