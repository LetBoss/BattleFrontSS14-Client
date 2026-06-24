using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged.Auto;

public sealed class GunToggleableAutoFireSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private CMGunSystem _rmcGun;

	[Dependency]
	private RMCGunBatterySystem _rmcGunBattery;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<Entity<XenoComponent>> _targets = new HashSet<Entity<XenoComponent>>();

	public readonly PolygonShape Shape = new PolygonShape();

	public bool Debug;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAutoFireComponent, GetItemActionsEvent>((EntityEventRefHandler<GunToggleableAutoFireComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAutoFireComponent, GunToggleableAutoFireActionEvent>((EntityEventRefHandler<GunToggleableAutoFireComponent, GunToggleableAutoFireActionEvent>)OnAutoFireAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableAutoFireComponent, GunGetBatteryDrainEvent>((EntityEventRefHandler<GunToggleableAutoFireComponent, GunGetBatteryDrainEvent>)OnAutoFireGetBatteryDrain, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveGunAutoFireComponent, ComponentRemove>((EntityEventRefHandler<ActiveGunAutoFireComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveGunAutoFireComponent, ItemUnwieldedEvent>((EntityEventRefHandler<ActiveGunAutoFireComponent, ItemUnwieldedEvent>)OnDoRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveGunAutoFireComponent, DroppedEvent>((EntityEventRefHandler<ActiveGunAutoFireComponent, DroppedEvent>)OnDoRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveGunAutoFireComponent, GunUnpoweredEvent>((EntityEventRefHandler<ActiveGunAutoFireComponent, GunUnpoweredEvent>)OnDoRemove, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<GunToggleableAutoFireComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		((EntitySystem)this).Dirty<GunToggleableAutoFireComponent>(ent, (MetaDataComponent)null);
	}

	private void OnAutoFireAction(Entity<GunToggleableAutoFireComponent> ent, ref GunToggleableAutoFireActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.Performer;
		if (!_rmcGun.HasRequiredEquippedPopup(Entity<GunRequireEquippedComponent>.op_Implicit(ent.Owner), user))
		{
			return;
		}
		if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), Entity<GunToggleableAutoFireComponent>.op_Implicit(ent)))
		{
			string msg = base.Loc.GetString("rmc-toggleable-autofire-requires-wielding", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(msg, user, user, PopupType.MediumCaution);
			return;
		}
		WieldableComponent wieldable = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), ref wieldable) && !wieldable.Wielded)
		{
			string msg2 = base.Loc.GetString("rmc-toggleable-autofire-requires-wielding", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(msg2, user, user, PopupType.MediumCaution);
			return;
		}
		_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		ActiveGunAutoFireComponent activeGunAutoFireComponent = default(ActiveGunAutoFireComponent);
		if (((EntitySystem)this).EnsureComp<ActiveGunAutoFireComponent>(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), ref activeGunAutoFireComponent))
		{
			((EntitySystem)this).RemCompDeferred<ActiveGunAutoFireComponent>(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent));
			AutoUpdated(Entity<GunToggleableAutoFireComponent>.op_Implicit((Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), Entity<GunToggleableAutoFireComponent>.op_Implicit(ent))), active: false);
		}
		else
		{
			AutoUpdated(Entity<GunToggleableAutoFireComponent>.op_Implicit((Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), Entity<GunToggleableAutoFireComponent>.op_Implicit(ent))), active: true);
		}
	}

	private void OnAutoFireGetBatteryDrain(Entity<GunToggleableAutoFireComponent> ent, ref GunGetBatteryDrainEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ActiveGunAutoFireComponent>(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent)))
		{
			args.Drain += ent.Comp.BatteryDrain;
		}
	}

	private void OnRemove(Entity<ActiveGunAutoFireComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<ActiveGunAutoFireComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			AutoUpdated(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent.Owner), active: false);
		}
	}

	private void OnDoRemove<T>(Entity<ActiveGunAutoFireComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ActiveGunAutoFireComponent>(Entity<ActiveGunAutoFireComponent>.op_Implicit(ent));
		AutoUpdated(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent.Owner), active: false);
	}

	private void AutoUpdated(Entity<GunToggleableAutoFireComponent?> ent, bool active)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GunToggleableAutoFireComponent>(Entity<GunToggleableAutoFireComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			SharedActionsSystem actions = _actions;
			EntityUid? action = ent.Comp.Action;
			actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), active);
			_rmcGunBattery.RefreshBatteryDrain(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(ent.Owner));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		if (!Debug && _net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveGunAutoFireComponent, GunToggleableAutoFireComponent, GunComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveGunAutoFireComponent, GunToggleableAutoFireComponent, GunComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		ActiveGunAutoFireComponent active = default(ActiveGunAutoFireComponent);
		GunToggleableAutoFireComponent auto = default(GunToggleableAutoFireComponent);
		GunComponent gun = default(GunComponent);
		TransformComponent xform = default(TransformComponent);
		BaseContainer container = default(BaseContainer);
		Box2Rotated box = default(Box2Rotated);
		while (query.MoveNext(ref uid, ref active, ref auto, ref gun, ref xform))
		{
			if (time < active.NextFire)
			{
				continue;
			}
			active.NextFire = time + active.FailCooldown;
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, xform)), ref container) || !_hands.IsHolding(Entity<HandsComponent>.op_Implicit(container.Owner), uid))
			{
				((EntitySystem)this).RemCompDeferred<ActiveGunAutoFireComponent>(uid);
				continue;
			}
			ValueTuple<Vector2, Angle> worldPositionRotation = _transform.GetWorldPositionRotation(xform);
			Vector2 pos = worldPositionRotation.Item1;
			Angle rotation = worldPositionRotation.Item2;
			rotation = DirectionExtensions.ToAngle(((Angle)(ref rotation)).GetCardinalDir());
			pos += ((Angle)(ref rotation)).ToWorldVec() * auto.Range.Y / 2f;
			((Box2Rotated)(ref box))._002Ector(Box2.CenteredAround(pos, Vector2i.op_Implicit(auto.Range)), rotation, pos);
			Transform shapeTransform = Transform.Empty;
			Shape.Set(box);
			_targets.Clear();
			_entityLookup.GetEntitiesIntersecting<XenoComponent, PolygonShape>(xform.MapID, Shape, shapeTransform, _targets, (LookupFlags)78);
			foreach (Entity<XenoComponent> target in _targets)
			{
				if (!_mobState.IsDead(Entity<XenoComponent>.op_Implicit(target)) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(container.Owner), Entity<TransformComponent>.op_Implicit(target.Owner), auto.MaxRange, CollisionGroup.Impassable | CollisionGroup.BulletImpassable))
				{
					if (_net.IsServer)
					{
						_gun.AttemptShoot(container.Owner, uid, gun, target.Owner.ToCoordinates());
					}
					active.NextFire = time + active.Cooldown;
					break;
				}
			}
		}
	}
}
