using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._RMC14.Movement;
using Content.Client._RMC14.Weapons.Melee;
using Content.Client.Animations;
using Content.Client.Gameplay;
using Content.Client.Weapons.Melee.Components;
using Content.Shared._RMC14.Input;
using Content.Shared.ActionBlocker;
using Content.Shared.Effects;
using Content.Shared.Physics;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client.Weapons.Melee;

public sealed class MeleeWeaponSystem : SharedMeleeWeaponSystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private InputSystem _inputSystem;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private MapSystem _map;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<TransformComponent> _xformQuery;

	private const string MeleeLungeKey = "melee-lunge";

	[Dependency]
	private RMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private RMCMeleeWeaponSystem _rmcMeleeWeapon;

	private const string FadeAnimationKey = "melee-fade";

	private const string SlashAnimationKey = "melee-slash";

	private const string ThrustAnimationKey = "melee-thrust";

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		((EntitySystem)this).SubscribeNetworkEvent<MeleeLungeEvent>((EntityEventHandler<MeleeLungeEvent>)OnMeleeLunge, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesOutsidePrediction = true;
	}

	public override void FrameUpdate(float frameTime)
	{
		((EntitySystem)this).FrameUpdate(frameTime);
		UpdateEffects();
	}

	public override void Update(float frameTime)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Invalid comparison between Unknown and I4
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Invalid comparison between Unknown and I4
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Invalid comparison between Unknown and I4
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Invalid comparison between Unknown and I4
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Invalid comparison between Unknown and I4
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Invalid comparison between Unknown and I4
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!Timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid value = localEntity.Value;
		if (!TryGetWeapon(value, out EntityUid weaponUid, out MeleeWeaponComponent melee))
		{
			return;
		}
		if (CombatMode.IsInCombatMode(value))
		{
			ActionBlockerSystem blocker = Blocker;
			Entity<MeleeWeaponComponent>? weapon = Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, melee));
			if (blocker.CanAttack(value, null, weapon))
			{
				BoundKeyState state = _inputSystem.CmdStates.GetState(EngineKeyFunctions.Use);
				BoundKeyState state2 = _inputSystem.CmdStates.GetState(EngineKeyFunctions.UseSecondary);
				BoundKeyState state3 = _inputSystem.CmdStates.GetState(CMKeyFunctions.CMXenoWideSwing);
				if ((melee.AutoAttack || ((int)state != 1 && (int)state2 != 1 && (int)state3 != 1)) && melee.Attacking)
				{
					((EntitySystem)this).RaisePredictiveEvent<StopAttackEvent>(new StopAttackEvent(((EntitySystem)this).GetNetEntity(weaponUid, (MetaDataComponent)null)));
				}
				if (melee.Attacking || melee.NextAttack > Timing.CurTime)
				{
					return;
				}
				MapCoordinates val = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
				if (val.MapId == MapId.Nullspace)
				{
					return;
				}
				EntityUid val2 = default(EntityUid);
				MapGridComponent val3 = default(MapGridComponent);
				EntityCoordinates coordinates = ((!MapManager.TryFindGridAt(val, ref val2, ref val3)) ? TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((SharedMapSystem)_map).GetMap(val.MapId)), val) : TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(val2), val));
				GunComponent gunComponent = default(GunComponent);
				if (((EntitySystem)this).TryComp<GunComponent>(weaponUid, ref gunComponent) && gunComponent.UseKey)
				{
					AltFireMeleeComponent altFireMeleeComponent = default(AltFireMeleeComponent);
					if (((EntitySystem)this).TryComp<AltFireMeleeComponent>(weaponUid, ref altFireMeleeComponent) && (int)state2 == 1)
					{
						switch (altFireMeleeComponent.AttackType)
						{
						case AltFireAttackType.Light:
							ClientLightAttack(value, val, coordinates, weaponUid, melee);
							break;
						case AltFireAttackType.Heavy:
							ClientHeavyAttack(value, coordinates, weaponUid, melee);
							break;
						case AltFireAttackType.Disarm:
							ClientDisarm(value, val, coordinates, melee);
							break;
						}
					}
				}
				else if ((int)state2 == 1)
				{
					if (melee.AltDisarm && weaponUid == value)
					{
						ClientDisarm(value, val, coordinates, melee);
					}
					else
					{
						ClientHeavyAttack(value, coordinates, weaponUid, melee);
					}
				}
				else
				{
					if ((int)state == 1)
					{
						ClientLightAttack(value, val, coordinates, weaponUid, melee);
					}
					if ((int)state3 == 1)
					{
						ClientHeavyAttack(value, coordinates, weaponUid, melee);
					}
				}
				return;
			}
		}
		melee.Attacking = false;
	}

	protected override bool InRange(EntityUid user, EntityUid target, float range, ICommonSession? session)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent obj = ((EntitySystem)this).Transform(target);
		EntityCoordinates coordinates = obj.Coordinates;
		Angle localRotation = obj.LocalRotation;
		return Interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), coordinates, localRotation, range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: false, overlapCheck: false);
	}

	protected override void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_color.RaiseEffect(Color.Red, targets, Filter.Local());
	}

	public void ClientHeavyAttack(EntityUid user, EntityCoordinates coordinates, EntityUid meleeUid, MeleeWeaponComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = default(TransformComponent);
		if (_xformQuery.TryGetComponent(user, ref val) && Timing.IsFirstTimePredicted)
		{
			MapCoordinates val2 = TransformSystem.ToMapCoordinates(coordinates, true);
			if (!(val2.MapId != val.MapID))
			{
				Vector2 worldPosition = TransformSystem.GetWorldPosition(val);
				Vector2 vector = val2.Position - worldPosition;
				float range = MathF.Min(component.Range, vector.Length());
				List<NetEntity> netEntityList = ((EntitySystem)this).GetNetEntityList(ArcRayCast(worldPosition, DirectionExtensions.ToWorldAngle(vector), component.Angle, range, val.MapID, user).ToList());
				_rmcLagCompensation.SendLastRealTick();
				((EntitySystem)this).RaisePredictiveEvent<HeavyAttackEvent>(new HeavyAttackEvent(((EntitySystem)this).GetNetEntity(meleeUid, (MetaDataComponent)null), netEntityList.GetRange(0, Math.Min(MaxTargets, netEntityList.Count)), ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null)));
			}
		}
	}

	private void ClientDisarm(EntityUid attacker, MapCoordinates mousePos, EntityCoordinates coordinates, MeleeWeaponComponent meleeComponent)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = null;
		if (_stateManager.CurrentState is GameplayStateBase gameplayStateBase)
		{
			val = gameplayStateBase.GetClickedEntity(mousePos);
		}
		MapCoordinates mapCoordinates = TransformSystem.GetMapCoordinates(attacker, (TransformComponent)null);
		if (!(mousePos.MapId != mapCoordinates.MapId) && !((mapCoordinates.Position - mousePos.Position).Length() > meleeComponent.Range))
		{
			_rmcLagCompensation.SendLastRealTick();
			((EntitySystem)this).RaisePredictiveEvent<DisarmAttackEvent>(new DisarmAttackEvent(((EntitySystem)this).GetNetEntity(val, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null)));
		}
	}

	private void ClientLightAttack(EntityUid attacker, MapCoordinates mousePos, EntityCoordinates coordinates, EntityUid weaponUid, MeleeWeaponComponent meleeComponent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates mapCoordinates = TransformSystem.GetMapCoordinates(attacker, (TransformComponent)null);
		if (!(mousePos.MapId != mapCoordinates.MapId))
		{
			EntityUid? val = null;
			if (_stateManager.CurrentState is GameplayStateBase gameplayStateBase)
			{
				val = gameplayStateBase.GetClickedEntity(mousePos);
			}
			if (!((mapCoordinates.Position - mousePos.Position).Length() > _rmcMeleeWeapon.GetUserLightAttackRange(attacker, val, meleeComponent)) && !Interaction.CombatModeCanHandInteract(attacker, val))
			{
				_rmcLagCompensation.SendLastRealTick();
				((EntitySystem)this).RaisePredictiveEvent<LightAttackEvent>(new LightAttackEvent(((EntitySystem)this).GetNetEntity(val, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(weaponUid, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null)));
			}
		}
	}

	private void OnMeleeLunge(MeleeLungeEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Entity);
		EntityUid entity2 = ((EntitySystem)this).GetEntity(ev.Weapon);
		if (((EntitySystem)this).Exists(entity) && ((EntitySystem)this).Exists(entity2))
		{
			DoLunge(entity, entity2, ev.Angle, ev.LocalPos, ev.Animation);
		}
	}

	public override void DoLunge(EntityUid user, EntityUid weapon, Angle angle, Vector2 localPos, string? animation, bool predicted = true)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (localPos == Vector2.Zero || !Timing.IsFirstTimePredicted)
		{
			return;
		}
		Animation lungeAnimation = GetLungeAnimation(localPos);
		_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(user), "melee-lunge");
		_animation.Play(user, lungeAnimation, "melee-lunge");
		TransformComponent val = default(TransformComponent);
		if (localPos == Vector2.Zero || animation == null || !_xformQuery.TryGetComponent(user, ref val) || val.MapID == MapId.Nullspace)
		{
			return;
		}
		EntityUid val2 = ((EntitySystem)this).Spawn(animation, val.Coordinates);
		SpriteComponent val3 = default(SpriteComponent);
		WeaponArcVisualsComponent weaponArcVisualsComponent = default(WeaponArcVisualsComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(val2, ref val3) || !((EntitySystem)this).TryComp<WeaponArcVisualsComponent>(val2, ref weaponArcVisualsComponent))
		{
			return;
		}
		Angle spriteRotation = Angle.Zero;
		MeleeWeaponComponent meleeWeaponComponent = default(MeleeWeaponComponent);
		if (weaponArcVisualsComponent.Animation != WeaponArcAnimation.None && ((EntitySystem)this).TryComp<MeleeWeaponComponent>(weapon, ref meleeWeaponComponent))
		{
			SpriteComponent item = default(SpriteComponent);
			if (user != weapon && ((EntitySystem)this).TryComp<SpriteComponent>(weapon, ref item))
			{
				_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((weapon, item)), Entity<SpriteComponent>.op_Implicit((val2, val3)));
			}
			spriteRotation = meleeWeaponComponent.WideAnimationRotation;
			if (meleeWeaponComponent.SwingLeft)
			{
				angle = Angle.op_Implicit(Angle.op_Implicit(angle) * -1.0);
			}
		}
		_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((val2, val3)), DirectionExtensions.ToWorldAngle(localPos));
		float distance = Math.Clamp(localPos.Length() / 2f, 0.2f, 1f);
		TransformComponent component = _xformQuery.GetComponent(val2);
		switch (weaponArcVisualsComponent.Animation)
		{
		case WeaponArcAnimation.Slash:
			((EntitySystem)this).EnsureComp<TrackUserComponent>(val2).User = user;
			_animation.Play(val2, GetSlashAnimation(val3, angle, spriteRotation), "melee-slash");
			if (weaponArcVisualsComponent.Fadeout)
			{
				_animation.Play(val2, GetFadeAnimation(val3, 0.065f, 0.114999995f), "melee-fade");
			}
			break;
		case WeaponArcAnimation.Thrust:
			((EntitySystem)this).EnsureComp<TrackUserComponent>(val2).User = user;
			_animation.Play(val2, GetThrustAnimation(Entity<SpriteComponent>.op_Implicit((val2, val3)), distance, spriteRotation), "melee-thrust");
			if (weaponArcVisualsComponent.Fadeout)
			{
				_animation.Play(val2, GetFadeAnimation(val3, 0.05f, 0.15f), "melee-fade");
			}
			break;
		case WeaponArcAnimation.None:
		{
			ValueTuple<Vector2, Angle> worldPositionRotation = TransformSystem.GetWorldPositionRotation(val);
			Vector2 item2 = worldPositionRotation.Item1;
			Angle item3 = worldPositionRotation.Item2;
			Angle val4 = item3 - val.LocalRotation;
			Vector2 vector = Vector2.Transform(item2 + ((Angle)(ref val4)).RotateVec(ref localPos), TransformSystem.GetInvWorldMatrix(component.ParentUid));
			TransformSystem.SetLocalPositionNoLerp(val2, vector, component);
			if (weaponArcVisualsComponent.Fadeout)
			{
				_animation.Play(val2, GetFadeAnimation(val3, 0f, 0.15f), "melee-fade");
			}
			break;
		}
		}
	}

	private Animation GetSlashAnimation(SpriteComponent sprite, Angle arc, Angle spriteRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_01a0: Expected O, but got Unknown
		Angle val = sprite.Rotation + Angle.op_Implicit(Angle.op_Implicit(arc) / 2.0);
		Angle val2 = sprite.Rotation - Angle.op_Implicit(Angle.op_Implicit(arc) / 2.0);
		Vector2 vector = new Vector2(0f, -1f);
		Vector2 vector2 = ((Angle)(ref val)).RotateVec(ref vector);
		vector = new Vector2(0f, -1f);
		Vector2 vector3 = ((Angle)(ref val2)).RotateVec(ref vector);
		val += spriteRotation;
		val2 += spriteRotation;
		return new Animation
		{
			Length = TimeSpan.FromSeconds(0.11499999463558197),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Rotation",
				KeyFrames = 
				{
					new KeyFrame((object)val, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)val, 0.03f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)val2, 0.065f, (Func<float, float>)null)
				}
			} },
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				KeyFrames = 
				{
					new KeyFrame((object)vector2, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector2, 0.03f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector3, 0.065f, (Func<float, float>)null)
				}
			} }
		};
	}

	private Animation GetThrustAnimation(Entity<SpriteComponent> sprite, float distance, Angle spriteRotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0108: Expected O, but got Unknown
		Angle rotation = sprite.Comp.Rotation;
		Vector2 vector = new Vector2(0f, (0f - distance) / 5f);
		Vector2 vector2 = ((Angle)(ref rotation)).RotateVec(ref vector);
		rotation = sprite.Comp.Rotation;
		vector = new Vector2(0f, 0f - distance);
		Vector2 vector3 = ((Angle)(ref rotation)).RotateVec(ref vector);
		_sprite.SetRotation(sprite.AsNullable(), sprite.Comp.Rotation + spriteRotation);
		return new Animation
		{
			Length = TimeSpan.FromSeconds(0.15000000596046448),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				KeyFrames = 
				{
					new KeyFrame((object)vector2, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector3, 0.05f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector3, 0.15f, (Func<float, float>)null)
				}
			} }
		};
	}

	private Animation GetFadeAnimation(SpriteComponent sprite, float start, float end)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0085: Expected O, but got Unknown
		Animation val = new Animation
		{
			Length = TimeSpan.FromSeconds(end)
		};
		List<AnimationTrack> animationTracks = val.AnimationTracks;
		AnimationTrackComponentProperty val2 = new AnimationTrackComponentProperty
		{
			ComponentType = typeof(SpriteComponent),
			Property = "Color",
			KeyFrames = 
			{
				new KeyFrame((object)sprite.Color, start, (Func<float, float>)null)
			}
		};
		List<KeyFrame> keyFrames = ((AnimationTrackProperty)val2).KeyFrames;
		Color color = sprite.Color;
		keyFrames.Add(new KeyFrame((object)((Color)(ref color)).WithAlpha(0f), end, (Func<float, float>)null));
		animationTracks.Add((AnimationTrack)val2);
		return val;
	}

	private Animation GetLungeAnimation(Vector2 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(0.10000000149011612),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)(Vector2Helpers.Normalized(direction) * 0.15f), 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, 0.1f, (Func<float, float>)null)
				}
			} }
		};
	}

	private void UpdateEffects()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<TrackUserComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<TrackUserComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		TrackUserComponent trackUserComponent = default(TrackUserComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref trackUserComponent, ref val3))
		{
			if (trackUserComponent.User.HasValue && !((EntitySystem)this).TerminatingOrDeleted(trackUserComponent.User, (MetaDataComponent)null))
			{
				Vector2 vector = TransformSystem.GetWorldPosition(trackUserComponent.User.Value);
				if (trackUserComponent.Offset != Vector2.Zero)
				{
					Angle worldRotation = TransformSystem.GetWorldRotation(val3);
					vector += ((Angle)(ref worldRotation)).RotateVec(ref trackUserComponent.Offset);
				}
				if (trackUserComponent.OriginOffset.HasValue && trackUserComponent.OriginOffset != Vector2.Zero)
				{
					Angle worldRotation2 = TransformSystem.GetWorldRotation(trackUserComponent.User.Value);
					Vector2 vector2 = vector;
					Vector2 value = trackUserComponent.OriginOffset.Value;
					vector = vector2 + ((Angle)(ref worldRotation2)).RotateVec(ref value);
				}
				TransformSystem.SetWorldPosition(val2, vector);
			}
		}
	}
}
