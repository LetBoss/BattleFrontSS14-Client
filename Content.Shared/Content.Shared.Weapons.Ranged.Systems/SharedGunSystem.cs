using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Loadout;
using Content.Shared._PUBG.Weapons;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Stack;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Camera;
using Content.Shared.Chemistry.Components;
using Content.Shared.CombatMode;
using Content.Shared.Containers;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Examine;
using Content.Shared.Explosion.Components;
using Content.Shared.Gravity;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Reflect;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Weapons.Ranged.Systems;

public abstract class SharedGunSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class BatteryAmmoProviderComponentState : ComponentState
	{
		public int Shots;

		public int MaxShots;

		public float FireCost;
	}

	[Serializable]
	[NetSerializable]
	public sealed class HitscanEvent : EntityEventArgs
	{
		public List<(NetCoordinates coordinates, Angle angle, SpriteSpecifier Sprite, float Distance)> Sprites = new List<(NetCoordinates, Angle, SpriteSpecifier, float)>();
	}

	private sealed class CycleModeEvent : InstantActionEvent, ISerializationGenerated<CycleModeEvent>, ISerializationGenerated
	{
		public SelectiveFire Mode;

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref CycleModeEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InstantActionEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (CycleModeEvent)definitionCast;
			serialization.TryCustomCopy<CycleModeEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref CycleModeEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CycleModeEvent cast = (CycleModeEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CycleModeEvent cast = (CycleModeEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override CycleModeEvent Instantiate()
		{
			return new CycleModeEvent();
		}
	}

	[Serializable]
	[NetSerializable]
	protected sealed class RevolverAmmoProviderComponentState : ComponentState
	{
		public int CurrentIndex;

		public List<NetEntity?> AmmoSlots;

		public bool?[] Chambers;
	}

	public sealed class RevolverSpinEvent : EntityEventArgs
	{
	}

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedRMCStackSystem _rmcStack;

	protected const string ChamberSlot = "gun_chamber";

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected IMapManager MapManager;

	[Dependency]
	protected SharedMapSystem MapSystem;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	protected IPrototypeManager ProtoManager;

	[Dependency]
	protected IRobustRandom Random;

	[Dependency]
	protected ISharedAdminLogManager Logs;

	[Dependency]
	protected DamageableSystem Damageable;

	[Dependency]
	protected ExamineSystemShared Examine;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private RechargeBasicEntityAmmoSystem _recharge;

	[Dependency]
	protected SharedActionsSystem Actions;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	protected SharedContainerSystem Containers;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	protected SharedPointLightSystem Lights;

	[Dependency]
	protected SharedPopupSystem PopupSystem;

	[Dependency]
	protected SharedPhysicsSystem Physics;

	[Dependency]
	protected SharedProjectileSystem Projectiles;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	protected TagSystem TagSystem;

	[Dependency]
	protected ThrowingSystem ThrowingSystem;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private SharedStaminaSystem _stamina;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private SharedCameraRecoilSystem _recoil;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetConfigurationManager _netConfig;

	[Dependency]
	private ISharedPlayerManager _playerManager;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private AttachableHolderSystem _attachableHolder;

	[Dependency]
	private SharedRMCFlamerSystem _flamer;

	[Dependency]
	private VehicleSystem _rmcVehicle;

	[Dependency]
	private VehicleWeaponsSystem _rmcVehicleWeapons;

	[Dependency]
	private RMCSharedWeaponControllerSystem _rmcSharedWeaponController;

	[Dependency]
	private PubgWeaponModulesSystem _pubgWeaponModules;

	private const float InteractNextFire = 0.3f;

	private const double SafetyNextFire = 0.5;

	private const float EjectOffset = 0.4f;

	protected const string AmmoExamineColor = "yellow";

	protected const string FireRateExamineColor = "yellow";

	public const string ModeExamineColor = "cyan";

	public const float GunClumsyChance = 0.5f;

	private const float DamagePitchVariation = 0.05f;

	public const string MagazineSlot = "gun_magazine";

	protected const string RevolverContainer = "revolver-ammo";

	public bool GunPrediction { get; private set; }

	public void SetEnabled(EntityUid uid, AutoShootGunComponent component, bool status)
	{
		component.Enabled = status;
	}

	protected virtual void InitializeBallistic()
	{
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentInit>((ComponentEventHandler<BallisticAmmoProviderComponent, ComponentInit>)OnBallisticInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, MapInitEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, MapInitEvent>)OnBallisticMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, TakeAmmoEvent>)OnBallisticTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<BallisticAmmoProviderComponent, GetAmmoCountEvent>)OnBallisticAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, ExaminedEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, ExaminedEvent>)OnBallisticExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>)OnBallisticVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, InteractUsingEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, InteractUsingEvent>)OnBallisticInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, AfterInteractEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, AfterInteractEvent>)OnBallisticAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, AmmoFillDoAfterEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, AmmoFillDoAfterEvent>)OnBallisticAmmoFillDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, DelayedAmmoInsertDoAfterEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, DelayedAmmoInsertDoAfterEvent>)OnBallisticDelayedAmmoInsertDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, DelayedCycleDoAfterEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, DelayedCycleDoAfterEvent>)OnBallisticDelayedCycleDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, UseInHandEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, UseInHandEvent>)OnBallisticUse, (Type[])null, (Type[])null);
	}

	private void OnBallisticUse(EntityUid uid, BallisticAmmoProviderComponent component, UseInHandEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && component.Cycleable)
		{
			if (component.CycleDelay > 0.0)
			{
				BallisticCycleDelayCheck(uid, component, args.User);
			}
			else
			{
				ManualCycle(uid, component, TransformSystem.GetMapCoordinates(uid, (TransformComponent)null), args.User);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void BallisticCycleDelayCheck(EntityUid uid, BallisticAmmoProviderComponent component, EntityUid user)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (component.CycleDelay > 0.0)
		{
			Popup(base.Loc.GetString("gun-ballistic-cycle-delayed", (ValueTuple<string, object>)("entity", uid)), uid, user);
			TimeSpan cycleDelayConverted = TimeSpan.FromSeconds(component.CycleDelay);
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, cycleDelayConverted, new DelayedCycleDoAfterEvent(), used: uid, target: uid, eventTarget: uid)
			{
				BreakOnMove = true,
				BreakOnDamage = false,
				NeedHand = true
			});
		}
		else
		{
			ManualCycle(uid, component, TransformSystem.GetMapCoordinates(uid, (TransformComponent)null), user);
		}
	}

	private void OnBallisticInteractUsing(EntityUid uid, BallisticAmmoProviderComponent component, InteractUsingEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && TryAmmoInsert(uid, component, args.Used, args.User, args.Target, component.InsertDelay))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public bool TryAmmoInsert(EntityUid uid, BallisticAmmoProviderComponent component, EntityUid ammo, EntityUid loader, EntityUid weapon, double insertDelay)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, ammo))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<ActiveTimerTriggerComponent>(ammo))
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-primed", (ValueTuple<string, object>)("ammoEntity", ammo)), uid, loader);
			return false;
		}
		if (GetBallisticShots(component) >= component.Capacity)
		{
			return false;
		}
		ExpendableLightComponent light = default(ExpendableLightComponent);
		if (((EntitySystem)this).TryComp<ExpendableLightComponent>(ammo, ref light) && light.CurrentState != ExpendableLightState.BrandNew)
		{
			return false;
		}
		if (insertDelay > 0.0)
		{
			TimeSpan insertDelayConverted = TimeSpan.FromSeconds(insertDelay);
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, loader, insertDelayConverted, new DelayedAmmoInsertDoAfterEvent(), used: ammo, target: weapon, eventTarget: uid)
			{
				BreakOnMove = true,
				BreakOnDamage = false,
				NeedHand = true
			});
		}
		else
		{
			ManualLoad(uid, component, ammo, loader);
		}
		return true;
	}

	private void OnBallisticAfterInteract(EntityUid uid, BallisticAmmoProviderComponent component, AfterInteractEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && component.MayTransfer && Timing.IsFirstTimePredicted && args.Target.HasValue)
		{
			EntityUid used = args.Used;
			EntityUid? target = args.Target;
			BallisticAmmoProviderComponent targetComponent = default(BallisticAmmoProviderComponent);
			BaseContainer container = default(BaseContainer);
			if ((!target.HasValue || !(used == target.GetValueOrDefault())) && !((EntitySystem)this).Deleted(args.Target) && ((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(args.Target, ref targetComponent) && targetComponent.Whitelist != null && (!Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(args.Target.Value, null)), ref container) || !(container.Owner != args.User) || !((EntitySystem)this).HasComp<StorageComponent>(container.Owner)))
			{
				((HandledEntityEventArgs)args).Handled = true;
				_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, component.FillDelay, new AmmoFillDoAfterEvent(), used: uid, target: args.Target, eventTarget: uid)
				{
					BreakOnMove = true,
					BreakOnDamage = false,
					NeedHand = true
				});
			}
		}
	}

	private void OnBallisticAmmoFillDoAfter(EntityUid uid, BallisticAmmoProviderComponent component, AmmoFillDoAfterEvent args)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		BallisticAmmoProviderComponent target = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).Deleted(args.Target) || !((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(args.Target, ref target) || target.Whitelist == null || args.Cancelled)
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-cancelled", (ValueTuple<string, object>)("entity", uid)), uid, args.User);
			return;
		}
		if (target.Entities.Count + target.UnspawnedCount == target.Capacity)
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-target-full", (ValueTuple<string, object>)("entity", args.Target)), args.Target, args.User);
			return;
		}
		if (component.Entities.Count + component.UnspawnedCount == 0)
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-empty", (ValueTuple<string, object>)("entity", uid)), uid, args.User);
			return;
		}
		List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
		TakeAmmoEvent evTakeAmmo = new TakeAmmoEvent(Math.Clamp(target.Capacity - target.Count, 0, 20), ammo, ((EntitySystem)this).Transform(uid).Coordinates, args.User);
		((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(uid, evTakeAmmo, false);
		foreach (var item in ammo)
		{
			EntityUid? ent = item.Item1;
			if (ent.HasValue)
			{
				if (_whitelistSystem.IsWhitelistFail(target.Whitelist, ent.Value))
				{
					Popup(base.Loc.GetString("gun-ballistic-transfer-invalid", (ValueTuple<string, object>)("ammoEntity", ent.Value), (ValueTuple<string, object>)("targetEntity", args.Target.Value)), uid, args.User);
					SimulateInsertAmmo(ent.Value, uid, ((EntitySystem)this).Transform(uid).Coordinates);
				}
				else
				{
					Audio.PlayPredicted(component.SoundInsert, uid, (EntityUid?)args.User, (AudioParams?)null);
					SimulateInsertAmmo(ent.Value, args.Target.Value, ((EntitySystem)this).Transform(args.Target.Value).Coordinates);
				}
				if (((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null))
				{
					((EntitySystem)this).Del((EntityUid?)ent.Value);
				}
			}
		}
		bool moreSpace = target.Entities.Count + target.UnspawnedCount < target.Capacity;
		bool moreAmmo = component.Entities.Count + component.UnspawnedCount > 0;
		args.Repeat = moreSpace && moreAmmo;
		if (component.DeleteWhenEmpty && component.Entities.Count == 0)
		{
			((EntitySystem)this).Del((EntityUid?)uid);
		}
		void SimulateInsertAmmo(EntityUid used, EntityUid ammoProvider, EntityCoordinates coordinates)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			_interaction.InteractUsing(args.User, used, ammoProvider, coordinates, checkCanInteract: false, checkCanUse: false);
		}
	}

	private void OnBallisticDelayedAmmoInsertDoAfter(EntityUid uid, BallisticAmmoProviderComponent component, DelayedAmmoInsertDoAfterEvent args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent target = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).Deleted(args.Target) || !((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(args.Target, ref target) || target.Whitelist == null || args.Cancelled)
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-cancelled", (ValueTuple<string, object>)("entity", uid)), uid, args.User);
		}
		else if (target.Entities.Count + target.UnspawnedCount == target.Capacity)
		{
			Popup(base.Loc.GetString("gun-ballistic-transfer-target-full", (ValueTuple<string, object>)("entity", args.Target)), args.Target, args.User);
		}
		else if (args.Used.HasValue)
		{
			ManualLoad(uid, component, args.Used.Value, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = false;
		}
	}

	private void ManualLoad(EntityUid uid, BallisticAmmoProviderComponent component, EntityUid used, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(used, ref stack))
		{
			EntityCoordinates coordinates = TransformSystem.GetMoverCoordinates(used);
			EntityUid? split = _rmcStack.Split(Entity<StackComponent>.op_Implicit((used, stack)), 1, coordinates);
			if (split.HasValue)
			{
				used = split.Value;
			}
			SoundSpecifier sound = ((EntitySystem)this).CompOrNull<CartridgeAmmoComponent>(used)?.SoundInsert;
			if (sound != null)
			{
				Audio.PlayPredicted(sound, uid, (EntityUid?)user, (AudioParams?)null);
			}
			UpdateAmmoCount(uid, prediction: true, 1);
			if (_netManager.IsClient)
			{
				return;
			}
		}
		component.Entities.Add(used);
		Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(used), (BaseContainer)(object)component.Container, (TransformComponent)null, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		Audio.PlayPredicted(component.SoundInsert, uid, (EntityUid?)user, (AudioParams?)null);
		UpdateBallisticAppearance(uid, component);
		UpdateAmmoCount(uid);
		((EntitySystem)this).DirtyField<BallisticAmmoProviderComponent>(uid, component, "Entities", (MetaDataComponent)null);
	}

	private void OnBallisticDelayedCycleDoAfter(EntityUid uid, BallisticAmmoProviderComponent component, DelayedCycleDoAfterEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Deleted(uid, (MetaDataComponent)null) || args.Cancelled)
		{
			Popup(base.Loc.GetString("gun-ballistic-cycle-delayed-cancelled", (ValueTuple<string, object>)("entity", uid)), uid, args.User);
			return;
		}
		if (component.Entities.Count + component.UnspawnedCount == 0)
		{
			Popup(base.Loc.GetString("gun-ballistic-cycle-delayed-empty", (ValueTuple<string, object>)("entity", uid)), uid, args.User);
			return;
		}
		ManualCycle(uid, component, TransformSystem.GetMapCoordinates(uid, (TransformComponent)null), args.User);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnBallisticVerb(EntityUid uid, BallisticAmmoProviderComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null && component.Cycleable && component.Cycleable)
		{
			args.Verbs.Add(new Verb
			{
				Text = base.Loc.GetString("gun-ballistic-cycle"),
				Disabled = (GetBallisticShots(component) == 0),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					BallisticCycleDelayCheck(uid, component, args.User);
				}
			});
		}
	}

	private void OnBallisticExamine(EntityUid uid, BallisticAmmoProviderComponent component, ExaminedEvent args)
	{
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("gun-magazine-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("count", GetBallisticShots(component))));
		}
	}

	private void ManualCycle(EntityUid uid, BallisticAmmoProviderComponent component, MapCoordinates coordinates, EntityUid? user = null, GunComponent? gunComp = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (component.Cycleable)
		{
			if (((EntitySystem)this).Resolve<GunComponent>(uid, ref gunComp, false) && gunComp != null && gunComp.FireRateModified > 0f && !((EntitySystem)this).Paused(uid, (MetaDataComponent)null))
			{
				gunComp.NextFire = Timing.CurTime + TimeSpan.FromSeconds(1f / gunComp.FireRateModified);
				((EntitySystem)this).DirtyField<GunComponent>(uid, gunComp, "NextFire", (MetaDataComponent)null);
			}
			Audio.PlayPredicted(component.SoundRack, uid, user, (AudioParams?)null);
			int shots = GetBallisticShots(component);
			Cycle(uid, component, coordinates);
			string text = base.Loc.GetString((shots == 0) ? "gun-ballistic-cycled-empty" : "gun-ballistic-cycled");
			Popup(text, uid, user);
			UpdateBallisticAppearance(uid, component);
			UpdateAmmoCount(uid);
		}
	}

	protected abstract void Cycle(EntityUid uid, BallisticAmmoProviderComponent component, MapCoordinates coordinates);

	private void OnBallisticInit(EntityUid uid, BallisticAmmoProviderComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		component.Container = Containers.EnsureContainer<Container>(uid, "ballistic-ammo", (ContainerManagerComponent)null);
		UpdateBallisticAppearance(uid, component);
	}

	private void OnBallisticMapInit(EntityUid uid, BallisticAmmoProviderComponent component, MapInitEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (component.Proto.HasValue)
		{
			component.UnspawnedCount = Math.Max(0, component.Capacity - ((BaseContainer)component.Container).ContainedEntities.Count);
			UpdateBallisticAppearance(uid, component);
			((EntitySystem)this).DirtyField<BallisticAmmoProviderComponent>(uid, component, "UnspawnedCount", (MetaDataComponent)null);
		}
	}

	protected int GetBallisticShots(BallisticAmmoProviderComponent component)
	{
		return component.Entities.Count + component.UnspawnedCount;
	}

	private void OnBallisticTakeAmmo(EntityUid uid, BallisticAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < args.Shots; i++)
		{
			if (component.Entities.Count > 0)
			{
				List<EntityUid> entities = component.Entities;
				EntityUid entity = entities[entities.Count - 1];
				args.Ammo.Add((entity, EnsureShootable(entity)));
				component.Entities.RemoveAt(component.Entities.Count - 1);
				((EntitySystem)this).DirtyField<BallisticAmmoProviderComponent>(uid, component, "Entities", (MetaDataComponent)null);
				Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(entity), (BaseContainer)(object)component.Container, true, false, (EntityCoordinates?)null, (Angle?)null);
			}
			else if (component.UnspawnedCount > 0)
			{
				component.UnspawnedCount--;
				((EntitySystem)this).DirtyField<BallisticAmmoProviderComponent>(uid, component, "UnspawnedCount", (MetaDataComponent)null);
				EntProtoId? proto = component.Proto;
				EntityUid entity = ((EntitySystem)this).Spawn(proto.HasValue ? EntProtoId.op_Implicit(proto.GetValueOrDefault()) : null, args.Coordinates);
				args.Ammo.Add((entity, EnsureShootable(entity)));
			}
		}
		UpdateBallisticAppearance(uid, component);
	}

	private void OnBallisticAmmoCount(EntityUid uid, BallisticAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		args.Count = GetBallisticShots(component);
		args.Capacity = component.Capacity;
	}

	public void UpdateBallisticAppearance(EntityUid uid, BallisticAmmoProviderComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (Timing.IsFirstTimePredicted && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)GetBallisticShots(component), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)component.Capacity, appearance);
		}
	}

	public void SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent> entity, int count)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.UnspawnedCount != count)
		{
			entity.Comp.UnspawnedCount = count;
			UpdateBallisticAppearance(entity.Owner, entity.Comp);
			UpdateAmmoCount(entity.Owner);
			((EntitySystem)this).Dirty<BallisticAmmoProviderComponent>(entity, (MetaDataComponent)null);
		}
	}

	protected virtual void InitializeBasicEntity()
	{
		((EntitySystem)this).SubscribeLocalEvent<BasicEntityAmmoProviderComponent, MapInitEvent>((ComponentEventHandler<BasicEntityAmmoProviderComponent, MapInitEvent>)OnBasicEntityMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BasicEntityAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<BasicEntityAmmoProviderComponent, TakeAmmoEvent>)OnBasicEntityTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>)OnBasicEntityAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnBasicEntityMapInit(EntityUid uid, BasicEntityAmmoProviderComponent component, MapInitEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		int? count = component.Count;
		if (!count.HasValue)
		{
			component.Count = component.Capacity;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		UpdateBasicEntityAppearance(uid, component);
	}

	private void OnBasicEntityTakeAmmo(EntityUid uid, BasicEntityAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < args.Shots; i++)
		{
			if (component.Count <= 0)
			{
				return;
			}
			if (component.Count.HasValue)
			{
				component.Count--;
			}
			EntityUid ent = ((EntitySystem)this).Spawn(component.Proto, args.Coordinates);
			args.Ammo.Add((ent, EnsureShootable(ent)));
		}
		_recharge.Reset(uid);
		UpdateBasicEntityAppearance(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnBasicEntityAmmoCount(EntityUid uid, BasicEntityAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		args.Capacity = component.Capacity ?? int.MaxValue;
		args.Count = component.Count ?? int.MaxValue;
	}

	private void UpdateBasicEntityAppearance(EntityUid uid, BasicEntityAmmoProviderComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (Timing.IsFirstTimePredicted && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.HasAmmo, (object)(component.Count != 0), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)(component.Count ?? int.MaxValue), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)(component.Capacity ?? int.MaxValue), appearance);
		}
	}

	public bool ChangeBasicEntityAmmoCount(EntityUid uid, int delta, BasicEntityAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BasicEntityAmmoProviderComponent>(uid, ref component, false) || !component.Count.HasValue)
		{
			return false;
		}
		return UpdateBasicEntityAmmoCount(uid, component.Count.Value + delta, component);
	}

	public bool UpdateBasicEntityAmmoCount(EntityUid uid, int count, BasicEntityAmmoProviderComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BasicEntityAmmoProviderComponent>(uid, ref component, false))
		{
			return false;
		}
		if (count > component.Capacity)
		{
			return false;
		}
		component.Count = count;
		UpdateBasicEntityAppearance(uid, component);
		UpdateAmmoCount(uid);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}

	protected virtual void InitializeBattery()
	{
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentGetState>((ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentGetState>)OnBatteryGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentHandleState>((ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentHandleState>)OnBatteryHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>)OnBatteryTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>)OnBatteryAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ExaminedEvent>((ComponentEventHandler<HitscanBatteryAmmoProviderComponent, ExaminedEvent>)OnBatteryExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentGetState>((ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentGetState>)OnBatteryGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>((ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>)OnBatteryHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>)OnBatteryTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>)OnBatteryAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>((ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>)OnBatteryExamine, (Type[])null, (Type[])null);
	}

	private void OnBatteryHandleState(EntityUid uid, BatteryAmmoProviderComponent component, ref ComponentHandleState args)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is BatteryAmmoProviderComponentState state)
		{
			component.Shots = state.Shots;
			component.Capacity = state.MaxShots;
			component.FireCost = state.FireCost;
			UpdateAmmoCount(uid, prediction: false);
		}
	}

	private void OnBatteryGetState(EntityUid uid, BatteryAmmoProviderComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new BatteryAmmoProviderComponentState
		{
			Shots = component.Shots,
			MaxShots = component.Capacity,
			FireCost = component.FireCost
		};
	}

	private void OnBatteryExamine(EntityUid uid, BatteryAmmoProviderComponent component, ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("gun-battery-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("count", component.Shots)));
	}

	private void OnBatteryTakeAmmo(EntityUid uid, BatteryAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		int shots = Math.Min(args.Shots, component.Shots);
		if (shots != 0)
		{
			for (int i = 0; i < shots; i++)
			{
				args.Ammo.Add(GetShootable(component, args.Coordinates));
				component.Shots--;
			}
			TakeCharge(Entity<BatteryAmmoProviderComponent>.op_Implicit((uid, component)));
			UpdateBatteryAppearance(uid, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnBatteryAmmoCount(EntityUid uid, BatteryAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		args.Count = component.Shots;
		args.Capacity = component.Capacity;
	}

	protected virtual void TakeCharge(Entity<BatteryAmmoProviderComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateAmmoCount(Entity<BatteryAmmoProviderComponent>.op_Implicit(entity), prediction: false);
	}

	protected void UpdateBatteryAppearance(EntityUid uid, BatteryAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.HasAmmo, (object)(component.Shots != 0), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)component.Shots, appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)component.Capacity, appearance);
		}
	}

	private (EntityUid? Entity, IShootable) GetShootable(BatteryAmmoProviderComponent component, EntityCoordinates coordinates)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!(component is ProjectileBatteryAmmoProviderComponent proj))
		{
			if (component is HitscanBatteryAmmoProviderComponent hitscan)
			{
				return (Entity: null, ProtoManager.Index<HitscanPrototype>(hitscan.Prototype));
			}
			throw new ArgumentOutOfRangeException();
		}
		EntityUid ent = ((EntitySystem)this).Spawn(proj.Prototype, coordinates);
		return (Entity: ent, EnsureShootable(ent));
	}

	protected virtual void InitializeCartridge()
	{
	}

	protected virtual void InitializeChamberMagazine()
	{
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ComponentStartup>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ComponentStartup>)OnChamberStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>)OnChamberMagazineTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, GetAmmoCountEvent>)OnChamberAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<ActivationVerb>>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<ActivationVerb>>)OnChamberActivationVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<InteractionVerb>>)OnChamberInteractionVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>)OnMagazineVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ActivateInWorldEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ActivateInWorldEvent>)OnChamberActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, UseInHandEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, UseInHandEvent>)OnChamberUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>)OnMagazineSlotChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>)OnMagazineSlotChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ExaminedEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ExaminedEvent>)OnChamberMagazineExamine, (Type[])null, (Type[])null);
	}

	private void OnChamberStartup(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ComponentStartup args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (component.BoltClosed.HasValue)
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.BoltClosed, (object)component.BoltClosed.Value, (AppearanceComponent)null);
		}
	}

	private void OnChamberActivate(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ActivateInWorldEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleBolt(uid, component, args.User);
		}
	}

	private void OnChamberUse(EntityUid uid, ChamberMagazineAmmoProviderComponent component, UseInHandEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (component.CanRack)
			{
				UseChambered(uid, component, args.User);
			}
			else
			{
				ToggleBolt(uid, component, args.User);
			}
		}
	}

	private void OnChamberActivationVerb(EntityUid uid, ChamberMagazineAmmoProviderComponent component, GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.CanComplexInteract && args.Hands != null && component.BoltClosed.HasValue && component.CanRack)
		{
			args.Verbs.Add(new ActivationVerb
			{
				Text = base.Loc.GetString("gun-chamber-rack"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					UseChambered(uid, component, args.User);
				}
			});
		}
	}

	private void UseChambered(EntityUid uid, ChamberMagazineAmmoProviderComponent component, EntityUid? user = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (component.BoltClosed == false)
		{
			ToggleBolt(uid, component, user);
			return;
		}
		if (TryTakeChamberEntity(uid, out var chamberEnt))
		{
			if (_netManager.IsServer)
			{
				EjectCartridge(chamberEnt.Value);
			}
			else
			{
				TransformSystem.DetachEntity(chamberEnt.Value, ((EntitySystem)this).Transform(chamberEnt.Value));
			}
		}
		if (!CycleCartridge(uid, component, user))
		{
			UpdateAmmoCount(uid);
		}
		if (component.BoltClosed != false)
		{
			Audio.PlayPredicted(component.RackSound, uid, user, (AudioParams?)null);
		}
	}

	private void OnChamberInteractionVerb(EntityUid uid, ChamberMagazineAmmoProviderComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.CanComplexInteract && args.Hands != null && component.BoltClosed.HasValue)
		{
			args.Verbs.Add(new InteractionVerb
			{
				Text = (component.BoltClosed.Value ? base.Loc.GetString("gun-chamber-bolt-open") : base.Loc.GetString("gun-chamber-bolt-close")),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					ToggleBolt(uid, component, args.User);
				}
			});
		}
	}

	public void SetBoltClosed(EntityUid uid, ChamberMagazineAmmoProviderComponent component, bool value, EntityUid? user = null, AppearanceComponent? appearance = null, ItemSlotsComponent? slots = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		if (!component.BoltClosed.HasValue || value == component.BoltClosed)
		{
			return;
		}
		((EntitySystem)this).Resolve<AppearanceComponent, ItemSlotsComponent>(uid, ref appearance, ref slots, false);
		Appearance.SetData(uid, (Enum)AmmoVisuals.BoltClosed, (object)value, appearance);
		if (value)
		{
			CycleCartridge(uid, component, user, appearance);
			if (user.HasValue)
			{
				PopupSystem.PopupClient(base.Loc.GetString("gun-chamber-bolt-closed"), uid, user.Value);
			}
			if (slots != null)
			{
				_slots.SetLock(uid, "gun_chamber", locked: true, slots);
			}
			Audio.PlayPredicted(component.BoltClosedSound, uid, user, (AudioParams?)null);
		}
		else
		{
			if (TryTakeChamberEntity(uid, out var chambered))
			{
				if (_netManager.IsServer)
				{
					EjectCartridge(chambered.Value);
				}
				else
				{
					TransformSystem.DetachEntity(chambered.Value, ((EntitySystem)this).Transform(chambered.Value));
				}
				UpdateAmmoCount(uid);
			}
			if (user.HasValue)
			{
				PopupSystem.PopupClient(base.Loc.GetString("gun-chamber-bolt-opened"), uid, user.Value);
			}
			if (slots != null)
			{
				_slots.SetLock(uid, "gun_chamber", locked: false, slots);
			}
			Audio.PlayPredicted(component.BoltOpenedSound, uid, user, (AudioParams?)null);
		}
		component.BoltClosed = value;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private bool CycleCartridge(EntityUid uid, ChamberMagazineAmmoProviderComponent component, EntityUid? user = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? magEnt = GetMagazineEntity(uid);
		EntityUid? chambered = GetChamberEntity(uid);
		bool result = false;
		if (magEnt.HasValue && !chambered.HasValue)
		{
			TakeAmmoEvent relayedArgs = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), ((EntitySystem)this).Transform(uid).Coordinates, user);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(magEnt.Value, relayedArgs, false);
			if (relayedArgs.Ammo.Count > 0)
			{
				EntityUid? newChamberEnt = relayedArgs.Ammo[0].Entity;
				TryInsertChamber(uid, newChamberEnt.Value);
				GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
				((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magEnt.Value, ref ammoEv, false);
				FinaliseMagazineTakeAmmo(uid, component, ammoEv.Count, ammoEv.Capacity, user, appearance);
				UpdateAmmoCount(uid);
				if (_netManager.IsClient)
				{
					foreach (var item in relayedArgs.Ammo)
					{
						EntityUid? ent = item.Entity;
						if (((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null))
						{
							((EntitySystem)this).Del((EntityUid?)ent.Value);
						}
					}
				}
			}
			else
			{
				UpdateAmmoCount(uid);
			}
			result = true;
		}
		return result;
	}

	public void ToggleBolt(EntityUid uid, ChamberMagazineAmmoProviderComponent component, EntityUid? user = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (component.BoltClosed.HasValue)
		{
			SetBoltClosed(uid, component, !component.BoltClosed.Value, user);
		}
	}

	private void OnChamberMagazineExamine(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ExaminedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange)
		{
			return;
		}
		int count = GetChamberMagazineCountCapacity(uid, component).Item1;
		using (args.PushGroup("ChamberMagazineAmmoProviderComponent"))
		{
			if (component.BoltClosed.HasValue)
			{
				string boltState = ((component.BoltClosed != true) ? base.Loc.GetString("gun-chamber-bolt-closed-state") : base.Loc.GetString("gun-chamber-bolt-open-state"));
				args.PushMarkup(base.Loc.GetString("gun-chamber-bolt", (ValueTuple<string, object>)("bolt", boltState), (ValueTuple<string, object>)("color", component.BoltClosed.Value ? Color.FromHex((ReadOnlySpan<char>)"#94e1f2", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#f29d94", (Color?)null))));
			}
			args.PushMarkup(base.Loc.GetString("gun-magazine-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("count", count)));
		}
	}

	private bool TryTakeChamberEntity(EntityUid uid, [NotNullWhen(true)] out EntityUid? entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainer(uid, "gun_chamber", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null)
			{
				entity = slot.ContainedEntity;
				if (!entity.HasValue)
				{
					return false;
				}
				Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(entity.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null);
				return true;
			}
		}
		entity = null;
		return false;
	}

	protected EntityUid? GetChamberEntity(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainer(uid, "gun_chamber", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null)
			{
				return slot.ContainedEntity;
			}
		}
		return null;
	}

	protected (int, int) GetChamberMagazineCountCapacity(EntityUid uid, ChamberMagazineAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		bool num = GetChamberEntity(uid).HasValue;
		var (magCount, magCapacity) = GetMagazineCountCapacity(uid, component);
		return ((num ? 1 : 0) + magCount, magCapacity);
	}

	private bool TryInsertChamber(EntityUid uid, EntityUid ammo)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainer(uid, "gun_chamber", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null)
			{
				return Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ammo), (BaseContainer)(object)slot, (TransformComponent)null, false);
			}
		}
		return false;
	}

	private void OnChamberAmmoCount(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		OnMagazineAmmoCount(uid, component, ref args);
		args.Capacity++;
		if (GetChamberEntity(uid).HasValue)
		{
			args.Count++;
		}
	}

	private void OnChamberMagazineTakeAmmo(EntityUid uid, ChamberMagazineAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (component.BoltClosed == false)
		{
			args.Reason = base.Loc.GetString("gun-chamber-bolt-ammo");
			return;
		}
		AppearanceComponent appearance = default(AppearanceComponent);
		((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance);
		BaseContainer container = default(BaseContainer);
		if (component.AutoCycle)
		{
			if (!TryTakeChamberEntity(uid, out var chamberEnt))
			{
				return;
			}
			args.Ammo.Add((chamberEnt.Value, EnsureShootable(chamberEnt.Value)));
			EntityUid? magEnt = GetMagazineEntity(uid);
			if (magEnt.HasValue)
			{
				TakeAmmoEvent relayedArgs = new TakeAmmoEvent(args.Shots, new List<(EntityUid?, IShootable)>(), args.Coordinates, args.User);
				((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(magEnt.Value, relayedArgs, false);
				if (relayedArgs.Ammo.Count > 0)
				{
					List<(EntityUid? Entity, IShootable Shootable)> ammo = relayedArgs.Ammo;
					EntityUid? newChamberEnt = ammo[ammo.Count - 1].Entity;
					TryInsertChamber(uid, newChamberEnt.Value);
				}
				for (int i = 0; i < relayedArgs.Ammo.Count - 1; i++)
				{
					args.Ammo.Add(relayedArgs.Ammo[i]);
				}
				if (relayedArgs.Ammo.Count == 0)
				{
					SetBoltClosed(uid, component, value: false, args.User, appearance);
				}
				GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
				((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magEnt.Value, ref ammoEv, false);
				FinaliseMagazineTakeAmmo(uid, component, ammoEv.Count, ammoEv.Capacity, args.User, appearance);
			}
			else
			{
				Appearance.SetData(uid, (Enum)AmmoVisuals.MagLoaded, (object)false, appearance);
			}
		}
		else if (Containers.TryGetContainer(uid, "gun_chamber", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null && slot.ContainedEntity.HasValue)
			{
				EntityUid? chamberEnt = slot.ContainedEntity;
				args.Ammo.Add((chamberEnt.Value, EnsureShootable(chamberEnt.Value)));
			}
		}
	}

	private void InitializeClothing()
	{
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<ClothingSlotAmmoProviderComponent, TakeAmmoEvent>)OnClothingTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<ClothingSlotAmmoProviderComponent, GetAmmoCountEvent>)OnClothingAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnClothingTakeAmmo(EntityUid uid, ClothingSlotAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		GetConnectedContainerEvent getConnectedContainerEvent = default(GetConnectedContainerEvent);
		((EntitySystem)this).RaiseLocalEvent<GetConnectedContainerEvent>(uid, ref getConnectedContainerEvent, false);
		if (getConnectedContainerEvent.ContainerEntity.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(getConnectedContainerEvent.ContainerEntity.Value, args, false);
		}
	}

	private void OnClothingAmmoCount(EntityUid uid, ClothingSlotAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		GetConnectedContainerEvent getConnectedContainerEvent = default(GetConnectedContainerEvent);
		((EntitySystem)this).RaiseLocalEvent<GetConnectedContainerEvent>(uid, ref getConnectedContainerEvent, false);
		if (getConnectedContainerEvent.ContainerEntity.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(getConnectedContainerEvent.ContainerEntity.Value, ref args, false);
		}
	}

	private void InitializeContainer()
	{
		((EntitySystem)this).SubscribeLocalEvent<ContainerAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<ContainerAmmoProviderComponent, TakeAmmoEvent>)OnContainerTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainerAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<ContainerAmmoProviderComponent, GetAmmoCountEvent>)OnContainerAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnContainerTakeAmmo(EntityUid uid, ContainerAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid valueOrDefault = component.ProviderUid.GetValueOrDefault();
		if (!component.ProviderUid.HasValue)
		{
			valueOrDefault = uid;
			component.ProviderUid = valueOrDefault;
		}
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainer(component.ProviderUid.Value, component.Container, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		for (int i = 0; i < args.Shots; i++)
		{
			if (!container.ContainedEntities.Any())
			{
				break;
			}
			EntityUid ent = container.ContainedEntities[0];
			if (_netManager.IsServer)
			{
				Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent), container, true, false, (EntityCoordinates?)null, (Angle?)null);
			}
			args.Ammo.Add((ent, EnsureShootable(ent)));
		}
	}

	private void OnContainerAmmoCount(EntityUid uid, ContainerAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid valueOrDefault = component.ProviderUid.GetValueOrDefault();
		if (!component.ProviderUid.HasValue)
		{
			valueOrDefault = uid;
			component.ProviderUid = valueOrDefault;
		}
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainer(component.ProviderUid.Value, component.Container, ref container, (ContainerManagerComponent)null))
		{
			args.Capacity = 0;
			args.Count = 0;
		}
		else
		{
			args.Capacity = int.MaxValue;
			args.Count = container.ContainedEntities.Count;
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeAllEvent<RequestStopShootEvent>((EntitySessionEventHandler<RequestStopShootEvent>)OnStopShootRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, MeleeHitEvent>((ComponentEventHandler<GunComponent, MeleeHitEvent>)OnGunMelee, (Type[])null, (Type[])null);
		InitializeBallistic();
		InitializeBattery();
		InitializeCartridge();
		InitializeChamberMagazine();
		InitializeMagazine();
		InitializeRevolver();
		InitializeBasicEntity();
		InitializeClothing();
		InitializeContainer();
		InitializeSolution();
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<GunComponent, GetVerbsEvent<AlternativeVerb>>)OnAltVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, ExaminedEvent>((ComponentEventHandler<GunComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, CycleModeEvent>((ComponentEventHandler<GunComponent, CycleModeEvent>)OnCycleMode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, HandSelectedEvent>((ComponentEventHandler<GunComponent, HandSelectedEvent>)OnGunSelected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, MapInitEvent>((EntityEventRefHandler<GunComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCGunPrediction, (Action<bool>)delegate(bool v)
		{
			GunPrediction = v;
		}, true);
	}

	private void OnMapInit(Entity<GunComponent> gun, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshModifiers(Entity<GunComponent>.op_Implicit((Entity<GunComponent>.op_Implicit(gun), Entity<GunComponent>.op_Implicit(gun))));
	}

	private void OnGunMelee(EntityUid uid, GunComponent component, MeleeHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(uid, ref melee) && component.MeleeCooldownOnShoot && melee.NextAttack > component.NextFire)
		{
			component.NextFire = melee.NextAttack;
			((EntitySystem)this).DirtyField<GunComponent>(uid, component, "NextFire", (MetaDataComponent)null);
		}
	}

	private void OnStopShootRequest(RequestStopShootEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gunUid = ((EntitySystem)this).GetEntity(ev.Gun);
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			GunComponent gun = default(GunComponent);
			if (((EntitySystem)this).TryComp<GunComponent>(gunUid, ref gun) && TryGetGun(user, out EntityUid _, out GunComponent userGun) && userGun == gun)
			{
				StopShooting(gunUid, gun);
			}
		}
	}

	public bool CanShoot(GunComponent component)
	{
		if (component.NextFire > Timing.CurTime)
		{
			return false;
		}
		return true;
	}

	public EntityUid? SwapTarget(Entity<GunComponent> gun, EntityUid? target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target2 = gun.Comp.Target;
		gun.Comp.Target = target;
		return target2;
	}

	public bool TryGetGun(EntityUid entity, out EntityUid gunEntity, [NotNullWhen(true)] out GunComponent? gunComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		VehiclePortGunOperatorComponent portGunOperator = default(VehiclePortGunOperatorComponent);
		EntityUid? gun;
		if (((EntitySystem)this).TryComp<VehiclePortGunOperatorComponent>(entity, ref portGunOperator))
		{
			gun = portGunOperator.Gun;
			if (gun.HasValue)
			{
				EntityUid portGun = gun.GetValueOrDefault();
				VehiclePortGunComponent portGunComp = default(VehiclePortGunComponent);
				if (((EntitySystem)this).TryComp<VehiclePortGunComponent>(portGun, ref portGunComp))
				{
					gun = portGunComp.Operator;
					GunComponent portGunGun = default(GunComponent);
					if (gun.HasValue && gun.GetValueOrDefault() == entity && ((EntitySystem)this).TryComp<GunComponent>(portGun, ref portGunGun))
					{
						gunEntity = portGun;
						gunComp = portGunGun;
						return true;
					}
				}
			}
		}
		VehicleWeaponsOperatorComponent vehicleOperator = default(VehicleWeaponsOperatorComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(entity, ref vehicleOperator))
		{
			gun = vehicleOperator.Vehicle;
			if (gun.HasValue)
			{
				EntityUid vehicle = gun.GetValueOrDefault();
				GunComponent selectedGun = default(GunComponent);
				if (_rmcVehicleWeapons.TryGetSelectedWeaponForOperator(vehicle, entity, out var selected) && ((EntitySystem)this).TryComp<GunComponent>(selected, ref selectedGun))
				{
					gunEntity = selected;
					gunComp = selectedGun;
					return true;
				}
			}
		}
		if (_attachableHolder.TryGetInhandSupercedingGun(entity, out gunEntity, out gunComp))
		{
			return true;
		}
		gunEntity = default(EntityUid);
		gunComp = null;
		gun = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(entity));
		GunComponent gun2 = default(GunComponent);
		if (gun.HasValue)
		{
			EntityUid held = gun.GetValueOrDefault();
			if (((EntitySystem)this).TryComp<GunComponent>(held, ref gun2))
			{
				gunEntity = held;
				gunComp = gun2;
				return true;
			}
		}
		if (((EntitySystem)this).TryComp<GunComponent>(entity, ref gun2))
		{
			gunEntity = entity;
			gunComp = gun2;
			return true;
		}
		if (_rmcSharedWeaponController.TryGetControlledWeapon(entity, out EntityUid? weapon, out gun2))
		{
			gunEntity = weapon.Value;
			gunComp = gun2;
			return true;
		}
		return false;
	}

	public void StopShooting(EntityUid uid, GunComponent gun)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (gun.ShotCounter != 0)
		{
			gun.ShotCounter = 0;
			gun.ShootCoordinates = null;
			gun.Target = null;
			((EntitySystem)this).DirtyField<GunComponent>(uid, gun, "ShotCounter", (MetaDataComponent)null);
		}
	}

	public List<EntityUid>? AttemptShoot(Entity<GunComponent> ent, EntityUid user, EntityCoordinates coordinates)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ShootCoordinates = coordinates;
		List<EntityUid>? result = AttemptShoot(user, Entity<GunComponent>.op_Implicit(ent), Entity<GunComponent>.op_Implicit(ent));
		ent.Comp.ShotCounter = 0;
		((EntitySystem)this).DirtyField<GunComponent>(ent.Owner, ent.Comp, "ShotCounter", (MetaDataComponent)null);
		return result;
	}

	public void AttemptShoot(EntityUid user, EntityUid gunUid, GunComponent gun, EntityCoordinates toCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		gun.ShootCoordinates = toCoordinates;
		AttemptShoot(user, gunUid, gun);
		gun.ShotCounter = 0;
		((EntitySystem)this).DirtyField<GunComponent>(gunUid, gun, "ShotCounter", (MetaDataComponent)null);
	}

	public void AttemptShoot(EntityUid gunUid, GunComponent gun)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = default(EntityCoordinates);
		((EntityCoordinates)(ref coordinates))._002Ector(gunUid, gun.DefaultDirection);
		gun.ShootCoordinates = coordinates;
		AttemptShoot(gunUid, gunUid, gun);
		gun.ShotCounter = 0;
	}

	public List<EntityUid>? AttemptShoot(EntityUid user, EntityUid gunUid, GunComponent gun, List<int>? predictedProjectiles = null, ICommonSession? userSession = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		if (gun.FireRateModified <= 0f || !_actionBlockerSystem.CanAttack(user))
		{
			return null;
		}
		EntityCoordinates? toCoordinates = gun.ShootCoordinates;
		if (!toCoordinates.HasValue)
		{
			return null;
		}
		if (!TransformSystem.IsValid(toCoordinates.Value))
		{
			return null;
		}
		TimeSpan curTime = Timing.CurTime;
		ShotAttemptedEvent prevention = new ShotAttemptedEvent
		{
			User = user,
			Used = Entity<GunComponent>.op_Implicit((gunUid, gun))
		};
		((EntitySystem)this).RaiseLocalEvent<ShotAttemptedEvent>(gunUid, ref prevention, false);
		if (prevention.Cancelled)
		{
			return null;
		}
		((EntitySystem)this).RaiseLocalEvent<ShotAttemptedEvent>(user, ref prevention, false);
		if (prevention.Cancelled)
		{
			return null;
		}
		if (gun.NextFire > curTime)
		{
			return null;
		}
		TimeSpan fireRate = TimeSpan.FromSeconds(1f / gun.FireRateModified);
		if (gun.SelectedMode == SelectiveFire.Burst || gun.BurstActivated)
		{
			fireRate = TimeSpan.FromSeconds(1f / gun.BurstFireRate);
		}
		if (gun.NextFire < curTime - fireRate || (gun.ShotCounter == 0 && gun.NextFire < curTime))
		{
			gun.NextFire = curTime;
		}
		int shots = 0;
		TimeSpan lastFire = gun.NextFire;
		while (gun.NextFire <= curTime)
		{
			gun.NextFire += fireRate;
			shots++;
		}
		((EntitySystem)this).DirtyField<GunComponent>(gunUid, gun, "NextFire", (MetaDataComponent)null);
		if (!gun.BurstActivated)
		{
			switch (gun.SelectedMode)
			{
			case SelectiveFire.SemiAuto:
				shots = Math.Min(shots, 1 - gun.ShotCounter);
				break;
			case SelectiveFire.Burst:
				shots = Math.Min(shots, gun.ShotsPerBurstModified - gun.ShotCounter);
				break;
			default:
				throw new ArgumentOutOfRangeException($"No implemented shooting behavior for {gun.SelectedMode}!");
			case SelectiveFire.FullAuto:
				break;
			}
		}
		else
		{
			shots = Math.Min(shots, gun.ShotsPerBurstModified - gun.ShotCounter);
		}
		EntityUid originEntity = (((EntitySystem)this).HasComp<GunUseGunOriginComponent>(gunUid) ? gunUid : user);
		EntityCoordinates fromCoordinates = ((EntitySystem)this).Transform(originEntity).Coordinates;
		BeforeAttemptShootEvent shotOriginEv = new BeforeAttemptShootEvent(fromCoordinates, gun.ShootOriginOffset);
		((EntitySystem)this).RaiseLocalEvent<BeforeAttemptShootEvent>(user, ref shotOriginEv, false);
		if (shotOriginEv.Handled)
		{
			fromCoordinates = shotOriginEv.Origin;
		}
		AttemptShootEvent attemptEv = new AttemptShootEvent(user, null, fromCoordinates, toCoordinates);
		((EntitySystem)this).RaiseLocalEvent<AttemptShootEvent>(gunUid, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			if (attemptEv.Message != null)
			{
				PopupSystem.PopupClient(attemptEv.Message, gunUid, user);
			}
			gun.BurstActivated = false;
			gun.BurstShotsCount = 0;
			gun.NextFire = (attemptEv.ResetCooldown ? curTime : TimeSpan.FromSeconds(Math.Max(lastFire.TotalSeconds + 0.5, gun.NextFire.TotalSeconds)));
			return null;
		}
		fromCoordinates = attemptEv.FromCoordinates;
		toCoordinates = attemptEv.ToCoordinates;
		if (!toCoordinates.HasValue)
		{
			return null;
		}
		if (!TransformSystem.IsValid(fromCoordinates) || !TransformSystem.IsValid(toCoordinates.Value))
		{
			return null;
		}
		TakeAmmoEvent ev = new TakeAmmoEvent(shots, new List<(EntityUid?, IShootable)>(), fromCoordinates, user);
		if (shots > 0)
		{
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(gunUid, ev, false);
		}
		UpdateAmmoCount(gunUid);
		gun.ShotCounter += shots;
		((EntitySystem)this).DirtyField<GunComponent>(gunUid, gun, "ShotCounter", (MetaDataComponent)null);
		if (ev.Ammo.Count <= 0)
		{
			OnEmptyGunShotEvent emptyGunShotEvent = default(OnEmptyGunShotEvent);
			((EntitySystem)this).RaiseLocalEvent<OnEmptyGunShotEvent>(gunUid, ref emptyGunShotEvent, false);
			gun.BurstActivated = false;
			gun.BurstShotsCount = 0;
			gun.NextFire += TimeSpan.FromSeconds(gun.BurstCooldown);
			if (shots > 0)
			{
				PopupSystem.PopupCursor(ev.Reason ?? base.Loc.GetString("gun-magazine-fired-empty"));
				gun.NextFire = TimeSpan.FromSeconds(Math.Max(lastFire.TotalSeconds + 0.5, gun.NextFire.TotalSeconds));
				Audio.PlayPredicted(gun.SoundEmpty, gunUid, (EntityUid?)user, (AudioParams?)null);
				return null;
			}
			return null;
		}
		if (gun.SelectedMode == SelectiveFire.Burst)
		{
			gun.BurstActivated = true;
		}
		if (gun.BurstActivated)
		{
			gun.BurstShotsCount += shots;
			if (gun.BurstShotsCount >= gun.ShotsPerBurstModified)
			{
				gun.NextFire += TimeSpan.FromSeconds(gun.BurstCooldown);
				gun.BurstActivated = false;
				gun.BurstShotsCount = 0;
			}
		}
		List<EntityUid> projectiles = null;
		bool userImpulse = false;
		if (Timing.IsFirstTimePredicted)
		{
			projectiles = Shoot(gunUid, gun, ev.Ammo, fromCoordinates, toCoordinates.Value, out userImpulse, user, attemptEv.ThrowItems, predictedProjectiles, userSession);
		}
		GunShotEvent shotEv = new GunShotEvent(user, ev.Ammo, fromCoordinates, toCoordinates.Value);
		((EntitySystem)this).RaiseLocalEvent<GunShotEvent>(gunUid, ref shotEv, false);
		PhysicsComponent userPhysics = default(PhysicsComponent);
		if (userImpulse && ((EntitySystem)this).TryComp<PhysicsComponent>(user, ref userPhysics) && _gravity.IsWeightless(user, userPhysics))
		{
			CauseImpulse(fromCoordinates, toCoordinates.Value, user, userPhysics);
		}
		((EntitySystem)this).DirtyField<GunComponent>(gunUid, gun, "BurstActivated", (MetaDataComponent)null);
		((EntitySystem)this).Dirty(gunUid, (IComponent)(object)gun, (MetaDataComponent)null);
		foreach (var item in ev.Ammo)
		{
			EntityUid? ent = item.Entity;
			if (ent.HasValue && ((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null) && (((EntitySystem)this).HasComp<GunIgnorePredictionComponent>(gunUid) || projectiles == null || !projectiles.Contains(ent.Value)))
			{
				((EntitySystem)this).Del(ent);
			}
		}
		return projectiles;
	}

	public void Shoot(EntityUid gunUid, GunComponent gun, EntityUid ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, out bool userImpulse, EntityUid? user = null, bool throwItems = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IShootable shootable = EnsureShootable(ammo);
		Shoot(gunUid, gun, new List<(EntityUid?, IShootable)>(1) { (ammo, shootable) }, fromCoordinates, toCoordinates, out userImpulse, user, throwItems);
	}

	public List<EntityUid>? Shoot(EntityUid gunUid, GunComponent gun, List<(EntityUid? Entity, IShootable Shootable)> ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, out bool userImpulse, EntityUid? user = null, bool throwItems = false, List<int>? predictedProjectiles = null, ICommonSession? userSession = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		userImpulse = true;
		if (user.HasValue)
		{
			SelfBeforeGunShotEvent selfEvent = new SelfBeforeGunShotEvent(user.Value, Entity<GunComponent>.op_Implicit((gunUid, gun)), ammo);
			((EntitySystem)this).RaiseLocalEvent<SelfBeforeGunShotEvent>(user.Value, selfEvent, false);
			if (((CancellableEntityEventArgs)selfEvent).Cancelled)
			{
				userImpulse = false;
				return null;
			}
		}
		MapCoordinates fromMap = TransformSystem.ToMapCoordinates(fromCoordinates, true);
		Vector2 toMap = TransformSystem.ToMapCoordinates(toCoordinates, true).Position;
		Vector2 mapDirection = toMap - fromMap.Position;
		Angle mapAngle = DirectionExtensions.ToAngle(mapDirection);
		Angle angle = GetRecoilAngle(Timing.CurTime, gunUid, gun, DirectionExtensions.ToAngle(mapDirection));
		EntityUid gridUid = default(EntityUid);
		MapGridComponent grid = default(MapGridComponent);
		EntityCoordinates fromEnt = (EntityCoordinates)(MapManager.TryFindGridAt(fromMap, ref gridUid, ref grid) ? TransformSystem.WithEntityId(fromCoordinates, gridUid) : new EntityCoordinates(MapSystem.GetMap(fromMap.MapId), fromMap.Position));
		toMap = fromMap.Position + ((Angle)(ref angle)).ToVec() * mapDirection.Length();
		mapDirection = toMap - fromMap.Position;
		Vector2 gunVelocity = Physics.GetMapLinearVelocity(fromEnt);
		List<EntityUid> shotProjectiles = new List<EntityUid>(ammo.Count);
		CollisionRay ray = default(CollisionRay);
		foreach (var (ent, shootable) in ammo)
		{
			if (throwItems && ent.HasValue)
			{
				Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
				ShootOrThrow(ent.Value, mapDirection, gunVelocity, gun, gunUid, user);
				continue;
			}
			if (!(shootable is CartridgeAmmoComponent cartridge))
			{
				if (!(shootable is AmmoComponent newAmmo))
				{
					if (!(shootable is HitscanPrototype hitscan))
					{
						if (!(shootable is RMCFlamerAmmoProviderComponent flamer))
						{
							if (!(shootable is RMCSprayAmmoProviderComponent spray))
							{
								throw new ArgumentOutOfRangeException();
							}
							if (ent.HasValue)
							{
								_flamer.ShootSpray(Entity<RMCSprayAmmoProviderComponent>.op_Implicit((ent.Value, spray)), Entity<GunComponent>.op_Implicit((gunUid, gun)), user, fromCoordinates, toCoordinates);
							}
						}
						else if (ent.HasValue)
						{
							_flamer.ShootFlamer(Entity<RMCFlamerAmmoProviderComponent>.op_Implicit((ent.Value, flamer)), Entity<GunComponent>.op_Implicit((gunUid, gun)), user, fromCoordinates, toCoordinates);
						}
						continue;
					}
					EntityUid? lastHit = null;
					MapCoordinates from = fromMap;
					EntityCoordinates fromEffect = fromCoordinates;
					Vector2 dir = Vector2Helpers.Normalized(mapDirection);
					EntityUid lastUser = user.GetValueOrDefault(gunUid);
					if (hitscan.Reflective != ReflectType.None)
					{
						for (int reflectAttempt = 0; reflectAttempt < 3; reflectAttempt++)
						{
							((CollisionRay)(ref ray))._002Ector(from.Position, dir, hitscan.CollisionMask);
							List<RayCastResults> rayCastResults = Physics.IntersectRay(from.MapId, ray, hitscan.MaxLength, (EntityUid?)lastUser, false).ToList();
							if (!rayCastResults.Any())
							{
								break;
							}
							RayCastResults result = rayCastResults[0];
							if (!Containers.IsEntityOrParentInContainer(lastUser, (MetaDataComponent)null, (TransformComponent)null))
							{
								foreach (RayCastResults item in rayCastResults)
								{
									RayCastResults collide = item;
									EntityUid hitEntity = ((RayCastResults)(ref collide)).HitEntity;
									EntityUid? target = gun.Target;
									if (!target.HasValue || hitEntity != target.GetValueOrDefault())
									{
										RequireProjectileTargetComponent requireProjectileTargetComponent = ((EntitySystem)this).CompOrNull<RequireProjectileTargetComponent>(((RayCastResults)(ref collide)).HitEntity);
										if (requireProjectileTargetComponent != null && requireProjectileTargetComponent.Active)
										{
											continue;
										}
									}
									result = collide;
									break;
								}
							}
							EntityUid hit = ((RayCastResults)(ref result)).HitEntity;
							lastHit = hit;
							FireEffects(fromEffect, ((RayCastResults)(ref result)).Distance, DirectionExtensions.ToAngle(Vector2Helpers.Normalized(dir)), hitscan, hit);
							HitScanReflectAttemptEvent ev = new HitScanReflectAttemptEvent(user, gunUid, hitscan.Reflective, dir, Reflected: false);
							((EntitySystem)this).RaiseLocalEvent<HitScanReflectAttemptEvent>(hit, ref ev, false);
							if (!ev.Reflected)
							{
								break;
							}
							fromEffect = ((EntitySystem)this).Transform(hit).Coordinates;
							from = TransformSystem.ToMapCoordinates(fromEffect, true);
							dir = ev.Direction;
							lastUser = hit;
						}
					}
					if (lastHit.HasValue)
					{
						EntityUid hitEntity2 = lastHit.Value;
						if (hitscan.StaminaDamage > 0f)
						{
							_stamina.TakeStaminaDamage(hitEntity2, hitscan.StaminaDamage, null, user);
						}
						DamageSpecifier dmg = hitscan.Damage;
						EntityStringRepresentation hitName = ((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(hitEntity2));
						if (dmg != null)
						{
							dmg = Damageable.TryChangeDamage(hitEntity2, dmg * Damageable.UniversalHitscanDamageModifier, ignoreResistances: false, interruptsDoAfters: true, null, user, ent);
						}
						if (dmg != null)
						{
							if (!((EntitySystem)this).Deleted(hitEntity2, (MetaDataComponent)null))
							{
								Filter hitFilter = Filter.Pvs(hitEntity2, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null);
								if (_netManager.IsServer && GunPrediction && userSession != null)
								{
									hitFilter = hitFilter.RemovePlayer(userSession);
								}
								if (dmg.AnyPositive())
								{
									_color.RaiseEffect(Color.Red, new List<EntityUid> { hitEntity2 }, Filter.Pvs(hitEntity2, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null));
								}
								PlayImpactSound(hitEntity2, dmg, hitscan.Sound, hitscan.ForceSound);
							}
							if (user.HasValue)
							{
								ISharedAdminLogManager logs = Logs;
								LogStringHandler handler = new LogStringHandler(37, 3);
								handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "user", "ToPrettyString(user.Value)");
								handler.AppendLiteral(" hit ");
								handler.AppendFormatted<EntityStringRepresentation>(hitName, "target", "hitName");
								handler.AppendLiteral(" using hitscan and dealt ");
								handler.AppendFormatted(dmg.GetTotal(), "damage", "dmg.GetTotal()");
								handler.AppendLiteral(" damage");
								logs.Add(LogType.HitScanHit, ref handler);
							}
							else
							{
								ISharedAdminLogManager logs2 = Logs;
								LogStringHandler handler2 = new LogStringHandler(31, 2);
								handler2.AppendFormatted<EntityStringRepresentation>(hitName, "target", "hitName");
								handler2.AppendLiteral(" hit by hitscan dealing ");
								handler2.AppendFormatted(dmg.GetTotal(), "damage", "dmg.GetTotal()");
								handler2.AppendLiteral(" damage");
								logs2.Add(LogType.HitScanHit, ref handler2);
							}
						}
					}
					else
					{
						FireEffects(fromEffect, hitscan.MaxLength, DirectionExtensions.ToAngle(dir), hitscan);
					}
					PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
					Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
				}
				else
				{
					if (_netManager.IsServer || GunPrediction)
					{
						CreateAndFireProjectiles(ent.Value, newAmmo);
					}
					else
					{
						MuzzleFlash(gunUid, newAmmo, DirectionExtensions.ToAngle(mapDirection), user);
						PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
					}
					Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
					if (_netManager.IsClient)
					{
						RemoveShootable(ent.Value);
					}
					MarkPredicted(ent.Value, 0);
				}
				continue;
			}
			if (!cartridge.Spent)
			{
				if (_netManager.IsServer || GunPrediction)
				{
					EntityUid uid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(cartridge.Prototype), fromEnt);
					CreateAndFireProjectiles(uid, cartridge);
					if (_netManager.IsClient && ((EntitySystem)this).HasComp<GunIgnorePredictionComponent>(gunUid))
					{
						predictedProjectiles?.RemoveAll((int i) => i == uid.Id);
						((EntitySystem)this).QueueDel((EntityUid?)uid);
					}
					((EntitySystem)this).RaiseLocalEvent<AmmoShotEvent>(ent.Value, new AmmoShotEvent
					{
						FiredProjectiles = shotProjectiles
					}, false);
					SetCartridgeSpent(ent.Value, cartridge, spent: true);
					if (cartridge.DeleteOnSpawn && (_netManager.IsServer || ((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null)))
					{
						((EntitySystem)this).Del((EntityUid?)ent.Value);
					}
				}
				else
				{
					MuzzleFlash(gunUid, cartridge, DirectionExtensions.ToAngle(mapDirection), user);
					PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
				}
			}
			else
			{
				userImpulse = false;
				Audio.PlayPredicted(gun.SoundEmpty, gunUid, user, (AudioParams?)null);
			}
			Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
			if (!cartridge.DeleteOnSpawn && !Containers.IsEntityInContainer(ent.Value, (MetaDataComponent)null))
			{
				EjectCartridge(ent.Value, angle);
			}
			if (((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null))
			{
				((EntitySystem)this).Del((EntityUid?)ent.Value);
			}
			else
			{
				((EntitySystem)this).Dirty(ent.Value, (IComponent)(object)cartridge, (MetaDataComponent)null);
			}
		}
		((EntitySystem)this).RaiseLocalEvent<AmmoShotEvent>(gunUid, new AmmoShotEvent
		{
			FiredProjectiles = shotProjectiles
		}, false);
		ISharedAdminLogManager logs3 = Logs;
		LogStringHandler handler3 = new LogStringHandler(36, 4);
		handler3.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "ToPrettyString(user)");
		handler3.AppendLiteral(" shot ");
		handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(gunUid)), "ToPrettyString(gunUid)");
		handler3.AppendLiteral(" with ");
		handler3.AppendFormatted(shotProjectiles.Count, "shotProjectiles.Count");
		handler3.AppendLiteral(" projectiles aiming at ");
		handler3.AppendFormatted<MapCoordinates>(TransformSystem.ToMapCoordinates(toCoordinates, true), "TransformSystem.ToMapCoordinates(toCoordinates)");
		handler3.AppendLiteral(".");
		logs3.Add(LogType.RMCGunShot, LogImpact.Low, ref handler3);
		return shotProjectiles;
		void CreateAndFireProjectiles(EntityUid ammoEnt, AmmoComponent ammoComp)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			if (predictedProjectiles == null)
			{
				predictedProjectiles = new List<int>();
			}
			MarkPredicted(ammoEnt, 0);
			ProjectileSpreadComponent ammoSpreadComp = default(ProjectileSpreadComponent);
			if (((EntitySystem)this).TryComp<ProjectileSpreadComponent>(ammoEnt, ref ammoSpreadComp))
			{
				GunGetAmmoSpreadEvent spreadEvent = new GunGetAmmoSpreadEvent(ammoSpreadComp.Spread);
				((EntitySystem)this).RaiseLocalEvent<GunGetAmmoSpreadEvent>(gunUid, ref spreadEvent, false);
				Angle[] angles = LinearSpread(mapAngle - Angle.op_Implicit(Angle.op_Implicit(spreadEvent.Spread) / 2.0), mapAngle + Angle.op_Implicit(Angle.op_Implicit(spreadEvent.Spread) / 2.0), ammoSpreadComp.Count);
				ShootOrThrow(ammoEnt, ((Angle)(ref angles[0])).ToVec(), gunVelocity, gun, gunUid, user);
				shotProjectiles.Add(ammoEnt);
				for (int i = 1; i < ammoSpreadComp.Count; i++)
				{
					EntityUid newuid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ammoSpreadComp.Proto), fromEnt);
					ShootOrThrow(newuid, ((Angle)(ref angles[i])).ToVec(), gunVelocity, gun, gunUid, user);
					shotProjectiles.Add(newuid);
					MarkPredicted(newuid, i);
				}
			}
			else
			{
				ShootOrThrow(ammoEnt, mapDirection, gunVelocity, gun, gunUid, user);
				shotProjectiles.Add(ammoEnt);
			}
			MuzzleFlash(gunUid, ammoComp, DirectionExtensions.ToAngle(mapDirection), user);
			PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
		}
		void MarkPredicted(EntityUid projectile, int index)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			int predicted = default(int);
			if (GunPrediction && predictedProjectiles != null && userSession != null && Extensions.TryGetValue<int>((IList<int>)predictedProjectiles, index, ref predicted))
			{
				PredictedProjectileServerComponent comp = new PredictedProjectileServerComponent
				{
					Shooter = userSession,
					ClientId = predicted,
					ClientEnt = user
				};
				((EntitySystem)this).AddComp<PredictedProjectileServerComponent>(projectile, comp, true);
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	private void PlayGunshotAudio(EntityUid gunUid, GunComponent gun, EntityUid? user, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		SpatialGunshotComponent spatial = default(SpatialGunshotComponent);
		if (_netManager.IsClient || !((EntitySystem)this).TryComp<SpatialGunshotComponent>(gunUid, ref spatial))
		{
			Audio.PlayPredicted(gun.SoundGunshotModified, gunUid, user, (AudioParams?)null);
		}
		else
		{
			if (gun.SoundGunshotModified == null)
			{
				return;
			}
			SoundSpecifier nearSound = gun.SoundGunshotModified;
			SoundSpecifier farSound = spatial.FarSound;
			float audioRange = spatial.AudioRange;
			float nearAudioRange = spatial.NearAudioRange;
			float nearRange = spatial.NearRange;
			float coneAngle = spatial.ConeAngle;
			float nearVolume = spatial.NearVolume;
			bool farSoundDisabled = false;
			PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
			if (((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(gunUid, ref modulesComp))
			{
				PubgSpatialGunshotModifiers modifiers = _pubgWeaponModules.GetSpatialGunshotModifiers(gunUid, modulesComp);
				if (modifiers.FarSoundOverride != null)
				{
					farSound = modifiers.FarSoundOverride;
				}
				if (modifiers.DisableFarSound)
				{
					farSound = null;
					farSoundDisabled = true;
				}
				audioRange = MathF.Max(1f, spatial.AudioRange * modifiers.AudioRangeMultiplier);
				nearAudioRange = Math.Clamp(spatial.NearAudioRange * modifiers.AudioRangeMultiplier, 1f, audioRange);
				nearRange = Math.Clamp(spatial.NearRange * modifiers.NearRangeMultiplier, 1f, audioRange);
				coneAngle = Math.Clamp(spatial.ConeAngle * modifiers.ConeAngleMultiplier, 1f, 180f);
				nearVolume = Math.Clamp(nearVolume * modifiers.NearVolumeMultiplier, -12f, 12f);
			}
			nearAudioRange = MathF.Min(nearAudioRange, audioRange);
			if (!((EntityCoordinates)(ref fromCoordinates)).IsValid((IEntityManager)(object)base.EntityManager) || !((EntityCoordinates)(ref toCoordinates)).IsValid((IEntityManager)(object)base.EntityManager))
			{
				return;
			}
			MapCoordinates fromMap = TransformSystem.ToMapCoordinates(fromCoordinates, true);
			MapCoordinates toMap = TransformSystem.ToMapCoordinates(toCoordinates, true);
			if (fromMap.MapId == MapId.Nullspace || toMap.MapId == MapId.Nullspace)
			{
				return;
			}
			SoundSpecifier effectiveFarSound = farSound ?? (farSoundDisabled ? null : nearSound);
			Vector2 delta = toMap.Position - fromMap.Position;
			Vector2 shootDirection = ((delta.LengthSquared() > 1E-06f) ? Vector2Helpers.Normalized(delta) : Vector2.Zero);
			Filter nearFilter = Filter.Empty();
			Filter farFilter = Filter.Empty();
			foreach (ICommonSession player in Filter.Empty().AddInRange(fromMap, audioRange, _playerManager, (IEntityManager)(object)base.EntityManager).Recipients)
			{
				if (!player.AttachedEntity.HasValue)
				{
					continue;
				}
				EntityUid? attachedEntity = player.AttachedEntity;
				EntityUid? val = user;
				if (attachedEntity.HasValue == val.HasValue && (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() == val.GetValueOrDefault()))
				{
					continue;
				}
				MapCoordinates listenerPos = TransformSystem.GetMapCoordinates(player.AttachedEntity.Value, (TransformComponent)null);
				float distance = (listenerPos.Position - fromMap.Position).Length();
				if (distance <= nearRange)
				{
					nearFilter.AddPlayer(player);
					continue;
				}
				Vector2 toListener = Vector2Helpers.Normalized(listenerPos.Position - fromMap.Position);
				float angle = MathF.Acos(MathHelper.Clamp(Vector2.Dot(shootDirection, toListener), -1f, 1f)) * (180f / (float)Math.PI);
				if (shootDirection != Vector2.Zero && angle <= coneAngle && distance <= nearAudioRange)
				{
					nearFilter.AddPlayer(player);
				}
				else if (effectiveFarSound != null)
				{
					farFilter.AddPlayer(player);
				}
			}
			AudioParams val2;
			if (nearFilter.Count > 0)
			{
				val2 = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(nearAudioRange);
				AudioParams nearParams = ((AudioParams)(ref val2)).WithVolume(nearVolume);
				Audio.PlayStatic(nearSound, nearFilter, fromCoordinates, true, (AudioParams?)nearParams);
			}
			if (effectiveFarSound != null && farFilter.Count > 0)
			{
				AudioParams farParams = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(audioRange);
				Audio.PlayStatic(effectiveFarSound, farFilter, fromCoordinates, true, (AudioParams?)farParams);
			}
			SoundSpecifier interiorSound = effectiveFarSound ?? nearSound;
			AudioParams val3;
			if (effectiveFarSound == null)
			{
				val2 = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(audioRange);
				val3 = ((AudioParams)(ref val2)).WithVolume(nearVolume);
			}
			else
			{
				val3 = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(audioRange);
			}
			AudioParams interiorParams = val3;
			RelayGunshotIntoVehicleInteriors(interiorSound, interiorParams, fromCoordinates);
		}
	}

	private void RelayGunshotIntoVehicleInteriors(SoundSpecifier sound, AudioParams baseParams, EntityCoordinates sourceCoords)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityCoordinates)(ref sourceCoords)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		MapCoordinates mapCoords = TransformSystem.ToMapCoordinates(sourceCoords, true);
		if (mapCoords.MapId == MapId.Nullspace)
		{
			return;
		}
		Vector2 sourcePosition = mapCoords.Position;
		EntityQueryEnumerator<RMCVehicleInteriorAudioRelayComponent, VehicleEnterComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCVehicleInteriorAudioRelayComponent, VehicleEnterComponent, TransformComponent>();
		EntityUid vehicleUid = default(EntityUid);
		RMCVehicleInteriorAudioRelayComponent relay = default(RMCVehicleInteriorAudioRelayComponent);
		VehicleEnterComponent vehicleEnterComponent = default(VehicleEnterComponent);
		TransformComponent vehicleXform = default(TransformComponent);
		while (query.MoveNext(ref vehicleUid, ref relay, ref vehicleEnterComponent, ref vehicleXform))
		{
			if (vehicleXform.MapID != mapCoords.MapId)
			{
				continue;
			}
			Box2 val = _lookup.GetWorldAABB(vehicleUid, (TransformComponent)null);
			val = ((Box2)(ref val)).Enlarged(relay.ExteriorRange);
			if (!((Box2)(ref val)).Contains(sourcePosition, true) || !_rmcVehicle.TryGetInteriorEntryCoordinates(vehicleUid, out var interiorCoords) || !_rmcVehicle.TryGetInteriorMapId(vehicleUid, out var interiorMapId))
			{
				continue;
			}
			ICommonSession[] sessions = _playerManager.Sessions;
			foreach (ICommonSession session in sessions)
			{
				if (TryGetVehicleRelayCoordinates(session, vehicleUid, mapCoords, interiorMapId, interiorCoords, relay, out var relayCoords))
				{
					AudioParams val2 = ((AudioParams)(ref baseParams)).AddVolume(relay.InteriorVolumeOffset);
					val2 = ((AudioParams)(ref val2)).WithMaxDistance(relay.InteriorMaxDistance);
					AudioParams audioParams = ((AudioParams)(ref val2)).WithReferenceDistance(relay.InteriorReferenceDistance);
					(EntityUid, AudioComponent)? relayed = Audio.PlayStatic(sound, Filter.SinglePlayer(session), relayCoords, false, (AudioParams?)audioParams);
					if (relayed.HasValue && relay.InteriorNoOcclusion)
					{
						AudioComponent item = relayed.Value.Item2;
						item.Flags = (AudioFlags)(item.Flags | 2);
						((EntitySystem)this).Dirty(relayed.Value.Item1, (IComponent)(object)relayed.Value.Item2, (MetaDataComponent)null);
					}
				}
			}
		}
	}

	private bool TryGetVehicleRelayCoordinates(ICommonSession session, EntityUid vehicle, MapCoordinates mapCoords, MapId interiorMapId, EntityCoordinates interiorCoords, RMCVehicleInteriorAudioRelayComponent relay, out EntityCoordinates relayCoords)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		relayCoords = EntityCoordinates.Invalid;
		EntityUid? attachedEntity = session.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid attached = attachedEntity.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(attached, (MetaDataComponent)null))
			{
				RMCVehicleInteriorOccupantComponent occupant = default(RMCVehicleInteriorOccupantComponent);
				if (((EntitySystem)this).TryComp<RMCVehicleInteriorOccupantComponent>(attached, ref occupant))
				{
					attachedEntity = occupant.Vehicle;
					if (attachedEntity.HasValue && !(attachedEntity.GetValueOrDefault() != vehicle))
					{
						if (TransformSystem.GetMapId(Entity<TransformComponent>.op_Implicit(attached)) != interiorMapId)
						{
							return false;
						}
						EyeComponent eyeForOuter = default(EyeComponent);
						if (((EntitySystem)this).TryComp<EyeComponent>(attached, ref eyeForOuter))
						{
							attachedEntity = eyeForOuter.Target;
							if (attachedEntity.HasValue)
							{
								EntityUid outerTarget = attachedEntity.GetValueOrDefault();
								if (((EntitySystem)this).Exists(outerTarget) && TransformSystem.GetMapId(Entity<TransformComponent>.op_Implicit(outerTarget)) == mapCoords.MapId)
								{
									relayCoords = new EntityCoordinates(vehicle, Vector2.Zero);
									return ((EntityCoordinates)(ref relayCoords)).IsValid((IEntityManager)(object)base.EntityManager);
								}
							}
						}
						Vector2 vehPos = TransformSystem.GetWorldPosition(vehicle);
						Angle worldRotation = TransformSystem.GetWorldRotation(vehicle);
						Vector2 delta = mapCoords.Position - vehPos;
						Angle val = -worldRotation;
						Vector2 local = ((Angle)(ref val)).RotateVec(ref delta) * relay.InsideScale;
						float clampSq = relay.InsideClamp * relay.InsideClamp;
						if (local.LengthSquared() > clampSq)
						{
							local = local / local.Length() * relay.InsideClamp;
						}
						relayCoords = ((EntityCoordinates)(ref interiorCoords)).Offset(relay.InsideOffset + local);
						return ((EntityCoordinates)(ref relayCoords)).IsValid((IEntityManager)(object)base.EntityManager);
					}
				}
				return false;
			}
		}
		return false;
	}

	private Angle GetRecoilAngle(TimeSpan curTime, EntityUid gunUid, GunComponent component, Angle direction)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		double timeSinceLastFire = (curTime - component.LastFire).TotalSeconds;
		if (!double.IsFinite(timeSinceLastFire) || timeSinceLastFire < 0.0)
		{
			timeSinceLastFire = 0.0;
		}
		double currentTheta = component.CurrentAngle.Theta;
		double increaseTheta = component.AngleIncreaseModified.Theta;
		double decayTheta = component.AngleDecayModified.Theta;
		double minTheta = component.MinAngleModified.Theta;
		double maxTheta = component.MaxAngleModified.Theta;
		if (!double.IsFinite(currentTheta))
		{
			currentTheta = 0.0;
		}
		if (!double.IsFinite(increaseTheta))
		{
			increaseTheta = 0.0;
		}
		if (!double.IsFinite(decayTheta))
		{
			decayTheta = 0.0;
		}
		if (!double.IsFinite(minTheta))
		{
			minTheta = 0.0;
		}
		if (!double.IsFinite(maxTheta))
		{
			maxTheta = minTheta;
		}
		if (minTheta > maxTheta)
		{
			double num = maxTheta;
			maxTheta = minTheta;
			minTheta = num;
		}
		double newTheta = MathHelper.Clamp(currentTheta + increaseTheta - decayTheta * timeSinceLastFire, minTheta, maxTheta);
		if (!double.IsFinite(newTheta))
		{
			newTheta = minTheta;
		}
		component.CurrentAngle = new Angle(newTheta);
		component.LastFire = component.NextFire;
		float random = new Xoroshiro64S((long)(((ulong)Timing.CurTick.Value << 32) | (uint)((EntitySystem)this).GetNetEntity(gunUid, (MetaDataComponent)null).Id)).NextFloat(-0.5f, 0.5f);
		double spread = component.CurrentAngle.Theta * (double)random;
		if (!double.IsFinite(spread))
		{
			spread = 0.0;
		}
		double angleTheta = direction.Theta + spread;
		if (!double.IsFinite(angleTheta))
		{
			angleTheta = direction.Theta;
		}
		return new Angle(angleTheta);
	}

	private void ShootOrThrow(EntityUid uid, Vector2 mapDirection, Vector2 gunVelocity, GunComponent gun, EntityUid gunUid, EntityUid? user)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = gun.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(target2, (MetaDataComponent)null))
			{
				TargetedProjectileComponent targeted = ((EntitySystem)this).EnsureComp<TargetedProjectileComponent>(uid);
				targeted.Target = target2;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)targeted, (MetaDataComponent)null);
			}
		}
		if (!((EntitySystem)this).HasComp<ProjectileComponent>(uid))
		{
			RemoveShootable(uid);
			ThrowingSystem.TryThrow(uid, mapDirection, gun.ProjectileSpeedModified, user, 2f, null, compensateFriction: false, recoil: false, animated: true, playSound: true, doSpin: true, unanchor: false, rotate: false);
		}
		else
		{
			ShootProjectile(uid, mapDirection, gunVelocity, gunUid, user, gun.ProjectileSpeedModified);
		}
	}

	private void FireEffects(EntityCoordinates fromCoordinates, float distance, Angle mapDirection, HitscanPrototype hitscan, EntityUid? hitEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		List<(NetCoordinates, Angle, SpriteSpecifier, float)> sprites = new List<(NetCoordinates, Angle, SpriteSpecifier, float)>();
		EntityUid? gridUid = TransformSystem.GetGrid(fromCoordinates);
		Angle angle = mapDirection;
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		TransformComponent gridXform = default(TransformComponent);
		if (xformQuery.TryGetComponent(gridUid, ref gridXform))
		{
			ValueTuple<Vector2, Angle, Matrix3x2> worldPositionRotationInvMatrix = TransformSystem.GetWorldPositionRotationInvMatrix(gridXform, xformQuery);
			Angle gridRot = worldPositionRotationInvMatrix.Item2;
			Matrix3x2 gridInvMatrix = worldPositionRotationInvMatrix.Item3;
			((EntityCoordinates)(ref fromCoordinates))._002Ector(gridUid.Value, Vector2.Transform(TransformSystem.ToMapCoordinates(fromCoordinates, true).Position, gridInvMatrix));
			angle -= gridRot;
		}
		if (distance >= 1f)
		{
			if (hitscan.MuzzleFlash != null)
			{
				EntityCoordinates coords = ((EntityCoordinates)(ref fromCoordinates)).Offset(Vector2Helpers.Normalized(((Angle)(ref angle)).ToVec()) / 2f);
				NetCoordinates netCoords = ((EntitySystem)this).GetNetCoordinates(coords, (MetaDataComponent)null);
				sprites.Add((netCoords, angle, hitscan.MuzzleFlash, 1f));
			}
			if (hitscan.TravelFlash != null)
			{
				EntityCoordinates coords2 = ((EntityCoordinates)(ref fromCoordinates)).Offset(((Angle)(ref angle)).ToVec() * (distance + 0.5f) / 2f);
				NetCoordinates netCoords2 = ((EntitySystem)this).GetNetCoordinates(coords2, (MetaDataComponent)null);
				sprites.Add((netCoords2, angle, hitscan.TravelFlash, distance - 1.5f));
			}
		}
		if (hitscan.ImpactFlash != null)
		{
			EntityCoordinates coords3 = ((EntityCoordinates)(ref fromCoordinates)).Offset(((Angle)(ref angle)).ToVec() * distance);
			NetCoordinates netCoords3 = ((EntitySystem)this).GetNetCoordinates(coords3, (MetaDataComponent)null);
			sprites.Add((netCoords3, ((Angle)(ref angle)).FlipPositive(), hitscan.ImpactFlash, 1f));
		}
		if (_netManager.IsServer && sprites.Count > 0)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new HitscanEvent
			{
				Sprites = sprites
			}, Filter.Pvs(fromCoordinates, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null), true);
		}
	}

	private Angle[] LinearSpread(Angle start, Angle end, int intervals)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Angle[] angles = (Angle[])(object)new Angle[intervals];
		for (int i = 0; i <= intervals - 1; i++)
		{
			angles[i] = new Angle(Angle.op_Implicit(start + Angle.op_Implicit(Angle.op_Implicit(end - start) * (double)i / (double)(intervals - 1))));
		}
		return angles;
	}

	public void PlayImpactSound(EntityUid otherEntity, DamageSpecifier? modifiedDamage, SoundSpecifier? weaponSound, bool forceWeaponSound, Filter? filter = null, EntityUid? projectile = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient && ((EntitySystem)this).HasComp<PredictedProjectileServerComponent>(projectile))
		{
			return;
		}
		if (filter == null)
		{
			filter = Filter.Pvs(otherEntity, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
		}
		bool playedSound = false;
		RangedDamageSoundComponent rangedSound = default(RangedDamageSoundComponent);
		if (!forceWeaponSound && modifiedDamage != null && modifiedDamage.GetTotal() > 0 && ((EntitySystem)this).TryComp<RangedDamageSoundComponent>(otherEntity, ref rangedSound))
		{
			string type = SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, ProtoManager);
			if (type != null)
			{
				Dictionary<string, SoundSpecifier>? soundTypes = rangedSound.SoundTypes;
				if (soundTypes != null && soundTypes.TryGetValue(type, out SoundSpecifier damageSoundType) && filter.Count > 0)
				{
					Audio.PlayEntity(damageSoundType, filter, otherEntity, true, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
					playedSound = true;
					goto IL_010a;
				}
			}
			if (type != null)
			{
				Dictionary<string, SoundSpecifier>? soundGroups = rangedSound.SoundGroups;
				if (soundGroups != null && soundGroups.TryGetValue(type, out SoundSpecifier damageSoundGroup) && filter.Count > 0)
				{
					Audio.PlayEntity(damageSoundGroup, filter, otherEntity, true, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
					playedSound = true;
				}
			}
		}
		goto IL_010a;
		IL_010a:
		if (!playedSound && weaponSound != null && filter.Count > 0)
		{
			Audio.PlayEntity(weaponSound, filter, otherEntity, true, (AudioParams?)null);
		}
	}

	private void Recoil(EntityUid? user, Vector2 recoil, float recoilScalar)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!_netManager.IsServer && Timing.IsFirstTimePredicted && user.HasValue && !(recoil == Vector2.Zero) && recoilScalar != 0f && !((EntitySystem)this).HasComp<WeaponControllerComponent>(user.Value))
		{
			_recoil.KickCamera(user.Value, Vector2Helpers.Normalized(recoil) * 0.5f * recoilScalar);
		}
	}

	public virtual void ShootProjectile(EntityUid uid, Vector2 direction, Vector2 gunVelocity, EntityUid? gunUid, EntityUid? user = null, float speed = 20f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = ((EntitySystem)this).EnsureComp<PhysicsComponent>(uid);
		Physics.SetBodyStatus(uid, physics, (BodyStatus)1, true);
		Vector2 targetMapVelocity = gunVelocity + Vector2Helpers.Normalized(direction) * speed;
		Vector2 currentMapVelocity = Physics.GetMapLinearVelocity(uid, physics, (TransformComponent)null);
		Vector2 finalLinear = physics.LinearVelocity + targetMapVelocity - currentMapVelocity;
		Physics.SetLinearVelocity(uid, finalLinear, true, true, (FixturesComponent)null, physics);
		ProjectileComponent projectile = ((EntitySystem)this).EnsureComp<ProjectileComponent>(uid);
		projectile.Weapon = gunUid;
		EntityUid? shooter = user ?? gunUid;
		if (shooter.HasValue)
		{
			Projectiles.SetShooter(uid, projectile, shooter.Value);
		}
		TransformSystem.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(uid), DirectionExtensions.ToWorldAngle(direction) + projectile.Angle);
	}

	protected abstract void Popup(string message, EntityUid? uid, EntityUid? user);

	public virtual void UpdateAmmoCount(EntityUid uid, bool prediction = true, int artificialIncrease = 0)
	{
	}

	protected void SetCartridgeSpent(EntityUid uid, CartridgeAmmoComponent cartridge, bool spent)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (cartridge.Spent != spent)
		{
			((EntitySystem)this).DirtyField<CartridgeAmmoComponent>(uid, cartridge, "Spent", (MetaDataComponent)null);
		}
		cartridge.Spent = spent;
		Appearance.SetData(uid, (Enum)AmmoVisuals.Spent, (object)spent, (AppearanceComponent)null);
		if (spent)
		{
			TagSystem.AddTag(uid, ProtoId<TagPrototype>.op_Implicit("HideContextMenu"));
		}
		else
		{
			TagSystem.RemoveTag(uid, ProtoId<TagPrototype>.op_Implicit("HideContextMenu"));
		}
	}

	protected void EjectCartridge(EntityUid entity, Angle? angle = null, bool playSound = true)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		Vector2 offsetPos = Random.NextVector2(0.4f);
		TransformComponent xform = ((EntitySystem)this).Transform(entity);
		EntityCoordinates coordinates = xform.Coordinates;
		coordinates = ((EntityCoordinates)(ref coordinates)).Offset(offsetPos);
		TransformSystem.SetLocalRotation(entity, Random.NextAngle(), xform);
		TransformSystem.SetCoordinates(entity, xform, coordinates, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
		if (angle.HasValue)
		{
			Angle ejectAngle = angle.Value;
			ejectAngle += Angle.op_Implicit(3.7f);
			ThrowingSystem.TryThrow(entity, Vector2Helpers.Normalized(((Angle)(ref ejectAngle)).ToVec()) / 100f, 5f, null, 2f, null, compensateFriction: false, recoil: true, animated: true, playSound: true, doSpin: true, unanchor: false, rotate: false);
		}
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		if (playSound && ((EntitySystem)this).TryComp<CartridgeAmmoComponent>(entity, ref cartridge))
		{
			SharedAudioSystem audio = Audio;
			SoundSpecifier? ejectSound = cartridge.EjectSound;
			AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f);
			audio.PlayPvs(ejectSound, entity, (AudioParams?)((AudioParams)(ref val)).WithVolume(-1f));
		}
	}

	public IShootable EnsureShootable(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		if (((EntitySystem)this).TryComp<CartridgeAmmoComponent>(uid, ref cartridge))
		{
			return cartridge;
		}
		return ((EntitySystem)this).EnsureComp<AmmoComponent>(uid);
	}

	protected void RemoveShootable(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<CartridgeAmmoComponent>(uid);
		((EntitySystem)this).RemCompDeferred<AmmoComponent>(uid);
	}

	protected void MuzzleFlash(EntityUid gun, AmmoComponent component, Angle worldAngle, EntityUid? user = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		GunMuzzleFlashAttemptEvent attemptEv = default(GunMuzzleFlashAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<GunMuzzleFlashAttemptEvent>(gun, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return;
		}
		EntProtoId? sprite = component.MuzzleFlash;
		if (sprite.HasValue)
		{
			Vector2 muzzleFlashOffset = component.MuzzleFlashOffset;
			Vector2 muzzleFlashOriginOffset = Vector2.Zero;
			GunComponent gunComp = default(GunComponent);
			if (((EntitySystem)this).TryComp<GunComponent>(gun, ref gunComp))
			{
				RMCBeforeMuzzleFlashEvent beforeEv = new RMCBeforeMuzzleFlashEvent(gun, gunComp.ShootOriginOffset);
				((EntitySystem)this).RaiseLocalEvent<RMCBeforeMuzzleFlashEvent>(gun, ref beforeEv, false);
				gun = beforeEv.Weapon;
				muzzleFlashOriginOffset = beforeEv.Offset;
			}
			NetEntity netEntity = ((EntitySystem)this).GetNetEntity(gun, (MetaDataComponent)null);
			EntProtoId? val = sprite;
			MuzzleFlashEvent ev = new MuzzleFlashEvent(netEntity, val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, worldAngle, muzzleFlashOffset, muzzleFlashOriginOffset);
			CreateEffect(gun, ev, gun, user, muzzleFlashOffset, muzzleFlashOriginOffset);
		}
	}

	public void CauseImpulse(EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid user, PhysicsComponent userPhysics)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Vector2 fromMap = TransformSystem.ToMapCoordinates(fromCoordinates, true).Position;
		Vector2 impulseVector = Vector2Helpers.Normalized(TransformSystem.ToMapCoordinates(toCoordinates, true).Position - fromMap) * 25f;
		Physics.ApplyLinearImpulse(user, -impulseVector, (FixturesComponent)null, userPhysics);
	}

	public void RefreshModifiers(Entity<GunComponent?> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GunComponent>(Entity<GunComponent>.op_Implicit(gun), ref gun.Comp, false))
		{
			GunComponent comp = gun.Comp;
			GunRefreshModifiersEvent ev = new GunRefreshModifiersEvent(Entity<GunComponent>.op_Implicit((Entity<GunComponent>.op_Implicit(gun), comp)), comp.SoundGunshot, comp.CameraRecoilScalar, comp.AngleIncrease, comp.AngleDecay, comp.MaxAngle, comp.MinAngle, comp.ShotsPerBurst, comp.FireRate, comp.ProjectileSpeed);
			((EntitySystem)this).RaiseLocalEvent<GunRefreshModifiersEvent>(Entity<GunComponent>.op_Implicit(gun), ref ev, false);
			if (comp.SoundGunshotModified != ev.SoundGunshot)
			{
				comp.SoundGunshotModified = ev.SoundGunshot;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "SoundGunshotModified", (MetaDataComponent)null);
			}
			if (!MathHelper.CloseTo(comp.CameraRecoilScalarModified, ev.CameraRecoilScalar, 1E-07f))
			{
				comp.CameraRecoilScalarModified = ev.CameraRecoilScalar;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "CameraRecoilScalarModified", (MetaDataComponent)null);
			}
			if (!((Angle)(ref comp.AngleIncreaseModified)).EqualsApprox(ev.AngleIncrease))
			{
				comp.AngleIncreaseModified = ev.AngleIncrease;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "AngleIncreaseModified", (MetaDataComponent)null);
			}
			if (!((Angle)(ref comp.AngleDecayModified)).EqualsApprox(ev.AngleDecay))
			{
				comp.AngleDecayModified = ev.AngleDecay;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "AngleDecayModified", (MetaDataComponent)null);
			}
			if (!((Angle)(ref comp.MaxAngleModified)).EqualsApprox(ev.MaxAngle))
			{
				comp.MaxAngleModified = ev.MaxAngle;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "MaxAngleModified", (MetaDataComponent)null);
			}
			if (!((Angle)(ref comp.MinAngleModified)).EqualsApprox(ev.MinAngle))
			{
				comp.MinAngleModified = ev.MinAngle;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "MinAngleModified", (MetaDataComponent)null);
			}
			if (comp.ShotsPerBurstModified != ev.ShotsPerBurst)
			{
				comp.ShotsPerBurstModified = ev.ShotsPerBurst;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "ShotsPerBurstModified", (MetaDataComponent)null);
			}
			if (!MathHelper.CloseTo(comp.FireRateModified, ev.FireRate, 1E-07f))
			{
				comp.FireRateModified = ev.FireRate;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "FireRateModified", (MetaDataComponent)null);
			}
			if (!MathHelper.CloseTo(comp.ProjectileSpeedModified, ev.ProjectileSpeed, 1E-07f))
			{
				comp.ProjectileSpeedModified = ev.ProjectileSpeed;
				((EntitySystem)this).DirtyField<GunComponent>(gun, "ProjectileSpeedModified", (MetaDataComponent)null);
			}
		}
	}

	protected abstract void CreateEffect(EntityUid gunUid, MuzzleFlashEvent message, EntityUid? user = null, EntityUid? player = null, Vector2 offset = default(Vector2), Vector2 originOffset = default(Vector2));

	private void OnExamine(EntityUid uid, GunComponent component, ExaminedEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange || !component.ShowExamineText || ((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("GunComponent"))
		{
			args.PushMarkup(base.Loc.GetString("gun-selected-mode-examine", (ValueTuple<string, object>)("color", "cyan"), (ValueTuple<string, object>)("mode", GetLocSelector(component.SelectedMode))));
			args.PushMarkup(base.Loc.GetString("gun-fire-rate-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("fireRate", $"{component.FireRateModified:0.0}")));
		}
	}

	private string GetLocSelector(SelectiveFire mode)
	{
		return base.Loc.GetString("gun-" + mode);
	}

	private void OnAltVerb(EntityUid uid, GunComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && args.CanComplexInteract && args.Hands != null && component.SelectedMode != component.AvailableModes && !((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			SelectiveFire nextMode = GetNextMode(component);
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					SelectFire(uid, component, nextMode, args.User);
				},
				Text = base.Loc.GetString("gun-selector-verb", (ValueTuple<string, object>)("mode", GetLocSelector(nextMode))),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}

	public SelectiveFire GetNextMode(GunComponent component)
	{
		List<SelectiveFire> modes = new List<SelectiveFire>();
		SelectiveFire[] values = Enum.GetValues<SelectiveFire>();
		foreach (SelectiveFire mode in values)
		{
			if ((mode & component.AvailableModes) != SelectiveFire.Invalid)
			{
				modes.Add(mode);
			}
		}
		int index = modes.IndexOf(component.SelectedMode);
		return modes[(index + 1) % modes.Count];
	}

	public void SelectFire(EntityUid uid, GunComponent component, SelectiveFire fire, EntityUid? user = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (component.SelectedMode == fire)
		{
			return;
		}
		component.SelectedMode = fire;
		if (!((EntitySystem)this).Paused(uid, (MetaDataComponent)null))
		{
			TimeSpan curTime = Timing.CurTime;
			TimeSpan cooldown = TimeSpan.FromSeconds(0.30000001192092896);
			if (component.NextFire < curTime)
			{
				component.NextFire = curTime + cooldown;
			}
			else
			{
				component.NextFire += cooldown;
			}
		}
		Audio.PlayPredicted(component.SoundMode, uid, user, (AudioParams?)null);
		Popup(base.Loc.GetString("gun-selected-mode", (ValueTuple<string, object>)("mode", GetLocSelector(fire))), uid, user);
		RMCFireModeChangedEvent ev = default(RMCFireModeChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<RMCFireModeChangedEvent>(uid, ref ev, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public void CycleFire(EntityUid uid, GunComponent component, EntityUid? user = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (component.SelectedMode != component.AvailableModes)
		{
			SelectiveFire nextMode = GetNextMode(component);
			SelectFire(uid, component, nextMode, user);
		}
	}

	private void OnCycleMode(EntityUid uid, GunComponent component, CycleModeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		SelectFire(uid, component, args.Mode, args.Performer);
	}

	private void OnGunSelected(EntityUid uid, GunComponent component, HandSelectedEvent args)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.ApplyingState || component.FireRateModified <= 0f)
		{
			return;
		}
		float fireDelay = 1f / component.FireRateModified;
		if (!fireDelay.Equals(0f) && component.ResetOnHandSelected && !((EntitySystem)this).Paused(uid, (MetaDataComponent)null))
		{
			TimeSpan minimum = Timing.CurTime + TimeSpan.FromSeconds(fireDelay);
			if (!(minimum < component.NextFire))
			{
				component.NextFire = minimum;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void InitializeMagazine()
	{
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, MapInitEvent>((EntityEventRefHandler<MagazineAmmoProviderComponent, MapInitEvent>)OnMagazineMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<MagazineAmmoProviderComponent, TakeAmmoEvent>)OnMagazineTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<MagazineAmmoProviderComponent, GetAmmoCountEvent>)OnMagazineAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>)OnMagazineVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>)OnMagazineSlotChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>)OnMagazineSlotChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, UseInHandEvent>((ComponentEventHandler<MagazineAmmoProviderComponent, UseInHandEvent>)OnMagazineUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, ExaminedEvent>((ComponentEventHandler<MagazineAmmoProviderComponent, ExaminedEvent>)OnMagazineExamine, (Type[])null, (Type[])null);
	}

	private void OnMagazineMapInit(Entity<MagazineAmmoProviderComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent)))
		{
			MagazineSlotChanged(ent);
		}
	}

	private void OnMagazineExamine(EntityUid uid, MagazineAmmoProviderComponent component, ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid) && args.IsInDetailsRange)
		{
			int count = GetMagazineCountCapacity(uid, component).Item1;
			args.PushMarkup(base.Loc.GetString("gun-magazine-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("count", count)));
		}
	}

	private void OnMagazineUse(EntityUid uid, MagazineAmmoProviderComponent component, UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid))
		{
			EntityUid? magEnt = GetMagazineEntity(uid);
			if (magEnt.HasValue)
			{
				((EntitySystem)this).RaiseLocalEvent<UseInHandEvent>(magEnt.Value, args, false);
				UpdateAmmoCount(uid);
				UpdateMagazineAppearance(uid, component, magEnt.Value);
			}
		}
	}

	private void OnMagazineVerb(EntityUid uid, MagazineAmmoProviderComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid) && args.CanInteract && args.CanAccess)
		{
			EntityUid? magEnt = GetMagazineEntity(uid);
			if (magEnt.HasValue)
			{
				((EntitySystem)this).RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(magEnt.Value, args, false);
				UpdateMagazineAppearance(magEnt.Value, component, magEnt.Value);
			}
		}
	}

	protected virtual void OnMagazineSlotChange(EntityUid uid, MagazineAmmoProviderComponent component, ContainerModifiedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid) && !("gun_magazine" != args.Container.ID))
		{
			MagazineSlotChanged(Entity<MagazineAmmoProviderComponent>.op_Implicit((uid, component)));
		}
	}

	private void MagazineSlotChanged(Entity<MagazineAmmoProviderComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		UpdateAmmoCount(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent));
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent), ref appearance))
		{
			EntityUid? magEnt = GetMagazineEntity(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent));
			Appearance.SetData(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent), (Enum)AmmoVisuals.MagLoaded, (object)magEnt.HasValue, appearance);
			if (magEnt.HasValue)
			{
				UpdateMagazineAppearance(Entity<MagazineAmmoProviderComponent>.op_Implicit(ent), Entity<MagazineAmmoProviderComponent>.op_Implicit(ent), magEnt.Value);
			}
		}
	}

	protected (int, int) GetMagazineCountCapacity(EntityUid uid, MagazineAmmoProviderComponent component)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		int capacity = 1;
		EntityUid? magEnt = GetMagazineEntity(uid);
		if (magEnt.HasValue)
		{
			GetAmmoCountEvent ev = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magEnt.Value, ref ev, false);
			count += ev.Count;
			capacity += ev.Capacity;
		}
		return (count, capacity);
	}

	protected EntityUid? GetMagazineEntity(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainer(uid, "gun_magazine", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null)
			{
				return slot.ContainedEntity;
			}
		}
		return null;
	}

	private void OnMagazineAmmoCount(EntityUid uid, MagazineAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid))
		{
			EntityUid? magEntity = GetMagazineEntity(uid);
			if (magEntity.HasValue)
			{
				((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magEntity.Value, ref args, false);
			}
		}
	}

	private void OnMagazineTakeAmmo(EntityUid uid, MagazineAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!UsesPubgStoredMagazineAmmo(uid))
		{
			EntityUid? magEntity = GetMagazineEntity(uid);
			AppearanceComponent appearance = default(AppearanceComponent);
			((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance);
			if (!magEntity.HasValue)
			{
				Appearance.SetData(uid, (Enum)AmmoVisuals.MagLoaded, (object)false, appearance);
				return;
			}
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(magEntity.Value, args, false);
			GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magEntity.Value, ref ammoEv, false);
			FinaliseMagazineTakeAmmo(uid, component, ammoEv.Count, ammoEv.Capacity, args.User, appearance);
		}
	}

	private void FinaliseMagazineTakeAmmo(EntityUid uid, MagazineAmmoProviderComponent component, int count, int capacity, EntityUid? user, AppearanceComponent? appearance)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		bool ejectMag = component.AutoEject && count == 0;
		ActorComponent actor = default(ActorComponent);
		if (ejectMag && ((EntitySystem)this).TryComp<ActorComponent>(user, ref actor))
		{
			ejectMag = _netConfig.GetClientCVar<bool>(actor.PlayerSession.Channel, RMCCVars.RMCAutoEjectMagazines);
		}
		if (ejectMag)
		{
			EjectMagazine(uid, component, user);
			Audio.PlayPredicted(component.SoundAutoEject, uid, user, (AudioParams?)null);
		}
		UpdateMagazineAppearance(uid, appearance, !ejectMag, count, capacity);
	}

	private void UpdateMagazineAppearance(EntityUid uid, MagazineAmmoProviderComponent component, EntityUid magEnt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance);
		int count = 0;
		int capacity = 0;
		AppearanceComponent magAppearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(magEnt, ref magAppearance))
		{
			int addCount = default(int);
			Appearance.TryGetData<int>(magEnt, (Enum)AmmoVisuals.AmmoCount, ref addCount, magAppearance);
			int addCapacity = default(int);
			Appearance.TryGetData<int>(magEnt, (Enum)AmmoVisuals.AmmoMax, ref addCapacity, magAppearance);
			count += addCount;
			capacity += addCapacity;
		}
		UpdateMagazineAppearance(uid, appearance, magLoaded: true, count, capacity);
	}

	private void UpdateMagazineAppearance(EntityUid uid, AppearanceComponent? appearance, bool magLoaded, int count, int capacity)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (appearance != null)
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.MagLoaded, (object)magLoaded, appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.HasAmmo, (object)(count != 0), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)count, appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)capacity, appearance);
		}
	}

	private void EjectMagazine(EntityUid uid, MagazineAmmoProviderComponent component, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (GetMagazineEntity(uid).HasValue)
		{
			_slots.TryEject(uid, "gun_magazine", user, out var _, null, excludeUserAudio: true);
		}
	}

	private bool UsesPubgStoredMagazineAmmo(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		PubgAmmoProviderComponent pubgAmmoProviderComponent = default(PubgAmmoProviderComponent);
		PubgWeaponModulesComponent modules = default(PubgWeaponModulesComponent);
		if (!((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(uid, ref pubgAmmoProviderComponent) || !((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(uid, ref modules))
		{
			return false;
		}
		foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
		{
			if (slot.Slot == PubgModuleSlotType.Magazine)
			{
				return slot.StoresAmmo;
			}
		}
		return false;
	}

	protected virtual void InitializeRevolver()
	{
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentGetState>((ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentGetState>)OnRevolverGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentHandleState>((ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentHandleState>)OnRevolverHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentInit>((ComponentEventHandler<RevolverAmmoProviderComponent, ComponentInit>)OnRevolverInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<RevolverAmmoProviderComponent, TakeAmmoEvent>)OnRevolverTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>)OnRevolverVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, InteractUsingEvent>((ComponentEventHandler<RevolverAmmoProviderComponent, InteractUsingEvent>)OnRevolverInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<RevolverAmmoProviderComponent, GetAmmoCountEvent>)OnRevolverGetAmmoCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, UseInHandEvent>((ComponentEventHandler<RevolverAmmoProviderComponent, UseInHandEvent>)OnRevolverUse, (Type[])null, (Type[])null);
	}

	private void OnRevolverUse(EntityUid uid, RevolverAmmoProviderComponent component, UseInHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _useDelay.TryResetDelay(uid))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Cycle(component);
			UpdateAmmoCount(uid, prediction: false);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnRevolverGetAmmoCount(EntityUid uid, RevolverAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		args.Count += GetRevolverCount(component);
		args.Capacity += component.Capacity;
	}

	private void OnRevolverInteractUsing(EntityUid uid, RevolverAmmoProviderComponent component, InteractUsingEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && TryRevolverInsert(uid, component, args.Used, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnRevolverGetState(EntityUid uid, RevolverAmmoProviderComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new RevolverAmmoProviderComponentState
		{
			CurrentIndex = component.CurrentIndex,
			AmmoSlots = ((EntitySystem)this).GetNetEntityList(component.AmmoSlots),
			Chambers = component.Chambers
		};
	}

	private void OnRevolverHandleState(EntityUid uid, RevolverAmmoProviderComponent component, ref ComponentHandleState args)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is RevolverAmmoProviderComponentState state)
		{
			int oldIndex = component.CurrentIndex;
			component.CurrentIndex = state.CurrentIndex;
			component.Chambers = new bool?[state.Chambers.Length];
			for (int i = 0; i < component.AmmoSlots.Count; i++)
			{
				component.AmmoSlots[i] = ((EntitySystem)this).EnsureEntity<RevolverAmmoProviderComponent>(state.AmmoSlots[i], uid);
				component.Chambers[i] = state.Chambers[i];
			}
			if (oldIndex != state.CurrentIndex)
			{
				UpdateAmmoCount(uid, prediction: false);
			}
		}
	}

	public bool TryRevolverInsert(EntityUid revolverUid, RevolverAmmoProviderComponent component, EntityUid uid, EntityUid? user)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsWhitelistFail(component.Whitelist, uid))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<SpeedLoaderComponent>(uid))
		{
			int freeSlots = 0;
			for (int i = 0; i < component.Capacity; i++)
			{
				if (!component.AmmoSlots[i].HasValue && !component.Chambers[i].HasValue)
				{
					freeSlots++;
				}
			}
			if (freeSlots == 0)
			{
				Popup(base.Loc.GetString("gun-revolver-full"), revolverUid, user);
				return false;
			}
			TransformComponent xform = ((EntitySystem)this).GetEntityQuery<TransformComponent>().GetComponent(uid);
			List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>(freeSlots);
			TakeAmmoEvent ev = new TakeAmmoEvent(freeSlots, ammo, xform.Coordinates, user);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(uid, ev, false);
			if (ev.Ammo.Count == 0)
			{
				Popup(base.Loc.GetString("gun-speedloader-empty"), revolverUid, user);
				return false;
			}
			for (int j = 0; j < component.Capacity; j++)
			{
				int index = (component.CurrentIndex + j) % component.Capacity;
				if (component.AmmoSlots[index].HasValue || component.Chambers[index].HasValue)
				{
					continue;
				}
				EntityUid? ent = ev.Ammo.Last().Entity;
				ev.Ammo.RemoveAt(ev.Ammo.Count - 1);
				if (!ent.HasValue)
				{
					((EntitySystem)this).Log.Error("Tried to load hitscan into a revolver which is unsupported");
					continue;
				}
				component.AmmoSlots[index] = ent.Value;
				Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent.Value), (BaseContainer)(object)component.AmmoContainer, (TransformComponent)null, false);
				SetChamber(index, component, uid);
				if (ev.Ammo.Count == 0)
				{
					break;
				}
			}
			UpdateRevolverAppearance(revolverUid, component);
			UpdateAmmoCount(revolverUid);
			((EntitySystem)this).Dirty(revolverUid, (IComponent)(object)component, (MetaDataComponent)null);
			Audio.PlayPredicted(component.SoundInsert, revolverUid, user, (AudioParams?)null);
			Popup(base.Loc.GetString("gun-revolver-insert"), revolverUid, user);
			return true;
		}
		for (int k = 0; k < component.Capacity; k++)
		{
			int index2 = (component.CurrentIndex + k) % component.Capacity;
			if (!component.AmmoSlots[index2].HasValue && !component.Chambers[index2].HasValue)
			{
				component.AmmoSlots[index2] = uid;
				Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer)(object)component.AmmoContainer, (TransformComponent)null, false);
				SetChamber(index2, component, uid);
				Audio.PlayPredicted(component.SoundInsert, revolverUid, user, (AudioParams?)null);
				Popup(base.Loc.GetString("gun-revolver-insert"), revolverUid, user);
				UpdateRevolverAppearance(revolverUid, component);
				UpdateAmmoCount(revolverUid);
				((EntitySystem)this).Dirty(revolverUid, (IComponent)(object)component, (MetaDataComponent)null);
				return true;
			}
		}
		Popup(base.Loc.GetString("gun-revolver-full"), revolverUid, user);
		return false;
	}

	private void SetChamber(int index, RevolverAmmoProviderComponent component, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		if (((EntitySystem)this).TryComp<CartridgeAmmoComponent>(uid, ref cartridge) && cartridge.Spent)
		{
			component.Chambers[index] = false;
		}
		else
		{
			component.Chambers[index] = true;
		}
	}

	private void OnRevolverVerbs(EntityUid uid, RevolverAmmoProviderComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("gun-revolver-empty"),
				Disabled = !AnyRevolverCartridges(component),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					EmptyRevolver(uid, component, args.User);
				},
				Priority = 1
			});
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("gun-revolver-spin"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					SpinRevolver(uid, component, args.User);
				}
			});
		}
	}

	private bool AnyRevolverCartridges(RevolverAmmoProviderComponent component)
	{
		for (int i = 0; i < component.Capacity; i++)
		{
			if (component.Chambers[i].HasValue || component.AmmoSlots[i].HasValue)
			{
				return true;
			}
		}
		return false;
	}

	private int GetRevolverCount(RevolverAmmoProviderComponent component)
	{
		int count = 0;
		for (int i = 0; i < component.Capacity; i++)
		{
			if (component.Chambers[i].HasValue || component.AmmoSlots[i].HasValue)
			{
				count++;
			}
		}
		return count;
	}

	private int GetRevolverUnspentCount(RevolverAmmoProviderComponent component)
	{
		int count = 0;
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		for (int i = 0; i < component.Capacity; i++)
		{
			bool? chamber = component.Chambers[i];
			if (chamber == true)
			{
				count++;
				continue;
			}
			EntityUid? ammo = component.AmmoSlots[i];
			if (((EntitySystem)this).TryComp<CartridgeAmmoComponent>(ammo, ref cartridge) && !cartridge.Spent)
			{
				count++;
			}
		}
		return count;
	}

	public void EmptyRevolver(EntityUid revolverUid, RevolverAmmoProviderComponent component, EntityUid? user = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates mapCoordinates = TransformSystem.GetMapCoordinates(revolverUid, (TransformComponent)null);
		bool anyEmpty = false;
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		for (int i = 0; i < component.Capacity; i++)
		{
			bool? chamber = component.Chambers[i];
			EntityUid? slot = component.AmmoSlots[i];
			if (!slot.HasValue)
			{
				if (!chamber.HasValue)
				{
					continue;
				}
				if (!_netManager.IsClient)
				{
					EntityUid uid = ((EntitySystem)this).Spawn(component.FillPrototype, mapCoordinates, (ComponentRegistry)null, default(Angle));
					if (((EntitySystem)this).TryComp<CartridgeAmmoComponent>(uid, ref cartridge))
					{
						SetCartridgeSpent(uid, cartridge, !chamber.Value);
					}
					EjectCartridge(uid);
				}
				component.Chambers[i] = null;
				anyEmpty = true;
			}
			else
			{
				component.AmmoSlots[i] = null;
				Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(slot.Value), (BaseContainer)(object)component.AmmoContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
				component.Chambers[i] = null;
				if (!_netManager.IsClient)
				{
					EjectCartridge(slot.Value);
				}
				anyEmpty = true;
			}
		}
		if (anyEmpty)
		{
			Audio.PlayPredicted(component.SoundEject, revolverUid, user, (AudioParams?)null);
			UpdateAmmoCount(revolverUid, prediction: false);
			UpdateRevolverAppearance(revolverUid, component);
			((EntitySystem)this).Dirty(revolverUid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void UpdateRevolverAppearance(EntityUid uid, RevolverAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			int count = GetRevolverCount(component);
			Appearance.SetData(uid, (Enum)AmmoVisuals.HasAmmo, (object)(count != 0), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)count, appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)component.Capacity, appearance);
		}
	}

	protected virtual void SpinRevolver(EntityUid revolverUid, RevolverAmmoProviderComponent component, EntityUid? user = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Audio.PlayPredicted(component.SoundSpin, revolverUid, user, (AudioParams?)null);
		Popup(base.Loc.GetString("gun-revolver-spun"), revolverUid, user);
	}

	private void OnRevolverTakeAmmo(EntityUid uid, RevolverAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		int currentIndex = component.CurrentIndex;
		Cycle(component, args.Shots);
		CartridgeAmmoComponent cartridge = default(CartridgeAmmoComponent);
		for (int i = 0; i < args.Shots; i++)
		{
			int index = (currentIndex + i) % component.Capacity;
			bool? chamber = component.Chambers[index];
			EntityUid? ent = null;
			if (component.AmmoSlots[index].HasValue)
			{
				ent = component.AmmoSlots[index];
				component.Chambers[index] = false;
			}
			else if (chamber ?? false)
			{
				ent = ((EntitySystem)this).Spawn(component.FillPrototype, args.Coordinates);
				if (!_netManager.IsClient)
				{
					component.AmmoSlots[index] = ent;
					Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent.Value), (BaseContainer)(object)component.AmmoContainer, (TransformComponent)null, false);
				}
				component.Chambers[index] = false;
			}
			if (!ent.HasValue)
			{
				continue;
			}
			if (((EntitySystem)this).TryComp<CartridgeAmmoComponent>(ent, ref cartridge))
			{
				if (cartridge.Spent)
				{
					continue;
				}
				SetCartridgeSpent(ent.Value, cartridge, spent: true);
				EntityUid spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(cartridge.Prototype), args.Coordinates);
				args.Ammo.Add((spawned, ((EntitySystem)this).EnsureComp<AmmoComponent>(spawned)));
				if (cartridge.DeleteOnSpawn)
				{
					component.AmmoSlots[index] = null;
					component.Chambers[index] = null;
				}
			}
			else
			{
				component.AmmoSlots[index] = null;
				component.Chambers[index] = null;
				args.Ammo.Add((ent.Value, ((EntitySystem)this).EnsureComp<AmmoComponent>(ent.Value)));
			}
			if (_netManager.IsClient && ((EntitySystem)this).IsClientSide(ent.Value, (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel(ent);
			}
		}
		UpdateAmmoCount(uid, prediction: false);
		UpdateRevolverAppearance(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void Cycle(RevolverAmmoProviderComponent component, int count = 1)
	{
		component.CurrentIndex = (component.CurrentIndex + count) % component.Capacity;
	}

	private void OnRevolverInit(EntityUid uid, RevolverAmmoProviderComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.AmmoContainer = Containers.EnsureContainer<Container>(uid, "revolver-ammo", (ContainerManagerComponent)null);
		component.AmmoSlots.EnsureCapacity(component.Capacity);
		int remainder = component.Capacity - component.AmmoSlots.Count;
		for (int i = 0; i < remainder; i++)
		{
			component.AmmoSlots.Add(null);
		}
		component.Chambers = new bool?[component.Capacity];
		if (component.FillPrototype == null)
		{
			return;
		}
		for (int j = 0; j < component.Capacity; j++)
		{
			if (component.AmmoSlots[j].HasValue)
			{
				component.Chambers[j] = null;
			}
			else
			{
				component.Chambers[j] = true;
			}
		}
	}

	protected virtual void InitializeSolution()
	{
		((EntitySystem)this).SubscribeLocalEvent<SolutionAmmoProviderComponent, TakeAmmoEvent>((ComponentEventHandler<SolutionAmmoProviderComponent, TakeAmmoEvent>)OnSolutionTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionAmmoProviderComponent, GetAmmoCountEvent>((ComponentEventRefHandler<SolutionAmmoProviderComponent, GetAmmoCountEvent>)OnSolutionAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnSolutionTakeAmmo(EntityUid uid, SolutionAmmoProviderComponent component, TakeAmmoEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		int shots = Math.Min(args.Shots, component.Shots);
		if (shots != 0)
		{
			for (int i = 0; i < shots; i++)
			{
				List<(EntityUid? Entity, IShootable Shootable)> ammo = args.Ammo;
				(EntityUid, IShootable) solutionShot = GetSolutionShot(uid, component, args.Coordinates);
				ammo.Add((solutionShot.Item1, solutionShot.Item2));
				component.Shots--;
			}
			UpdateSolutionShots(uid, component);
			UpdateSolutionAppearance(uid, component);
		}
	}

	private void OnSolutionAmmoCount(EntityUid uid, SolutionAmmoProviderComponent component, ref GetAmmoCountEvent args)
	{
		args.Count = component.Shots;
		args.Capacity = component.MaxShots;
	}

	protected virtual void UpdateSolutionShots(EntityUid uid, SolutionAmmoProviderComponent component, Solution? solution = null)
	{
	}

	protected virtual (EntityUid Entity, IShootable) GetSolutionShot(EntityUid uid, SolutionAmmoProviderComponent component, EntityCoordinates position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		EntityUid ent = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(component.Prototype), position);
		return (Entity: ent, EnsureShootable(ent));
	}

	protected void UpdateSolutionAppearance(EntityUid uid, SolutionAmmoProviderComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)AmmoVisuals.HasAmmo, (object)(component.Shots != 0), appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoCount, (object)component.Shots, appearance);
			Appearance.SetData(uid, (Enum)AmmoVisuals.AmmoMax, (object)component.MaxShots, appearance);
		}
	}
}
