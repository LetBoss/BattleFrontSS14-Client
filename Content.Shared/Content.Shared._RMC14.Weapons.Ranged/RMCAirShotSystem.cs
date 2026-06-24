using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared.CombatMode;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCAirShotSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedCombatModeSystem _combat;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private RMCCameraShakeSystem _cameraShake;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedDropshipWeaponSystem _dropship;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCAirShotComponent, UniqueActionEvent>((EntityEventRefHandler<RMCAirShotComponent, UniqueActionEvent>)OnUniqueAction, new Type[1] { typeof(CMGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAirShotComponent, AirShotDoAfterEvent>((EntityEventRefHandler<RMCAirShotComponent, AirShotDoAfterEvent>)OnAirShotDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAirShotComponent, ExaminedEvent>((EntityEventRefHandler<RMCAirShotComponent, ExaminedEvent>)OnAirShotExamined, (Type[])null, (Type[])null);
	}

	private void OnUniqueAction(Entity<RMCAirShotComponent> ent, ref UniqueActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && (!ent.Comp.RequiresCombat || _combat.IsInCombatMode(args.UserUid)) && (ent.Comp.RequiredSkills == null || _skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.UserUid), ent.Comp.RequiredSkills)) && !((EntitySystem)this).HasComp<DevouredComponent>(args.UserUid))
		{
			AttemptAirShot(ent, args.UserUid);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAirShotDoAfter(Entity<RMCAirShotComponent> ent, ref AirShotDoAfterEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (args.DoAfter.Cancelled || !((EntitySystem)this).TryComp<GunComponent>(Entity<RMCAirShotComponent>.op_Implicit(ent), ref gun))
		{
			return;
		}
		if (_net.IsServer)
		{
			EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
			TakeAmmoEvent ev = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), coordinates, args.User);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(Entity<RMCAirShotComponent>.op_Implicit(ent), ev, false);
			RMCAirProjectileComponent projectile = default(RMCAirProjectileComponent);
			foreach (var item in ev.Ammo)
			{
				EntityUid? casing = item.Entity;
				if (((EntitySystem)this).TryComp<RMCAirProjectileComponent>(casing, ref projectile))
				{
					EntProtoId? prototype = projectile.Prototype;
					EntityUid spawned = ((EntitySystem)this).Spawn(prototype.HasValue ? EntProtoId.op_Implicit(prototype.GetValueOrDefault()) : null, ((EntitySystem)this).GetCoordinates(args.Coordinates));
					if (((EntitySystem)this).HasComp<FlareSignalComponent>(spawned))
					{
						int id = _dropship.ComputeNextId();
						string flareIdentifier = _dropship.GetUserAbbreviation(args.User, id);
						_dropship.MakeDropshipTarget(spawned, flareIdentifier);
						ent.Comp.LastFlareId = flareIdentifier;
						((EntitySystem)this).Dirty<RMCAirShotComponent>(ent, (MetaDataComponent)null);
					}
				}
				((EntitySystem)this).Del(casing);
			}
		}
		_audio.PlayPredicted(gun.SoundGunshotModified, Entity<RMCAirShotComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		if (ent.Comp.ShakeAmount > 0)
		{
			Filter players = Filter.Pvs(args.User, 0.35f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
			List<ICommonSession> toRemove = new List<ICommonSession>();
			string selfMessage = base.Loc.GetString("rmc-gun-shoot-air-self", (ValueTuple<string, object>)("weapon", ent));
			_popup.PopupClient(selfMessage, args.User, args.User, PopupType.LargeCaution);
			ICommonSession session = default(ICommonSession);
			RMCSizeComponent size = default(RMCSizeComponent);
			foreach (ICommonSession player in players.Recipients)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (!attachedEntity.HasValue)
				{
					continue;
				}
				EntityUid playerEnt = attachedEntity.GetValueOrDefault();
				if (!_player.TryGetSessionByEntity(args.User, ref session) || session != player)
				{
					string othersMessage = base.Loc.GetString("rmc-gun-shoot-air-other", (ValueTuple<string, object>)("user", Identity.Name(args.User, (IEntityManager)(object)base.EntityManager, playerEnt)), (ValueTuple<string, object>)("weapon", Identity.Name(Entity<RMCAirShotComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, playerEnt)));
					_popup.PopupEntity(othersMessage, args.User, player, PopupType.LargeCaution);
					if (player.AttachedEntity.HasValue && ((EntitySystem)this).TryComp<RMCSizeComponent>(player.AttachedEntity.Value, ref size) && size.Size != RMCSizes.Humanoid)
					{
						toRemove.Add(player);
					}
				}
			}
			players.RemovePlayers((IEnumerable<ICommonSession>)toRemove);
			if (_net.IsServer)
			{
				_cameraShake.ShakeCamera(players, ent.Comp.ShakeAmount, ent.Comp.ShakeStrength);
			}
		}
		UpdateClientAmmoEvent ammoEv = new UpdateClientAmmoEvent(-1);
		((EntitySystem)this).RaiseLocalEvent<UpdateClientAmmoEvent>(Entity<RMCAirShotComponent>.op_Implicit(ent), ref ammoEv, false);
	}

	private void OnAirShotExamined(Entity<RMCAirShotComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RMCAirShotComponent", 5))
		{
			if (ent.Comp.RequiredSkills == null || _skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.Examiner), ent.Comp.RequiredSkills))
			{
				args.PushMarkup(base.Loc.GetString("rmc-gun-shoot-air-examine", (ValueTuple<string, object>)("harm", ent.Comp.RequiresCombat)));
			}
			string id = ent.Comp.LastFlareId;
			if (id != null)
			{
				args.PushMarkup(base.Loc.GetString("rmc-flare-gun-examine", (ValueTuple<string, object>)("id", id)));
			}
		}
	}

	private void AttemptAirShot(Entity<RMCAirShotComponent> ent, EntityUid shooter)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		GetAmmoCountEvent ammoCountEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(Entity<RMCAirShotComponent>.op_Implicit(ent), ref ammoCountEv, false);
		if (ammoCountEv.Count > 0)
		{
			EntityCoordinates shooterCoordinates = _transform.GetMoverCoordinates(shooter);
			if (!ent.Comp.IgnoreRoof && !_area.CanCAS(shooterCoordinates))
			{
				string msg = base.Loc.GetString("rmc-gun-shoot-air-blocked");
				_popup.PopupClient(msg, shooterCoordinates, shooter, PopupType.SmallCaution);
				return;
			}
			AirShotDoAfterEvent ev = new AirShotDoAfterEvent(((EntitySystem)this).GetNetCoordinates(shooterCoordinates, (MetaDataComponent)null));
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, shooter, ent.Comp.PreparationTime, ev, Entity<RMCAirShotComponent>.op_Implicit(ent))
			{
				BreakOnMove = ent.Comp.DoAfterBreakOnMove,
				NeedHand = true,
				BreakOnHandChange = true,
				MovementThreshold = 0.5f
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}
}
