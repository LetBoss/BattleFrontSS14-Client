using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Localizations;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;

namespace Content.Shared.Weapons.Reflect;

public sealed class ReflectSystem : EntitySystem
{
	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).Subs.SubscribeWithRelay<ReflectComponent, ProjectileReflectAttemptEvent>((EntityEventRefHandler<ReflectComponent, ProjectileReflectAttemptEvent>)OnReflectUserCollide, false, true, true);
		((EntitySystem)this).Subs.SubscribeWithRelay<ReflectComponent, HitScanReflectAttemptEvent>((EntityEventRefHandler<ReflectComponent, HitScanReflectAttemptEvent>)OnReflectUserHitscan, false, true, true);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, ProjectileReflectAttemptEvent>((EntityEventRefHandler<ReflectComponent, ProjectileReflectAttemptEvent>)OnReflectCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, HitScanReflectAttemptEvent>((EntityEventRefHandler<ReflectComponent, HitScanReflectAttemptEvent>)OnReflectHitscan, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, GotEquippedEvent>((EntityEventRefHandler<ReflectComponent, GotEquippedEvent>)OnReflectEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, GotUnequippedEvent>((EntityEventRefHandler<ReflectComponent, GotUnequippedEvent>)OnReflectUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, GotEquippedHandEvent>((EntityEventRefHandler<ReflectComponent, GotEquippedHandEvent>)OnReflectHandEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, GotUnequippedHandEvent>((EntityEventRefHandler<ReflectComponent, GotUnequippedHandEvent>)OnReflectHandUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReflectComponent, ExaminedEvent>((EntityEventRefHandler<ReflectComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnReflectUserCollide(Entity<ReflectComponent> ent, ref ProjectileReflectAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && ent.Comp.InRightPlace && TryReflectProjectile(ent, ent.Owner, Entity<ProjectileComponent>.op_Implicit(args.ProjUid)))
		{
			args.Cancelled = true;
		}
	}

	private void OnReflectUserHitscan(Entity<ReflectComponent> ent, ref HitScanReflectAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Reflected && ent.Comp.InRightPlace && TryReflectHitscan(ent, ent.Owner, args.Shooter, args.SourceItem, args.Direction, args.Reflective, out var dir))
		{
			args.Direction = dir.Value;
			args.Reflected = true;
		}
	}

	private void OnReflectCollide(Entity<ReflectComponent> ent, ref ProjectileReflectAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && TryReflectProjectile(ent, ent.Owner, Entity<ProjectileComponent>.op_Implicit(args.ProjUid)))
		{
			args.Cancelled = true;
		}
	}

	private void OnReflectHitscan(Entity<ReflectComponent> ent, ref HitScanReflectAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Reflected && TryReflectHitscan(ent, ent.Owner, args.Shooter, args.SourceItem, args.Direction, args.Reflective, out var dir))
		{
			args.Direction = dir.Value;
			args.Reflected = true;
		}
	}

	private bool TryReflectProjectile(Entity<ReflectComponent> reflector, EntityUid user, Entity<ProjectileComponent?> projectile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		ReflectiveComponent reflective = default(ReflectiveComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<ReflectiveComponent>(Entity<ProjectileComponent>.op_Implicit(projectile), ref reflective) || (reflector.Comp.Reflects & reflective.Reflective) == 0 || !_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(reflector.Owner)) || !RandomExtensions.Prob(_random, reflector.Comp.ReflectProb) || !((EntitySystem)this).TryComp<PhysicsComponent>(Entity<ProjectileComponent>.op_Implicit(projectile), ref physics))
		{
			return false;
		}
		Angle val = _random.NextAngle(Angle.op_Implicit(Angle.op_Implicit(-reflector.Comp.Spread) / 2.0), Angle.op_Implicit(Angle.op_Implicit(reflector.Comp.Spread) / 2.0));
		Angle rotation = ((Angle)(ref val)).Opposite();
		Vector2 existingVelocity = _physics.GetMapLinearVelocity(Entity<ProjectileComponent>.op_Implicit(projectile), physics, (TransformComponent)null);
		Vector2 relativeVelocity = existingVelocity - _physics.GetMapLinearVelocity(user, (PhysicsComponent)null, (TransformComponent)null);
		Vector2 difference = ((Angle)(ref rotation)).RotateVec(ref relativeVelocity) - existingVelocity;
		_physics.SetLinearVelocity(Entity<ProjectileComponent>.op_Implicit(projectile), physics.LinearVelocity + difference, true, true, (FixturesComponent)null, physics);
		Angle locRot = ((EntitySystem)this).Transform(Entity<ProjectileComponent>.op_Implicit(projectile)).LocalRotation;
		Vector2 vector = ((Angle)(ref locRot)).ToVec();
		Vector2 newRot = ((Angle)(ref rotation)).RotateVec(ref vector);
		_transform.SetLocalRotation(Entity<ProjectileComponent>.op_Implicit(projectile), DirectionExtensions.ToAngle(newRot), (TransformComponent)null);
		PlayAudioAndPopup(reflector.Comp, user);
		if (((EntitySystem)this).Resolve<ProjectileComponent>(Entity<ProjectileComponent>.op_Implicit(projectile), ref projectile.Comp, false))
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(26, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral(" reflected ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ProjectileComponent>.op_Implicit(projectile), (MetaDataComponent)null), "ToPrettyString(projectile)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(projectile.Comp.Weapon, (MetaDataComponent)null), "ToPrettyString(projectile.Comp.Weapon)");
			handler.AppendLiteral(" shot by ");
			handler.AppendFormatted(projectile.Comp.Shooter, "projectile.Comp.Shooter");
			adminLogger.Add(LogType.BulletHit, LogImpact.Medium, ref handler);
			projectile.Comp.Shooter = user;
			projectile.Comp.Weapon = user;
			((EntitySystem)this).Dirty(Entity<ProjectileComponent>.op_Implicit(projectile), (IComponent)(object)projectile.Comp, (MetaDataComponent)null);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(11, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler2.AppendLiteral(" reflected ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ProjectileComponent>.op_Implicit(projectile), (MetaDataComponent)null), "ToPrettyString(projectile)");
			adminLogger2.Add(LogType.BulletHit, LogImpact.Medium, ref handler2);
		}
		return true;
	}

	private bool TryReflectHitscan(Entity<ReflectComponent> reflector, EntityUid user, EntityUid? shooter, EntityUid shotSource, Vector2 direction, ReflectType hitscanReflectType, [NotNullWhen(true)] out Vector2? newDirection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if ((reflector.Comp.Reflects & hitscanReflectType) == 0 || !_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(reflector.Owner)) || !RandomExtensions.Prob(_random, reflector.Comp.ReflectProb))
		{
			newDirection = null;
			return false;
		}
		PlayAudioAndPopup(reflector.Comp, user);
		Angle spread = _random.NextAngle(Angle.op_Implicit(Angle.op_Implicit(-reflector.Comp.Spread) / 2.0), Angle.op_Implicit(Angle.op_Implicit(reflector.Comp.Spread) / 2.0));
		newDirection = -((Angle)(ref spread)).RotateVec(ref direction);
		if (shooter.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(33, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral(" reflected hitscan from ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(shotSource)), "ToPrettyString(shotSource)");
			handler.AppendLiteral(" shot by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(shooter.Value)), "ToPrettyString(shooter.Value)");
			adminLogger.Add(LogType.HitScanHit, LogImpact.Medium, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(24, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler2.AppendLiteral(" reflected hitscan from ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(shotSource)), "ToPrettyString(shotSource)");
			adminLogger2.Add(LogType.HitScanHit, LogImpact.Medium, ref handler2);
		}
		return true;
	}

	private void PlayAudioAndPopup(ReflectComponent reflect, EntityUid user)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("reflect-shot"), user);
			_audio.PlayPvs(reflect.SoundOnReflect, user, (AudioParams?)null);
		}
	}

	private void OnReflectEquipped(Entity<ReflectComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InRightPlace = (ent.Comp.SlotFlags & args.SlotFlags) == args.SlotFlags;
		((EntitySystem)this).Dirty<ReflectComponent>(ent, (MetaDataComponent)null);
	}

	private void OnReflectUnequipped(Entity<ReflectComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InRightPlace = false;
		((EntitySystem)this).Dirty<ReflectComponent>(ent, (MetaDataComponent)null);
	}

	private void OnReflectHandEquipped(Entity<ReflectComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InRightPlace = ent.Comp.ReflectingInHands;
		((EntitySystem)this).Dirty<ReflectComponent>(ent, (MetaDataComponent)null);
	}

	private void OnReflectHandUnequipped(Entity<ReflectComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InRightPlace = false;
		((EntitySystem)this).Dirty<ReflectComponent>(ent, (MetaDataComponent)null);
	}

	private void OnExamine(Entity<ReflectComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float value = MathF.Round(ent.Comp.ReflectProb * 100f, 1);
		if (_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) && value != 0f && ent.Comp.Reflects != ReflectType.None)
		{
			string[] compTypes = ent.Comp.Reflects.ToString().Split(", ");
			List<string> typeList = new List<string>(compTypes.Length);
			for (int i = 0; i < compTypes.Length; i++)
			{
				string type = base.Loc.GetString(("reflect-component-" + compTypes[i]).ToLower());
				typeList.Add(type);
			}
			string msg = ContentLocalizationManager.FormatList(typeList);
			args.PushMarkup(base.Loc.GetString("reflect-component-examine", (ValueTuple<string, object>)("value", value), (ValueTuple<string, object>)("type", msg)));
		}
	}
}
