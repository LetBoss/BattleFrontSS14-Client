using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.Construction.Upgrades;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Climbing.Events;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Barricade;

public abstract class SharedBarbedSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedStackSystem _stacks;

	[Dependency]
	private SharedToolSystem _toolSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedXenoAcidSystem _xenoAcid;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, ExaminedEvent>((EntityEventRefHandler<BarbedComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, AttackedEvent>((EntityEventRefHandler<BarbedComponent, AttackedEvent>)OnAttacked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, InteractUsingEvent>((EntityEventRefHandler<BarbedComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, DoAfterAttemptEvent<BarbedDoAfterEvent>>((EntityEventRefHandler<BarbedComponent, DoAfterAttemptEvent<BarbedDoAfterEvent>>)OnDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, BarbedDoAfterEvent>((EntityEventRefHandler<BarbedComponent, BarbedDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, CutBarbedDoAfterEvent>((EntityEventRefHandler<BarbedComponent, CutBarbedDoAfterEvent>)WireCutterOnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, DoorStateChangedEvent>((EntityEventRefHandler<BarbedComponent, DoorStateChangedEvent>)OnDoorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, AttemptClimbEvent>((EntityEventRefHandler<BarbedComponent, AttemptClimbEvent>)OnClimbAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, CMGetArmorPiercingEvent>((EntityEventRefHandler<BarbedComponent, CMGetArmorPiercingEvent>)OnGetArmorPiercing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, RMCConstructionUpgradedEvent>((EntityEventRefHandler<BarbedComponent, RMCConstructionUpgradedEvent>)OnConstructionUpgraded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BarbedComponent, XenoLeapHitAttempt>((EntityEventRefHandler<BarbedComponent, XenoLeapHitAttempt>)OnXenoLeapHitAttempt, (Type[])null, new Type[1] { typeof(XenoLeapSystem) });
	}

	private void OnExamined(Entity<BarbedComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.IsBarbed)
		{
			return;
		}
		using (args.PushGroup("SharedBarbedSystem"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-barricade-examine-barbed"));
		}
	}

	private void OnAttacked(Entity<BarbedComponent> barbed, ref AttackedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (barbed.Comp.IsBarbed)
		{
			_damageableSystem.TryChangeDamage(args.User, barbed.Comp.ThornsDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<BarbedComponent>.op_Implicit(barbed), Entity<BarbedComponent>.op_Implicit(barbed));
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-damage"), Entity<BarbedComponent>.op_Implicit(barbed), args.User, PopupType.SmallCaution);
		}
	}

	private void OnInteractUsing(Entity<BarbedComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		ToolComponent tool = default(ToolComponent);
		if (_xenoAcid.IsMelted(Entity<BarbedComponent>.op_Implicit(ent)))
		{
			string failPopup = base.Loc.GetString("rmc-construction-melted");
			_popupSystem.PopupClient(failPopup, Entity<BarbedComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
		}
		else if (!ent.Comp.IsBarbed && ((EntitySystem)this).HasComp<BarbedWireComponent>(args.Used))
		{
			BarbedDoAfterEvent ev = new BarbedDoAfterEvent();
			DoAfterArgs barbDoAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.WireTime, ev, Entity<BarbedComponent>.op_Implicit(ent), Entity<BarbedComponent>.op_Implicit(ent), args.Used)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = true,
				AttemptFrequency = AttemptFrequency.EveryTick,
				CancelDuplicate = false,
				DuplicateCondition = DuplicateConditions.SameTarget
			};
			if (_doAfterSystem.TryStartDoAfter(barbDoAfter))
			{
				((HandledEntityEventArgs)args).Handled = true;
				_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-slot-wiring"), Entity<BarbedComponent>.op_Implicit(ent), args.User);
			}
		}
		else if (ent.Comp.IsBarbed && ((EntitySystem)this).HasComp<BarbedWireComponent>(args.Used))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-slot-insert-full"), Entity<BarbedComponent>.op_Implicit(ent), args.User);
		}
		else if (ent.Comp.IsBarbed && ((EntitySystem)this).TryComp<ToolComponent>(args.Used, ref tool) && _toolSystem.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RemoveQuality), tool))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-cutting-action-begin"), Entity<BarbedComponent>.op_Implicit(ent), args.User);
			EntityManager entityManager = base.EntityManager;
			EntityUid user = args.User;
			TimeSpan cutTime = ent.Comp.CutTime;
			CutBarbedDoAfterEvent cutBarbedDoAfterEvent = new CutBarbedDoAfterEvent();
			EntityUid? eventTarget = Entity<BarbedComponent>.op_Implicit(ent);
			EntityUid? used = args.Used;
			DoAfterArgs cutDoAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, cutTime, cutBarbedDoAfterEvent, eventTarget, null, used)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = true
			};
			_doAfterSystem.TryStartDoAfter(cutDoAfter);
		}
	}

	private void OnDoAfterAttempt(Entity<BarbedComponent> barbed, ref DoAfterAttemptEvent<BarbedDoAfterEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (barbed.Comp.IsBarbed)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDoAfter(Entity<BarbedComponent> barbed, ref BarbedDoAfterEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (args.Used.HasValue && !args.Cancelled && !((HandledEntityEventArgs)args).Handled && !barbed.Comp.IsBarbed)
		{
			((HandledEntityEventArgs)args).Handled = true;
			StackComponent stackComp = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(args.Used.Value, ref stackComp))
			{
				_stacks.Use(args.Used.Value, 1, stackComp);
			}
			barbed.Comp.IsBarbed = true;
			((EntitySystem)this).Dirty<BarbedComponent>(barbed, (MetaDataComponent)null);
			UpdateBarricade(barbed, updateBarbed: true);
			_audio.PlayPredicted(barbed.Comp.BarbSound, barbed.Owner, (EntityUid?)args.User, (AudioParams?)null);
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-slot-insert-success"), barbed.Owner, args.User);
		}
	}

	private void WireCutterOnDoAfter(Entity<BarbedComponent> barbed, ref CutBarbedDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			barbed.Comp.IsBarbed = false;
			((EntitySystem)this).Dirty<BarbedComponent>(barbed, (MetaDataComponent)null);
			UpdateBarricade(barbed, updateBarbed: true);
			_audio.PlayPredicted(barbed.Comp.CutSound, barbed.Owner, (EntityUid?)args.User, (AudioParams?)null);
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-cutting-action-finish"), barbed.Owner, args.User);
			if (!_netManager.IsClient)
			{
				EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<BarbedComponent>.op_Implicit(barbed));
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(barbed.Comp.Spawn), coordinates);
			}
		}
	}

	private void OnDoorStateChanged(Entity<BarbedComponent> barbed, ref DoorStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateBarricade(barbed);
	}

	private void OnClimbAttempt(Entity<BarbedComponent> barbed, ref AttemptClimbEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (barbed.Comp.IsBarbed)
		{
			args.Cancelled = true;
			_popupSystem.PopupClient(base.Loc.GetString("barbed-wire-cant-climb"), barbed.Owner, args.User);
		}
	}

	private void OnGetArmorPiercing(Entity<BarbedComponent> barbed, ref CMGetArmorPiercingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (barbed.Comp.IsBarbed)
		{
			args.Piercing = 1000;
		}
	}

	private void OnConstructionUpgraded(Entity<BarbedComponent> barbed, ref RMCConstructionUpgradedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		BarbedComponent newComp = ((EntitySystem)this).EnsureComp<BarbedComponent>(args.New);
		newComp.IsBarbed = barbed.Comp.IsBarbed;
		((EntitySystem)this).Dirty(args.New, (IComponent)(object)newComp, (MetaDataComponent)null);
		UpdateBarricade(Entity<BarbedComponent>.op_Implicit((args.New, newComp)), updateBarbed: true);
	}

	private void OnXenoLeapHitAttempt(Entity<BarbedComponent> ent, ref XenoLeapHitAttempt args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsBarbed)
		{
			_damageableSystem.TryChangeDamage(args.Leaper, ent.Comp.ThornsDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<BarbedComponent>.op_Implicit(ent), Entity<BarbedComponent>.op_Implicit(ent));
		}
	}

	protected void UpdateBarricade(Entity<BarbedComponent> barbed, bool updateBarbed = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		DoorComponent door = default(DoorComponent);
		bool open = ((EntitySystem)this).TryComp<DoorComponent>(Entity<BarbedComponent>.op_Implicit(barbed), ref door) && door.State == DoorState.Open;
		BarbedWireVisuals barbedWireVisuals = (barbed.Comp.IsBarbed ? ((!open) ? BarbedWireVisuals.WiredClosed : BarbedWireVisuals.WiredOpen) : BarbedWireVisuals.UnWired);
		BarbedWireVisuals visual = barbedWireVisuals;
		if (updateBarbed)
		{
			BarbedStateChangedEvent ev = default(BarbedStateChangedEvent);
			((EntitySystem)this).RaiseLocalEvent<BarbedStateChangedEvent>(Entity<BarbedComponent>.op_Implicit(barbed), ref ev, false);
		}
		Fixture fixture = _fixture.GetFixtureOrNull(Entity<BarbedComponent>.op_Implicit(barbed), barbed.Comp.FixtureId, (FixturesComponent)null);
		if (fixture != null)
		{
			if (barbed.Comp.IsBarbed)
			{
				_physics.AddCollisionLayer(Entity<BarbedComponent>.op_Implicit(barbed), barbed.Comp.FixtureId, fixture, 33554432, (FixturesComponent)null, (PhysicsComponent)null);
			}
			else
			{
				_physics.RemoveCollisionLayer(Entity<BarbedComponent>.op_Implicit(barbed), barbed.Comp.FixtureId, fixture, 33554432, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
		_appearance.SetData(Entity<BarbedComponent>.op_Implicit(barbed), (Enum)BarbedWireVisualLayers.Wire, (object)visual, (AppearanceComponent)null);
	}
}
