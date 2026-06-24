using System;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Bombard;

public sealed class XenoBombardSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCProjectileSystem _rmcProjectile;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoBombardComponent, XenoBombardActionEvent>((EntityEventRefHandler<XenoBombardComponent, XenoBombardActionEvent>)OnBombard, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBombardComponent, DoAfterAttemptEvent<XenoBombardDoAfterEvent>>((EntityEventRefHandler<XenoBombardComponent, DoAfterAttemptEvent<XenoBombardDoAfterEvent>>)OnBombardDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBombardComponent, XenoBombardDoAfterEvent>((EntityEventRefHandler<XenoBombardComponent, XenoBombardDoAfterEvent>)OnBombardDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBombardComponent, XenoGasToggleActionEvent>((EntityEventRefHandler<XenoBombardComponent, XenoGasToggleActionEvent>)OnToggleType, (Type[])null, (Type[])null);
	}

	private void OnBombard(Entity<XenoBombardComponent> ent, ref XenoBombardActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates source = _transform.GetMapCoordinates(Entity<XenoBombardComponent>.op_Implicit(ent), (TransformComponent)null);
		MapCoordinates target = _transform.ToMapCoordinates(args.Target, true);
		if (source.MapId != target.MapId)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), ent.Comp.PlasmaCost))
		{
			Vector2 direction = target.Position - source.Position;
			if (direction.Length() > (float)ent.Comp.Range)
			{
				target = ((MapCoordinates)(ref source)).Offset(Vector2Helpers.Normalized(direction) * ent.Comp.Range);
			}
			_audio.PlayPredicted(ent.Comp.PrepareSound, Entity<XenoBombardComponent>.op_Implicit(ent), (EntityUid?)Entity<XenoBombardComponent>.op_Implicit(ent), (AudioParams?)null);
			XenoBombardDoAfterEvent ev = new XenoBombardDoAfterEvent
			{
				Coordinates = target
			};
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoBombardComponent>.op_Implicit(ent), ent.Comp.Delay, ev, Entity<XenoBombardComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(args.Action))
			{
				BreakOnMove = true,
				RootEntity = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				_rmcActions.DisableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(args.Action.Owner), Entity<XenoBombardComponent>.op_Implicit(ent));
				string selfMessage = base.Loc.GetString("rmc-glob-start-self");
				_popup.PopupClient(selfMessage, Entity<XenoBombardComponent>.op_Implicit(ent), Entity<XenoBombardComponent>.op_Implicit(ent));
				string othersMessage = base.Loc.GetString("rmc-glob-start-others", (ValueTuple<string, object>)("user", ent));
				_popup.PopupEntity(othersMessage, Entity<XenoBombardComponent>.op_Implicit(ent), Filter.PvsExcept(Entity<XenoBombardComponent>.op_Implicit(ent), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
			}
		}
	}

	private void OnBombardDoAfterAttempt(Entity<XenoBombardComponent> ent, ref DoAfterAttemptEvent<XenoBombardDoAfterEvent> args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Event.Target;
		if (target.HasValue)
		{
			EntityUid action = target.GetValueOrDefault();
			ActionComponent actionComponent = default(ActionComponent);
			if (((EntitySystem)this).HasComp<InstantActionComponent>(action) && ((EntitySystem)this).TryComp<ActionComponent>(action, ref actionComponent) && !actionComponent.Enabled)
			{
				_rmcActions.EnableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoBombardComponent>.op_Implicit(ent));
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnBombardDoAfter(Entity<XenoBombardComponent> ent, ref XenoBombardDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid action = target.GetValueOrDefault();
		_rmcActions.EnableSharedCooldownEvents(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoBombardComponent>.op_Implicit(ent));
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), ent.Comp.PlasmaCost) && !_net.IsClient)
		{
			MapCoordinates source = _transform.GetMapCoordinates(Entity<XenoBombardComponent>.op_Implicit(ent), (TransformComponent)null);
			if (!(source.MapId != args.Coordinates.MapId))
			{
				Vector2 direction = args.Coordinates.Position - source.Position;
				EntityUid projectile = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.Projectile), source, (ComponentRegistry)null, default(Angle));
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(projectile));
				ProjectileMaxRangeComponent max = ((EntitySystem)this).EnsureComp<ProjectileMaxRangeComponent>(projectile);
				_rmcProjectile.SetMaxRange(Entity<ProjectileMaxRangeComponent>.op_Implicit((projectile, max)), direction.Length());
				_gun.ShootProjectile(projectile, direction, Vector2.Zero, Entity<XenoBombardComponent>.op_Implicit(ent), Entity<XenoBombardComponent>.op_Implicit(ent), 7.5f);
				_audio.PlayEntity(ent.Comp.ShootSound, Entity<XenoBombardComponent>.op_Implicit(ent), Entity<XenoBombardComponent>.op_Implicit(ent), (AudioParams?)null);
				_rmcActions.ActivateSharedCooldown(Entity<ActionSharedCooldownComponent>.op_Implicit(action), Entity<XenoBombardComponent>.op_Implicit(ent));
				string selfMessage = base.Loc.GetString("rmc-glob-shoot-self");
				_popup.PopupClient(selfMessage, Entity<XenoBombardComponent>.op_Implicit(ent), Entity<XenoBombardComponent>.op_Implicit(ent));
				string othersMessage = base.Loc.GetString("rmc-glob-shoot-others", (ValueTuple<string, object>)("user", ent));
				_popup.PopupEntity(othersMessage, Entity<XenoBombardComponent>.op_Implicit(ent), Filter.PvsExcept(Entity<XenoBombardComponent>.op_Implicit(ent), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
			}
		}
	}

	private void OnToggleType(Entity<XenoBombardComponent> ent, ref XenoGasToggleActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Projectiles.Length != 0)
		{
			int index = Array.IndexOf(ent.Comp.Projectiles, ent.Comp.Projectile);
			index = ((index != -1 && index < ent.Comp.Projectiles.Length - 1) ? (index + 1) : 0);
			ent.Comp.Projectile = ent.Comp.Projectiles[index];
			((EntitySystem)this).Dirty<XenoBombardComponent>(ent, (MetaDataComponent)null);
		}
	}
}
