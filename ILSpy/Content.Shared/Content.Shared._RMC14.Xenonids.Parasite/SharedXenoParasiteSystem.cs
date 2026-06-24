using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.NPC;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.EggMorpher;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Parasite;

public abstract class SharedXenoParasiteSystem : EntitySystem
{
	[Dependency]
	private SharedRMCNPCSystem _rmcNpc;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private const string RipOffOnInfectionTag = "RipOffOnInfection";

	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	[Dependency]
	private SharedRMCSpriteSystem _rmcSprite;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private SharedRottingSystem _rotting;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	private const CollisionGroup LeapCollisionGroup = CollisionGroup.InteractImpassable;

	private const CollisionGroup ThrownCollisionGroup = CollisionGroup.InteractImpassable | CollisionGroup.BarricadeImpassable;

	protected readonly ProtoId<TagPrototype> ParasiteIsPreparingLeapProtoID = new ProtoId<TagPrototype>("RMCXenoParasitePreparingLeap");

	public void IntializeAI()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, PlayerAttachedEvent>((EntityEventRefHandler<XenoParasiteComponent, PlayerAttachedEvent>)OnPlayerAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, PlayerDetachedEvent>((EntityEventRefHandler<XenoParasiteComponent, PlayerDetachedEvent>)OnPlayerRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIDelayAddComponent, ComponentStartup>((EntityEventRefHandler<ParasiteAIDelayAddComponent, ComponentStartup>)OnAIDelayAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIComponent, MapInitEvent>((EntityEventRefHandler<ParasiteAIComponent, MapInitEvent>)OnAIAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIComponent, ExaminedEvent>((EntityEventRefHandler<ParasiteAIComponent, ExaminedEvent>)OnAIExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIComponent, DroppedEvent>((EntityEventRefHandler<ParasiteAIComponent, DroppedEvent>)OnAIDropPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<ParasiteAIComponent, EntGotInsertedIntoContainerMessage>)OnAIDropPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteAIComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<ParasiteAIComponent, GetVerbsEvent<ActivationVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrapParasiteComponent, ComponentStartup>((EntityEventRefHandler<TrapParasiteComponent, ComponentStartup>)OnTrapAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrapParasiteComponent, PlayerAttachedEvent>((EntityEventRefHandler<TrapParasiteComponent, PlayerAttachedEvent>)OnStopTrap, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrapParasiteComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<TrapParasiteComponent, EntGotInsertedIntoContainerMessage>)OnStopTrap, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrapParasiteComponent, XenoLeapHitEvent>((EntityEventRefHandler<TrapParasiteComponent, XenoLeapHitEvent>)OnLeapEndTrap, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrapParasiteComponent, ComponentShutdown>((EntityEventRefHandler<TrapParasiteComponent, ComponentShutdown>)OnTrapEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteTiredOutComponent, MapInitEvent>((EntityEventRefHandler<ParasiteTiredOutComponent, MapInitEvent>)OnParasiteAIMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteTiredOutComponent, UpdateMobStateEvent>((EntityEventRefHandler<ParasiteTiredOutComponent, UpdateMobStateEvent>)OnParasiteAIUpdateMobState, (Type[])null, new Type[2]
		{
			typeof(MobThresholdSystem),
			typeof(SharedXenoPheromonesSystem)
		});
	}

	private void OnTrapAdded(Entity<TrapParasiteComponent> para, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		para.Comp.LeapAt = _timing.CurTime + para.Comp.JumpTime;
		para.Comp.DisableAt = para.Comp.LeapAt + para.Comp.DisableTime;
		XenoLeapComponent leap = default(XenoLeapComponent);
		if (((EntitySystem)this).TryComp<XenoLeapComponent>(Entity<TrapParasiteComponent>.op_Implicit(para), ref leap))
		{
			para.Comp.NormalLeapDelay = leap.Delay;
			leap.Delay = TimeSpan.Zero;
			_rmcNpc.SleepNPC(Entity<TrapParasiteComponent>.op_Implicit(para));
		}
	}

	private void OnStopTrap<T>(Entity<TrapParasiteComponent> para, ref T args) where T : EntityEventArgs
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<TrapParasiteComponent>(Entity<TrapParasiteComponent>.op_Implicit(para));
	}

	private void OnTrapEnd(Entity<TrapParasiteComponent> para, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		XenoLeapComponent leap = default(XenoLeapComponent);
		if (((EntitySystem)this).TryComp<XenoLeapComponent>(Entity<TrapParasiteComponent>.op_Implicit(para), ref leap))
		{
			leap.Delay = para.Comp.NormalLeapDelay;
			_rmcNpc.SleepNPC(Entity<TrapParasiteComponent>.op_Implicit(para));
		}
	}

	private void OnLeapEndTrap(Entity<TrapParasiteComponent> para, ref XenoLeapHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ResetTrapState(para);
	}

	public void ResetTrapState(Entity<TrapParasiteComponent> para)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		XenoLeapComponent leap = default(XenoLeapComponent);
		if (((EntitySystem)this).TryComp<XenoLeapComponent>(Entity<TrapParasiteComponent>.op_Implicit(para), ref leap))
		{
			ParasiteAIComponent leapAI = default(ParasiteAIComponent);
			if (((EntitySystem)this).TryComp<ParasiteAIComponent>(Entity<TrapParasiteComponent>.op_Implicit(para), ref leapAI))
			{
				_rmcNpc.SleepNPC(Entity<TrapParasiteComponent>.op_Implicit(para));
			}
			leap.Delay = para.Comp.NormalLeapDelay;
			((EntitySystem)this).RemCompDeferred<TrapParasiteComponent>(Entity<TrapParasiteComponent>.op_Implicit(para));
		}
	}

	private void OnPlayerAdded(Entity<XenoParasiteComponent> para, ref PlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(para));
		((EntitySystem)this).RemCompDeferred<ParasiteAIDelayAddComponent>(Entity<XenoParasiteComponent>.op_Implicit(para));
	}

	private void OnPlayerRemoved(Entity<XenoParasiteComponent> para, ref PlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoParasiteComponent>.op_Implicit(para), (MetaDataComponent)null))
		{
			((EntitySystem)this).EnsureComp<ParasiteAIDelayAddComponent>(Entity<XenoParasiteComponent>.op_Implicit(para));
		}
	}

	private void OnAIDelayAdded(Entity<ParasiteAIDelayAddComponent> para, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		para.Comp.TimeToAI = _timing.CurTime + para.Comp.DelayTime;
		_rmcNpc.SleepNPC(Entity<ParasiteAIDelayAddComponent>.op_Implicit(para));
	}

	private void OnAIAdded(Entity<ParasiteAIComponent> para, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(Entity<ParasiteAIComponent>.op_Implicit(para)))
		{
			HandleDeathTimer(para);
			GoActive(para);
		}
	}

	private void OnAIExamined(Entity<ParasiteAIComponent> para, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(Entity<ParasiteAIComponent>.op_Implicit(para)) && ((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			switch (para.Comp.Mode)
			{
			case ParasiteMode.Idle:
				args.PushMarkup(base.Loc.GetString("rmc-xeno-parasite-ai-idle", (ValueTuple<string, object>)("parasite", para)) ?? "");
				break;
			case ParasiteMode.Active:
				args.PushMarkup(base.Loc.GetString("rmc-xeno-parasite-ai-active", (ValueTuple<string, object>)("parasite", para)) ?? "");
				break;
			case ParasiteMode.Dying:
				args.PushMarkup(base.Loc.GetString("rmc-xeno-parasite-ai-dying", (ValueTuple<string, object>)("parasite", para)) ?? "");
				break;
			}
		}
	}

	private void OnAIDropPickup<T>(Entity<ParasiteAIComponent> para, ref T args) where T : EntityEventArgs
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HandleDeathTimer(para);
		GoIdle(para);
	}

	public void HandleDeathTimer(Entity<ParasiteAIComponent> para)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer carry = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ParasiteAIComponent>.op_Implicit(para), null, null)), ref carry) && ((EntitySystem)this).HasComp<XenoNurturingComponent>(carry.Owner))
		{
			para.Comp.DeathTime = null;
			if (para.Comp.Mode == ParasiteMode.Dying)
			{
				para.Comp.Mode = ParasiteMode.Active;
				GoIdle(para);
			}
		}
		else if (!para.Comp.DeathTime.HasValue)
		{
			para.Comp.DeathTime = _timing.CurTime + para.Comp.LifeTime;
		}
	}

	public void UpdateAI(Entity<ParasiteAIComponent> para, TimeSpan currentTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		CheckCannibalize(para);
		if (para.Comp.DeathTime.HasValue)
		{
			TimeSpan value = currentTime;
			TimeSpan? deathTime = para.Comp.DeathTime;
			if (value > deathTime)
			{
				goto IL_0050;
			}
		}
		if (para.Comp.JumpsLeft > 0)
		{
			if (para.Comp.Mode == ParasiteMode.Active)
			{
				TimeSpan value = currentTime;
				TimeSpan? deathTime = para.Comp.NextJump;
				if (deathTime.HasValue && value >= deathTime.GetValueOrDefault() && !_container.IsEntityInContainer(Entity<ParasiteAIComponent>.op_Implicit(para), (MetaDataComponent)null))
				{
					if (!((EntitySystem)this).HasComp<StunnedComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
					{
						((EntitySystem)this).EnsureComp<TrapParasiteComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)).JumpTime = TimeSpan.Zero;
					}
					para.Comp.NextJump = currentTime + para.Comp.JumpTime;
				}
			}
			if (para.Comp.Mode == ParasiteMode.Idle && currentTime > para.Comp.NextActiveTime)
			{
				GoActive(para);
			}
			return;
		}
		goto IL_0050;
		IL_0050:
		if (para.Comp.Mode != ParasiteMode.Dying)
		{
			para.Comp.Mode = ParasiteMode.Dying;
			if (((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
			{
				DoRestAction(para);
			}
			ChangeHTN(Entity<ParasiteAIComponent>.op_Implicit(para), ParasiteMode.Dying);
			_rmcNpc.WakeNPC(Entity<ParasiteAIComponent>.op_Implicit(para));
			((EntitySystem)this).Dirty<ParasiteAIComponent>(para, (MetaDataComponent)null);
		}
		CheckDeath(para);
	}

	public void GoIdle(Entity<ParasiteAIComponent> para)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		para.Comp.JumpsLeft = para.Comp.InitialJumps;
		if (para.Comp.Mode == ParasiteMode.Active)
		{
			if (!((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
			{
				DoRestAction(para);
			}
			_rmcNpc.SleepNPC(Entity<ParasiteAIComponent>.op_Implicit(para));
			para.Comp.Mode = ParasiteMode.Idle;
			if (((EntitySystem)this).HasComp<TrapParasiteComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
			{
				((EntitySystem)this).RemCompDeferred<TrapParasiteComponent>(Entity<ParasiteAIComponent>.op_Implicit(para));
			}
			para.Comp.NextActiveTime = _timing.CurTime + TimeSpan.FromSeconds(_random.Next(para.Comp.MinIdleTime, para.Comp.MaxIdleTime + 1));
			((EntitySystem)this).Dirty<ParasiteAIComponent>(para, (MetaDataComponent)null);
		}
	}

	public void GoActive(Entity<ParasiteAIComponent> para)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (para.Comp.Mode != ParasiteMode.Dying)
		{
			if (((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
			{
				DoRestAction(para);
			}
			ChangeHTN(Entity<ParasiteAIComponent>.op_Implicit(para), ParasiteMode.Active);
			para.Comp.Mode = ParasiteMode.Active;
			_rmcNpc.SleepNPC(Entity<ParasiteAIComponent>.op_Implicit(para));
			if (((EntitySystem)this).HasComp<TrapParasiteComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
			{
				((EntitySystem)this).RemCompDeferred<TrapParasiteComponent>(Entity<ParasiteAIComponent>.op_Implicit(para));
			}
			para.Comp.NextJump = _timing.CurTime + para.Comp.JumpTime;
			((EntitySystem)this).Dirty<ParasiteAIComponent>(para, (MetaDataComponent)null);
		}
	}

	private void DoRestAction(Entity<ParasiteAIComponent> para)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		XenoComponent xeno = default(XenoComponent);
		ActionComponent actionComp = default(ActionComponent);
		if (((EntitySystem)this).TryComp<XenoComponent>(Entity<ParasiteAIComponent>.op_Implicit(para), ref xeno) && ((Component)xeno).Initialized && xeno.Actions.TryGetValue(EntProtoId.op_Implicit(para.Comp.RestAction), out var action) && ((EntitySystem)this).TryComp<ActionComponent>(action, ref actionComp))
		{
			BaseActionEvent actionEvent = _action.GetEvent(action);
			_action.PerformAction(Entity<ActionsComponent>.op_Implicit(para.Owner), Entity<ActionComponent>.op_Implicit((action, actionComp)), actionEvent);
		}
	}

	protected virtual void ChangeHTN(EntityUid parasite, ParasiteMode mode)
	{
	}

	private void CheckCannibalize(Entity<ParasiteAIComponent> para)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcHands.TryGetHolder(Entity<ParasiteAIComponent>.op_Implicit(para), out var user) || ((EntitySystem)this).HasComp<ThrownItemComponent>(Entity<ParasiteAIComponent>.op_Implicit(para)))
		{
			return;
		}
		int totalParasites = 0;
		foreach (Entity<ParasiteAIComponent> parasite in _entityLookup.GetEntitiesInRange<ParasiteAIComponent>(_transform.GetMapCoordinates(Entity<ParasiteAIComponent>.op_Implicit(para), (TransformComponent)null), para.Comp.CannibalizeCheck, (LookupFlags)110))
		{
			if (!(parasite == para) && !((EntitySystem)this).TerminatingOrDeleted(Entity<ParasiteAIComponent>.op_Implicit(parasite), (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(Entity<ParasiteAIComponent>.op_Implicit(parasite)) && !_mobState.IsDead(Entity<ParasiteAIComponent>.op_Implicit(parasite)) && parasite.Comp.Mode == ParasiteMode.Active && !_rmcHands.TryGetHolder(Entity<ParasiteAIComponent>.op_Implicit(parasite), out user) && !((EntitySystem)this).HasComp<ThrownItemComponent>(Entity<ParasiteAIComponent>.op_Implicit(parasite)) && !((EntitySystem)this).HasComp<StunnedComponent>(Entity<ParasiteAIComponent>.op_Implicit(parasite)))
			{
				totalParasites++;
			}
		}
		if (totalParasites > para.Comp.MaxSurroundingParas)
		{
			_popup.PopupCoordinates(base.Loc.GetString("rmc-xeno-parasite-ai-eaten", (ValueTuple<string, object>)("parasite", para)), _transform.GetMoverCoordinates(Entity<ParasiteAIComponent>.op_Implicit(para)), PopupType.SmallCaution);
			((EntitySystem)this).QueueDel((EntityUid?)Entity<ParasiteAIComponent>.op_Implicit(para));
		}
	}

	private void CheckDeath(Entity<ParasiteAIComponent> para)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<XenoEggComponent> item in _entityLookup.GetEntitiesInRange<XenoEggComponent>(_transform.GetMoverCoordinates(Entity<ParasiteAIComponent>.op_Implicit(para)), para.Comp.RangeCheck, (LookupFlags)110))
		{
			if (item.Comp.State == XenoEggState.Opened)
			{
				return;
			}
		}
		foreach (Entity<XenoResinHoleComponent> item2 in _entityLookup.GetEntitiesInRange<XenoResinHoleComponent>(_transform.GetMoverCoordinates(Entity<ParasiteAIComponent>.op_Implicit(para)), para.Comp.RangeCheck, (LookupFlags)110))
		{
			if (!item2.Comp.TrapPrototype.HasValue)
			{
				return;
			}
		}
		foreach (Entity<EggMorpherComponent> eggmorpher in _entityLookup.GetEntitiesInRange<EggMorpherComponent>(_transform.GetMoverCoordinates(Entity<ParasiteAIComponent>.op_Implicit(para)), para.Comp.RangeCheck, (LookupFlags)110))
		{
			if (eggmorpher.Comp.CurParasites < eggmorpher.Comp.MaxParasites)
			{
				return;
			}
		}
		((EntitySystem)this).EnsureComp<ParasiteTiredOutComponent>(Entity<ParasiteAIComponent>.op_Implicit(para));
	}

	private void OnParasiteAIMapInit(Entity<ParasiteTiredOutComponent> dead, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(Entity<ParasiteTiredOutComponent>.op_Implicit(dead), ref mobState))
		{
			_mobState.UpdateMobState(Entity<ParasiteTiredOutComponent>.op_Implicit(dead), mobState);
		}
	}

	private void OnParasiteAIUpdateMobState(Entity<ParasiteTiredOutComponent> dead, ref UpdateMobStateEvent args)
	{
		args.State = MobState.Dead;
	}

	private void OnGetVerbs(Entity<ParasiteAIComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = args.User;
		if (((EntitySystem)this).HasComp<ActorComponent>(uid) && ((EntitySystem)this).HasComp<GhostComponent>(uid) && _mobState.IsAlive(Entity<ParasiteAIComponent>.op_Implicit(ent)))
		{
			ActivationVerb parasiteVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("rmc-xeno-egg-ghost-verb"),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)XenoParasiteGhostUI.Key, uid, false);
				},
				Impact = LogImpact.High
			};
			args.Verbs.Add(parasiteVerb);
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InfectableComponent, ActivateInWorldEvent>((EntityEventRefHandler<InfectableComponent, ActivateInWorldEvent>)OnInfectableActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InfectableComponent, CanDropTargetEvent>((EntityEventRefHandler<InfectableComponent, CanDropTargetEvent>)OnInfectableCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, XenoLeapHitEvent>((EntityEventRefHandler<XenoParasiteComponent, XenoLeapHitEvent>)OnParasiteLeapHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, AfterInteractEvent>((EntityEventRefHandler<XenoParasiteComponent, AfterInteractEvent>)OnParasiteAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, BeforeInteractHandEvent>((EntityEventRefHandler<XenoParasiteComponent, BeforeInteractHandEvent>)OnParasiteInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, DoAfterAttemptEvent<AttachParasiteDoAfterEvent>>((EntityEventRefHandler<XenoParasiteComponent, DoAfterAttemptEvent<AttachParasiteDoAfterEvent>>)OnParasiteAttachDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, AttachParasiteDoAfterEvent>((EntityEventRefHandler<XenoParasiteComponent, AttachParasiteDoAfterEvent>)OnParasiteAttachDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, CanDragEvent>((EntityEventRefHandler<XenoParasiteComponent, CanDragEvent>)OnParasiteCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, CanDropDraggedEvent>((EntityEventRefHandler<XenoParasiteComponent, CanDropDraggedEvent>)OnParasiteCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, DragDropDraggedEvent>((EntityEventRefHandler<XenoParasiteComponent, DragDropDraggedEvent>)OnParasiteDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, ThrowItemAttemptEvent>((EntityEventRefHandler<XenoParasiteComponent, ThrowItemAttemptEvent>)OnParasiteThrowAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, PullAttemptEvent>((EntityEventRefHandler<XenoParasiteComponent, PullAttemptEvent>)OnParasiteTryPull, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<XenoParasiteComponent, GettingPickedUpAttemptEvent>)OnParasiteTryPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<XenoParasiteComponent, BeforeDamageChangedEvent>)OnParasiteBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, XenoLeapActionEvent>((EntityEventRefHandler<XenoParasiteComponent, XenoLeapActionEvent>)OnParasiteLeap, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, XenoLeapAttemptEvent>((EntityEventRefHandler<XenoParasiteComponent, XenoLeapAttemptEvent>)OnParasiteLeapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, XenoLeapDoAfterEvent>((EntityEventRefHandler<XenoParasiteComponent, XenoLeapDoAfterEvent>)OnParasiteLeapDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, XenoLeapStoppedEvent>((EntityEventRefHandler<XenoParasiteComponent, XenoLeapStoppedEvent>)OnParasiteLeapStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, ThrownEvent>((EntityEventRefHandler<XenoParasiteComponent, ThrownEvent>)OnParasiteThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, LandEvent>((EntityEventRefHandler<XenoParasiteComponent, LandEvent>)OnParasiteLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteSpentComponent, MapInitEvent>((EntityEventRefHandler<ParasiteSpentComponent, MapInitEvent>)OnParasiteSpentMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParasiteSpentComponent, UpdateMobStateEvent>((EntityEventRefHandler<ParasiteSpentComponent, UpdateMobStateEvent>)OnParasiteSpentUpdateMobState, (Type[])null, new Type[2]
		{
			typeof(MobThresholdSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<ParasiteSpentComponent, ExaminedEvent>((EntityEventRefHandler<ParasiteSpentComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimInfectedComponent, MapInitEvent>((EntityEventRefHandler<VictimInfectedComponent, MapInitEvent>)OnVictimInfectedMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimInfectedComponent, ComponentRemove>((EntityEventRefHandler<VictimInfectedComponent, ComponentRemove>)OnVictimInfectedRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimInfectedComponent, ExaminedEvent>((EntityEventRefHandler<VictimInfectedComponent, ExaminedEvent>)OnVictimInfectedExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimInfectedComponent, RejuvenateEvent>((EntityEventRefHandler<VictimInfectedComponent, RejuvenateEvent>)OnVictimInfectedRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimInfectedComponent, LarvaBurstDoAfterEvent>((EntityEventRefHandler<VictimInfectedComponent, LarvaBurstDoAfterEvent>)OnBurst, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimBurstComponent, MapInitEvent>((EntityEventRefHandler<VictimBurstComponent, MapInitEvent>)OnVictimBurstMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimBurstComponent, UpdateMobStateEvent>((EntityEventRefHandler<VictimBurstComponent, UpdateMobStateEvent>)OnVictimUpdateMobState, (Type[])null, new Type[2]
		{
			typeof(MobThresholdSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<VictimBurstComponent, RejuvenateEvent>((EntityEventRefHandler<VictimBurstComponent, RejuvenateEvent>)OnVictimBurstRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimBurstComponent, ExaminedEvent>((EntityEventRefHandler<VictimBurstComponent, ExaminedEvent>)OnVictimBurstExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BursterComponent, MoveInputEvent>((EntityEventRefHandler<BursterComponent, MoveInputEvent>)OnTryMove, (Type[])null, (Type[])null);
		IntializeAI();
	}

	private void OnInfectableActivate(Entity<InfectableComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		XenoParasiteComponent parasite = default(XenoParasiteComponent);
		if (((EntitySystem)this).TryComp<XenoParasiteComponent>(args.User, ref parasite) && StartInfect(Entity<XenoParasiteComponent>.op_Implicit((args.User, parasite)), args.Target, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnInfectableCanDropTarget(Entity<InfectableComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		XenoParasiteComponent parasite = default(XenoParasiteComponent);
		if (((EntitySystem)this).TryComp<XenoParasiteComponent>(args.Dragged, ref parasite) && CanInfectPopup(Entity<XenoParasiteComponent>.op_Implicit((args.Dragged, parasite)), Entity<InfectableComponent>.op_Implicit(ent), args.User, popup: false))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnParasiteLeapHit(Entity<XenoParasiteComponent> parasite, ref XenoLeapHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<XenoParasiteComponent>.op_Implicit(parasite));
		ParasiteAIComponent ai = default(ParasiteAIComponent);
		float range = (((EntitySystem)this).TryComp<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite), ref ai) ? ((float)ai.MaxInfectRange) : parasite.Comp.InfectRange);
		if (_transform.InRange(coordinates, args.Leaping.Origin, range))
		{
			Infect(parasite, args.Hit, popup: false);
		}
	}

	private void OnParasiteAfterInteract(Entity<XenoParasiteComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && args.Target.HasValue && !((HandledEntityEventArgs)args).Handled && _rmcHands.IsPickupByAllowed(Entity<WhitelistPickupByComponent>.op_Implicit(ent.Owner), Entity<WhitelistPickupComponent>.op_Implicit(args.User)) && StartInfect(ent, args.Target.Value, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnParasiteInteractHand(Entity<XenoParasiteComponent> ent, ref BeforeInteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (IsInfectable(ent, args.Target))
		{
			StartInfect(ent, args.Target, Entity<XenoParasiteComponent>.op_Implicit(ent));
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnParasiteAttachDoAfterAttempt(Entity<XenoParasiteComponent> ent, ref DoAfterAttemptEvent<AttachParasiteDoAfterEvent> args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.DoAfter.Args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (!CanInfectPopup(ent, target2, Entity<XenoParasiteComponent>.op_Implicit(ent)))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
		else
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnParasiteAttachDoAfter(Entity<XenoParasiteComponent> ent, ref AttachParasiteDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && args.Target.HasValue && Infect(ent, args.Target.Value))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnParasiteCanDrag(Entity<XenoParasiteComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnParasiteCanDropDragged(Entity<XenoParasiteComponent> ent, ref CanDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if ((!(args.User != ent.Owner) || _rmcHands.IsPickupByAllowed(Entity<WhitelistPickupByComponent>.op_Implicit(ent.Owner), Entity<WhitelistPickupComponent>.op_Implicit(args.User))) && CanInfectPopup(ent, args.Target, args.User, popup: false))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnParasiteDragDropDragged(Entity<XenoParasiteComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != ent.Owner) || _rmcHands.IsPickupByAllowed(Entity<WhitelistPickupByComponent>.op_Implicit(ent.Owner), Entity<WhitelistPickupComponent>.op_Implicit(args.User)))
		{
			StartInfect(ent, args.Target, args.User);
			args.Handled = true;
		}
	}

	private void OnParasiteThrowAttempt(Entity<XenoParasiteComponent> ent, ref ThrowItemAttemptEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			args.Cancelled = true;
			if (!_net.IsClient)
			{
				EntityUid user = args.User;
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-cant-throw", (ValueTuple<string, object>)("target", ent)), user, user, PopupType.SmallCaution);
			}
		}
	}

	private void OnParasiteTryPull(Entity<XenoParasiteComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent)) && !((EntitySystem)this).HasComp<InfectableComponent>(args.PullerUid))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-parasite-nonplayer-pull", (ValueTuple<string, object>)("parasite", ent)), Entity<XenoParasiteComponent>.op_Implicit(ent), args.PullerUid, PopupType.SmallCaution);
			args.Cancelled = true;
		}
	}

	private void OnParasiteTryPickup(Entity<XenoParasiteComponent> ent, ref GettingPickedUpAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-parasite-player-pickup", (ValueTuple<string, object>)("parasite", ent)), Entity<XenoParasiteComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (((EntitySystem)this).HasComp<OnFireComponent>(args.User))
		{
			_popup.PopupClient("Touching the parasite while you're on fire would burn it!", Entity<XenoParasiteComponent>.op_Implicit(ent), args.User, PopupType.MediumCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnParasiteBeforeDamageChanged(Entity<XenoParasiteComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.InfectedVictim.HasValue && !ent.Comp.FellOff)
		{
			args.Cancelled = true;
		}
	}

	private void OnParasiteLeap(Entity<XenoParasiteComponent> ent, ref XenoLeapActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		_tagSystem.AddTag(Entity<XenoParasiteComponent>.op_Implicit(ent), ParasiteIsPreparingLeapProtoID);
		_rmcSprite.UpdateDrawDepth(Entity<XenoParasiteComponent>.op_Implicit(ent));
		XenoHideComponent xenoHideComp = default(XenoHideComponent);
		if (!((EntitySystem)this).TryComp<XenoHideComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ref xenoHideComp) || !xenoHideComp.Hiding)
		{
			return;
		}
		XenoHideActionEvent ev = new XenoHideActionEvent();
		ev.Performer = Entity<XenoParasiteComponent>.op_Implicit(ent);
		ev.Toggle = false;
		((EntitySystem)this).RaiseLocalEvent<XenoHideActionEvent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ev, false);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoHideActionEvent>(Entity<XenoParasiteComponent>.op_Implicit(ent)))
		{
			_action.SetEnabled(action.AsNullable(), enabled: false);
		}
	}

	private void OnParasiteLeapAttempt(Entity<XenoParasiteComponent> ent, ref XenoLeapAttemptEvent args)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			_tagSystem.RemoveTag(Entity<XenoParasiteComponent>.op_Implicit(ent), ParasiteIsPreparingLeapProtoID);
			_rmcSprite.UpdateDrawDepth(Entity<XenoParasiteComponent>.op_Implicit(ent));
			{
				foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoHideActionEvent>(Entity<XenoParasiteComponent>.op_Implicit(ent)))
				{
					_action.SetEnabled(action.AsNullable(), enabled: false);
				}
				return;
			}
		}
		foreach (EntityUid contact in _physics.GetContactingEntities(Entity<XenoParasiteComponent>.op_Implicit(ent), (PhysicsComponent)null, false))
		{
			if (((EntitySystem)this).HasComp<DoorComponent>(contact) && !((EntitySystem)this).HasComp<ResinDoorComponent>(contact))
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-leap-blocked"), ((EntitySystem)this).Transform(Entity<XenoParasiteComponent>.op_Implicit(ent)).Coordinates, Entity<XenoParasiteComponent>.op_Implicit(ent));
				args.Cancelled = true;
				break;
			}
		}
	}

	private void OnParasiteLeapDoAfter(Entity<XenoParasiteComponent> ent, ref XenoLeapDoAfterEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		_tagSystem.RemoveTag(Entity<XenoParasiteComponent>.op_Implicit(ent), ParasiteIsPreparingLeapProtoID);
		_rmcSprite.UpdateDrawDepth(Entity<XenoParasiteComponent>.op_Implicit(ent));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoHideActionEvent>(Entity<XenoParasiteComponent>.op_Implicit(ent)))
		{
			_action.SetEnabled(action.AsNullable(), enabled: true);
		}
		if (args.Cancelled)
		{
			return;
		}
		HashSet<EntityUid> contactingEntities = _physics.GetContactingEntities(Entity<XenoParasiteComponent>.op_Implicit(ent), (PhysicsComponent)null, false);
		EntityUid? nearestResinDoor = null;
		float? nearestResinDoorDistance = null;
		float contactDistance = default(float);
		foreach (EntityUid contact in contactingEntities)
		{
			if (((EntitySystem)this).HasComp<DoorComponent>(contact) && ((EntitySystem)this).HasComp<ResinDoorComponent>(contact) && _physics.TryGetDistance(Entity<XenoParasiteComponent>.op_Implicit(ent), contact, ref contactDistance, (TransformComponent)null, (TransformComponent)null, (FixturesComponent)null, (FixturesComponent)null, (PhysicsComponent)null, (PhysicsComponent)null) && (!nearestResinDoorDistance.HasValue || nearestResinDoorDistance > contactDistance))
			{
				nearestResinDoor = contact;
				nearestResinDoorDistance = contactDistance;
			}
		}
		if (nearestResinDoor.HasValue)
		{
			PreventCollideComponent collisionPreventComp = new PreventCollideComponent();
			collisionPreventComp.Uid = nearestResinDoor.Value;
			((EntitySystem)this).AddComp<PreventCollideComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), collisionPreventComp, false);
		}
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ref fixtures))
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			_physics.SetCollisionMask(Entity<XenoParasiteComponent>.op_Implicit(ent), fixture.Key, fixture.Value, fixture.Value.CollisionMask | 0x80, (FixturesComponent)null, (PhysicsComponent)null);
		}
	}

	private void OnParasiteLeapStopped(Entity<XenoParasiteComponent> ent, ref XenoLeapStoppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<PreventCollideComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent));
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ref fixtures))
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			if ((fixture.Value.CollisionMask & 0xCD) != 0)
			{
				_physics.SetCollisionMask(Entity<XenoParasiteComponent>.op_Implicit(ent), fixture.Key, fixture.Value, fixture.Value.CollisionMask ^ 0x80, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	private void OnParasiteThrown(Entity<XenoParasiteComponent> ent, ref ThrownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ref fixtures))
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			_physics.SetCollisionMask(Entity<XenoParasiteComponent>.op_Implicit(ent), fixture.Key, fixture.Value, fixture.Value.CollisionMask | 0x4000080, (FixturesComponent)null, (PhysicsComponent)null);
		}
	}

	private void OnParasiteLand(Entity<XenoParasiteComponent> ent, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<XenoParasiteComponent>.op_Implicit(ent), ref fixtures))
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			if ((fixture.Value.CollisionMask & 0xCD & 0x4000000) == 0)
			{
				_physics.SetCollisionMask(Entity<XenoParasiteComponent>.op_Implicit(ent), fixture.Key, fixture.Value, fixture.Value.CollisionMask ^ 0x4000080, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	protected virtual void ParasiteLeapHit(Entity<XenoParasiteComponent> parasite)
	{
	}

	private void OnParasiteSpentMapInit(Entity<ParasiteSpentComponent> spent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(Entity<ParasiteSpentComponent>.op_Implicit(spent), ref mobState))
		{
			_mobState.UpdateMobState(Entity<ParasiteSpentComponent>.op_Implicit(spent), mobState);
		}
	}

	private void OnParasiteSpentUpdateMobState(Entity<ParasiteSpentComponent> spent, ref UpdateMobStateEvent args)
	{
		args.State = MobState.Dead;
	}

	private void OnExamined(Entity<ParasiteSpentComponent> spent, ref ExaminedEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup("[italic]" + base.Loc.GetString("rmc-xeno-parasite-dead", (ValueTuple<string, object>)("parasite", spent)) + "[/italic]");
	}

	private void OnVictimInfectedMapInit(Entity<VictimInfectedComponent> victim, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		victim.Comp.BurstAt = _timing.CurTime + victim.Comp.BurstDelay;
	}

	private void OnVictimInfectedRemoved(Entity<VictimInfectedComponent> victim, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_status.HasStatusEffect(Entity<VictimInfectedComponent>.op_Implicit(victim), "Unconscious"))
		{
			_status.TryRemoveStatusEffect(Entity<VictimInfectedComponent>.op_Implicit(victim), "Unconscious");
		}
		_standing.Stand(Entity<VictimInfectedComponent>.op_Implicit(victim));
	}

	private void OnVictimInfectedCancel<T>(Entity<VictimInfectedComponent> victim, ref T args) where T : CancellableEntityEventArgs
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((Component)victim.Comp).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
		}
	}

	private void OnVictimInfectedExamined(Entity<VictimInfectedComponent> victim, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup("This one is hosting a sister! She will emerge in time.");
		}
		else if (((EntitySystem)this).HasComp<GhostComponent>(args.Examiner))
		{
			args.PushMarkup("This creature is infected.");
		}
	}

	private void OnVictimInfectedRejuvenate(Entity<VictimInfectedComponent> victim, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<VictimInfectedComponent>(Entity<VictimInfectedComponent>.op_Implicit(victim));
	}

	private void OnVictimBurstMapInit(Entity<VictimBurstComponent> burst, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<VictimBurstComponent>.op_Implicit(burst), (Enum)BurstVisuals.Visuals, (object)VictimBurstState.Burst, (AppearanceComponent)null);
		_unrevivable.MakeUnrevivable(Entity<RMCRevivableComponent>.op_Implicit(burst.Owner));
	}

	private void OnVictimUpdateMobState(Entity<VictimBurstComponent> burst, ref UpdateMobStateEvent args)
	{
		args.State = MobState.Dead;
	}

	private void OnVictimBurstRejuvenate(Entity<VictimBurstComponent> burst, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<VictimBurstComponent>(Entity<VictimBurstComponent>.op_Implicit(burst));
	}

	private void OnVictimBurstExamine(Entity<VictimBurstComponent> burst, ref ExaminedEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("VictimBurstComponent"))
		{
			args.PushMarkup("[color=red][bold]" + base.Loc.GetString("rmc-xeno-infected-bursted", (ValueTuple<string, object>)("victim", burst)) + "[/bold][/color]");
		}
	}

	private bool StartInfect(Entity<XenoParasiteComponent> parasite, EntityUid victim, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInfectPopup(parasite, victim, user))
		{
			return false;
		}
		AttachParasiteDoAfterEvent ev = new AttachParasiteDoAfterEvent();
		TimeSpan delay = parasite.Comp.ManualAttachDelay;
		if (parasite.Owner == user)
		{
			delay = parasite.Comp.SelfAttachDelay;
		}
		if (((EntitySystem)this).HasComp<TrapParasiteComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite)))
		{
			delay = TimeSpan.Zero;
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<XenoParasiteComponent>.op_Implicit(parasite), victim)
		{
			BreakOnMove = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			AttemptFrequency = AttemptFrequency.EveryTick
		};
		_doAfter.TryStartDoAfter(doAfter);
		return true;
	}

	private bool IsInfectable(Entity<XenoParasiteComponent> parasite, EntityUid victim)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		InfectableComponent infected = default(InfectableComponent);
		if (((EntitySystem)this).TryComp<InfectableComponent>(victim, ref infected) && !parasite.Comp.InfectedVictim.HasValue && !infected.BeingInfected && !((EntitySystem)this).HasComp<ParasiteSpentComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite)))
		{
			return !((EntitySystem)this).HasComp<VictimInfectedComponent>(victim);
		}
		return false;
	}

	private bool CanInfectPopup(Entity<XenoParasiteComponent> parasite, EntityUid victim, EntityUid user, bool popup = true, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInfectable(parasite, victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-failed-cant-infect", (ValueTuple<string, object>)("target", victim)), victim, user, PopupType.MediumCaution);
			}
			return false;
		}
		StandingStateComponent standing = default(StandingStateComponent);
		if (!force && !((EntitySystem)this).HasComp<XenoNestedComponent>(victim) && ((EntitySystem)this).TryComp<StandingStateComponent>(victim, ref standing) && !_standing.IsDown(victim, standing))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-failed-cant-reach", (ValueTuple<string, object>)("target", victim)), victim, user, PopupType.MediumCaution);
			}
			return false;
		}
		if (_mobState.IsDead(victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-failed-target-dead"), victim, user, PopupType.MediumCaution);
			}
			return false;
		}
		if (_mobState.IsDead(Entity<XenoParasiteComponent>.op_Implicit(parasite)))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-failed-parasite-dead"), victim, user, PopupType.MediumCaution);
			}
			return false;
		}
		return true;
	}

	public bool Infect(Entity<XenoParasiteComponent> parasite, EntityUid victim, bool popup = true, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInfectPopup(parasite, victim, Entity<XenoParasiteComponent>.op_Implicit(parasite), popup, force))
		{
			return false;
		}
		InfectableComponent infectable = default(InfectableComponent);
		if (!((EntitySystem)this).TryComp<InfectableComponent>(victim, ref infectable))
		{
			return false;
		}
		if (_net.IsServer)
		{
			Vector2 pos = _transform.GetWorldPosition(victim);
			_transform.SetWorldPosition(Entity<XenoParasiteComponent>.op_Implicit(parasite), pos);
			ParasiteAIComponent ai = default(ParasiteAIComponent);
			if (((EntitySystem)this).TryComp<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite), ref ai))
			{
				ai.JumpsLeft--;
				if (_random.NextFloat() < ai.IdleChance)
				{
					GoIdle(Entity<ParasiteAIComponent>.op_Implicit((Entity<XenoParasiteComponent>.op_Implicit(parasite), ai)));
				}
			}
			TrapParasiteComponent trap = default(TrapParasiteComponent);
			if (((EntitySystem)this).TryComp<TrapParasiteComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite), ref trap))
			{
				ResetTrapState(Entity<TrapParasiteComponent>.op_Implicit((parasite.Owner, trap)));
			}
		}
		if (!TryRipOffClothing(victim, SlotFlags.HEAD))
		{
			return false;
		}
		if (!TryRipOffClothing(victim, SlotFlags.MASK, doPopup: false))
		{
			return false;
		}
		HumanoidAppearanceComponent appearance = default(HumanoidAppearanceComponent);
		if (_net.IsServer && ((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(victim, ref appearance) && infectable.Sound.TryGetValue(appearance.Sex, out SoundSpecifier sound))
		{
			_audio.PlayPvs(sound, victim, (AudioParams?)null);
		}
		infectable.BeingInfected = true;
		((EntitySystem)this).Dirty(victim, (IComponent)(object)infectable, (MetaDataComponent)null);
		_size.TryKnockOut(victim, parasite.Comp.ParalyzeTime);
		RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(victim));
		_inventory.TryEquip(victim, parasite.Owner, "mask", silent: true, force: true, predicted: true);
		((EntitySystem)this).EnsureComp<UnremoveableComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite)).DeleteOnDrop = false;
		parasite.Comp.InfectedVictim = victim;
		parasite.Comp.FallOffAt = _timing.CurTime + parasite.Comp.FallOffDelay;
		((EntitySystem)this).Dirty<XenoParasiteComponent>(parasite, (MetaDataComponent)null);
		((EntitySystem)this).RemCompDeferred<RMCGibOnDeathComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite));
		((EntitySystem)this).RemCompDeferred<ParasiteAIComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite));
		XenoParasiteInfectEvent ev = new XenoParasiteInfectEvent(victim, parasite.Owner);
		((EntitySystem)this).RaiseLocalEvent<XenoParasiteInfectEvent>(victim, ev, true);
		ParasiteLeapHit(parasite);
		return true;
	}

	public void RefreshIncubationMultipliers(Entity<VictimInfectedComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VictimInfectedComponent>(Entity<VictimInfectedComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return;
		}
		GetInfectedIncubationMultiplierEvent ev = new GetInfectedIncubationMultiplierEvent(ent.Comp.CurrentStage);
		((EntitySystem)this).RaiseLocalEvent<GetInfectedIncubationMultiplierEvent>(Entity<VictimInfectedComponent>.op_Implicit(ent), ref ev, false);
		float multiplier = 1f;
		foreach (float add in ev.Additions)
		{
			multiplier += add;
		}
		foreach (float multi in ev.Multipliers)
		{
			multiplier *= multi;
		}
		ent.Comp.IncubationMultiplier = multiplier;
	}

	public override void Update(float frameTime)
	{
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (_net.IsServer)
		{
			EntityQueryEnumerator<ParasiteAIComponent> aiQuery = ((EntitySystem)this).EntityQueryEnumerator<ParasiteAIComponent>();
			EntityUid uid = default(EntityUid);
			ParasiteAIComponent ai = default(ParasiteAIComponent);
			while (aiQuery.MoveNext(ref uid, ref ai))
			{
				if (!_mobState.IsDead(uid) && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
				{
					UpdateAI(Entity<ParasiteAIComponent>.op_Implicit((uid, ai)), time);
				}
			}
			EntityQueryEnumerator<TrapParasiteComponent> trapQuery = ((EntitySystem)this).EntityQueryEnumerator<TrapParasiteComponent>();
			EntityUid uid2 = default(EntityUid);
			TrapParasiteComponent trap = default(TrapParasiteComponent);
			while (trapQuery.MoveNext(ref uid2, ref trap))
			{
				if (!(trap.LeapAt > time) && !_mobState.IsDead(uid2) && !((EntitySystem)this).TerminatingOrDeleted(uid2, (MetaDataComponent)null))
				{
					_rmcNpc.WakeNPC(uid2);
					if (!(trap.DisableAt > time))
					{
						((EntitySystem)this).RemCompDeferred<TrapParasiteComponent>(uid2);
					}
				}
			}
			EntityQueryEnumerator<ParasiteAIDelayAddComponent> aiDelayQuery = ((EntitySystem)this).EntityQueryEnumerator<ParasiteAIDelayAddComponent>();
			EntityUid uid3 = default(EntityUid);
			ParasiteAIDelayAddComponent aid = default(ParasiteAIDelayAddComponent);
			while (aiDelayQuery.MoveNext(ref uid3, ref aid))
			{
				if (time > aid.TimeToAI)
				{
					((EntitySystem)this).EnsureComp<ParasiteAIComponent>(uid3);
					((EntitySystem)this).RemCompDeferred<ParasiteAIDelayAddComponent>(uid3);
				}
			}
		}
		EntityQueryEnumerator<XenoParasiteComponent> paraQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoParasiteComponent>();
		EntityUid uid4 = default(EntityUid);
		XenoParasiteComponent para = default(XenoParasiteComponent);
		InfectableComponent infectable = default(InfectableComponent);
		while (paraQuery.MoveNext(ref uid4, ref para))
		{
			TimeSpan? fallOffAt = para.FallOffAt;
			TimeSpan timeSpan = time;
			if (fallOffAt.HasValue && fallOffAt.GetValueOrDefault() < timeSpan && !para.FellOff && para.InfectedVictim.HasValue)
			{
				EntityUid infectedVictim = para.InfectedVictim.Value;
				if (((EntitySystem)this).TryComp<InfectableComponent>(infectedVictim, ref infectable))
				{
					para.FellOff = true;
					((EntitySystem)this).Dirty(uid4, (IComponent)(object)para, (MetaDataComponent)null);
					_inventory.TryUnequip(infectedVictim, "mask", silent: true, force: true, predicted: true);
					VictimInfectedComponent victimComp = ((EntitySystem)this).EnsureComp<VictimInfectedComponent>(infectedVictim);
					SetHive(Entity<VictimInfectedComponent>.op_Implicit((infectedVictim, victimComp)), _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(uid4))?.Owner);
					((EntitySystem)this).EnsureComp<ParasiteSpentComponent>(uid4);
					infectable.BeingInfected = false;
					((EntitySystem)this).Dirty(infectedVictim, (IComponent)(object)infectable, (MetaDataComponent)null);
				}
			}
		}
		EntityQueryEnumerator<VictimInfectedComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VictimInfectedComponent, TransformComponent>();
		EntityUid uid5 = default(EntityUid);
		VictimInfectedComponent infected = default(VictimInfectedComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid5, ref infected, ref xform))
		{
			if (_net.IsClient)
			{
				continue;
			}
			if (infected.BurstAt + infected.AutoBurstTime <= time && infected.SpawnedLarva.HasValue)
			{
				TryBurst(Entity<VictimInfectedComponent>.op_Implicit((uid5, infected)));
				continue;
			}
			if (_mobState.IsDead(uid5) && (((EntitySystem)this).HasComp<InfectStopOnDeathComponent>(uid5) || _rotting.IsRotten(uid5) || (_unrevivable.IsUnrevivable(uid5) && _unrevivable.DoesKillLarvaOnUnrevivable(Entity<RMCRevivableComponent>.op_Implicit(uid5)))))
			{
				if (infected.SpawnedLarva.HasValue)
				{
					TryBurst(Entity<VictimInfectedComponent>.op_Implicit((uid5, infected)));
				}
				else
				{
					((EntitySystem)this).RemCompDeferred<VictimInfectedComponent>(uid5);
				}
				continue;
			}
			if (infected.IncubationMultiplier != 1f)
			{
				infected.BurstAt += TimeSpan.FromSeconds(1f - infected.IncubationMultiplier) * frameTime;
			}
			if (infected.BurstAt <= time && !infected.SpawnedLarva.HasValue)
			{
				SpawnLarva(Entity<VictimInfectedComponent>.op_Implicit((uid5, infected)), out var _);
			}
			int stage = Math.Max((int)((infected.BurstDelay - (infected.BurstAt - time)) / infected.BurstDelay * (double)infected.FinalStage), infected.CurrentStage);
			if (stage != infected.CurrentStage)
			{
				infected.CurrentStage = stage;
				((EntitySystem)this).Dirty(uid5, (IComponent)(object)infected, (MetaDataComponent)null);
				RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(uid5));
			}
			if (!infected.DidBurstWarning && stage == infected.BurstWarningStart)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-burst-soon-self"), uid5, uid5, PopupType.MediumCaution);
				TimeSpan knockdownTime = infected.BaseKnockdownTime * 75.0;
				InfectionShakes(uid5, infected, knockdownTime, infected.JitterTime, popups: false);
				infected.DidBurstWarning = true;
			}
			else if (stage >= infected.BurstWarningStart)
			{
				if (RandomExtensions.Prob(_random, infected.InsanePainChance * frameTime))
				{
					string random = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "one", "two", "three", "four", "five" });
					string message = base.Loc.GetString("rmc-xeno-infection-insanepain-" + random);
					_popup.PopupEntity(message, uid5, uid5, PopupType.LargeCaution);
					TimeSpan knockdownTime2 = infected.BaseKnockdownTime * 2.0;
					TimeSpan jitterTime = infected.JitterTime * 0.0;
					InfectionShakes(uid5, infected, knockdownTime2, jitterTime, popups: false);
				}
			}
			else if (stage >= infected.FinalSymptomsStart)
			{
				if (RandomExtensions.Prob(_random, infected.MajorPainChance * frameTime))
				{
					string message2 = base.Loc.GetString("rmc-xeno-infection-majorpain-" + RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "chest", "breathing", "heart" }));
					_popup.PopupEntity(message2, uid5, uid5, PopupType.SmallCaution);
					if (RandomExtensions.Prob(_random, 0.5f))
					{
						VictimInfectedEmoteEvent ev = new VictimInfectedEmoteEvent(infected.ScreamId);
						((EntitySystem)this).RaiseLocalEvent<VictimInfectedEmoteEvent>(uid5, ref ev, false);
					}
				}
				if (RandomExtensions.Prob(_random, infected.ShakesChance * frameTime))
				{
					InfectionShakes(uid5, infected, infected.BaseKnockdownTime * 4.0, infected.JitterTime * 4.0);
				}
			}
			else if (stage >= infected.MiddlingSymptomsStart)
			{
				if (RandomExtensions.Prob(_random, infected.ThroatPainChance * frameTime))
				{
					string message3 = base.Loc.GetString("rmc-xeno-infection-throat-" + RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "sore", "mucous" }));
					_popup.PopupEntity(message3, uid5, uid5, PopupType.SmallCaution);
				}
				else if (RandomExtensions.Prob(_random, infected.MuscleAcheChance * frameTime))
				{
					_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-muscle-ache"), uid5, uid5, PopupType.SmallCaution);
					if (RandomExtensions.Prob(_random, 0.2f))
					{
						_damage.TryChangeDamage(uid5, infected.InfectionDamage, ignoreResistances: true, interruptsDoAfters: false);
					}
				}
				else if (RandomExtensions.Prob(_random, infected.SneezeCoughChance * frameTime))
				{
					ProtoId<EmotePrototype> emote = RandomExtensions.Pick<ProtoId<EmotePrototype>>(_random, (IReadOnlyList<ProtoId<EmotePrototype>>)new List<ProtoId<EmotePrototype>> { infected.SneezeId, infected.CoughId });
					VictimInfectedEmoteEvent ev2 = new VictimInfectedEmoteEvent(emote);
					((EntitySystem)this).RaiseLocalEvent<VictimInfectedEmoteEvent>(uid5, ref ev2, false);
				}
				if (RandomExtensions.Prob(_random, infected.ShakesChance * 5f / 6f * frameTime))
				{
					InfectionShakes(uid5, infected, infected.BaseKnockdownTime * 2.0, infected.JitterTime * 2.0);
				}
			}
			else if (stage >= infected.InitialSymptomsStart)
			{
				if (RandomExtensions.Prob(_random, infected.MinorPainChance * frameTime))
				{
					string message4 = base.Loc.GetString("rmc-xeno-infection-minorpain-" + RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "stomach", "chest" }));
					_popup.PopupEntity(message4, uid5, uid5, PopupType.SmallCaution);
				}
				if (RandomExtensions.Prob(_random, infected.ShakesChance * 2f / 3f * frameTime))
				{
					InfectionShakes(uid5, infected, infected.BaseKnockdownTime, infected.JitterTime);
				}
			}
		}
	}

	private void InfectionShakes(EntityUid victim, VictimInfectedComponent infected, TimeSpan knockdownTime, TimeSpan jitterTime, bool popups = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (_mobState.IsIncapacitated(victim))
		{
			return;
		}
		_size.TryKnockOut(victim, knockdownTime);
		_jitter.DoJitter(victim, jitterTime, refresh: false);
		_damage.TryChangeDamage(victim, infected.InfectionDamage, ignoreResistances: true, interruptsDoAfters: false);
		if (popups)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-shakes-self"), victim, victim, PopupType.MediumCaution);
			BaseContainer container = default(BaseContainer);
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(victim), ref container) || !((EntitySystem)this).HasComp<RMCHideParasiteInfectionContainerPopupComponent>(container.Owner))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-shakes", (ValueTuple<string, object>)("victim", victim)), victim, Filter.PvsExcept(victim, 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
			}
		}
	}

	private void OnTryMove(Entity<BursterComponent> burster, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		VictimInfectedComponent infected = default(VictimInfectedComponent);
		if (args.HasDirectionalMovement && ((EntitySystem)this).TryComp<VictimInfectedComponent>(burster.Comp.BurstFrom, ref infected) && !infected.IsBursting)
		{
			TryBurst(Entity<VictimInfectedComponent>.op_Implicit((burster.Comp.BurstFrom, infected)));
		}
	}

	private void TryBurst(Entity<VictimInfectedComponent> burstFrom)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		EntityUid victim = burstFrom.Owner;
		VictimInfectedComponent comp = burstFrom.Comp;
		if (!comp.SpawnedLarva.HasValue || comp.IsBursting)
		{
			return;
		}
		comp.IsBursting = true;
		((EntitySystem)this).Dirty(victim, (IComponent)(object)comp, (MetaDataComponent)null);
		EntityUid spawnedLarva = comp.SpawnedLarva.Value;
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, spawnedLarva, comp.BurstDoAfterDelay, new LarvaBurstDoAfterEvent(), victim, victim)
		{
			NeedHand = false,
			BreakOnDamage = false,
			BreakOnMove = false,
			BreakOnRest = false,
			Hidden = true,
			CancelDuplicate = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (_doAfter.TryStartDoAfter(doAfterEventArgs))
		{
			((EntitySystem)this).EnsureComp<VictimBurstComponent>(Entity<VictimInfectedComponent>.op_Implicit(burstFrom));
			_appearance.SetData(burstFrom.Owner, (Enum)BurstVisuals.Visuals, (object)VictimBurstState.Bursting, (AppearanceComponent)null);
			Filter shakeFilter = Filter.PvsExcept(victim, 2f, (IEntityManager)null);
			shakeFilter.RemoveWhereAttachedEntity((Predicate<EntityUid>)base.HasComp<BursterComponent>);
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-burst-now-victim"), victim, victim, PopupType.MediumCaution);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-infection-burst-soon", (ValueTuple<string, object>)("victim", victim)), victim, shakeFilter, recordReplay: true, PopupType.LargeCaution);
				_jitter.DoJitter(victim, comp.JitterTime / 1.2, refresh: true, 14f, 5f, forceValueChange: true);
			}
			string messageLarva = base.Loc.GetString("rmc-xeno-infection-burst-now-xeno", (ValueTuple<string, object>)("victim", Identity.Entity(victim, (IEntityManager)(object)base.EntityManager)));
			_popup.PopupClient(messageLarva, spawnedLarva, spawnedLarva, PopupType.MediumCaution);
		}
	}

	private void OnBurst(Entity<VictimInfectedComponent> ent, ref LarvaBurstDoAfterEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || _net.IsClient)
		{
			return;
		}
		((EntitySystem)this).EnsureComp<VictimBurstComponent>(ent.Owner);
		_appearance.SetData(ent.Owner, (Enum)BurstVisuals.Visuals, (object)VictimBurstState.Burst, (AppearanceComponent)null);
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(ent.Owner, ref mobState))
		{
			_mobState.UpdateMobState(ent.Owner, mobState);
		}
		EntityCoordinates coords = _transform.GetMoverCoordinates(Entity<VictimInfectedComponent>.op_Implicit(ent));
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<VictimInfectedComponent>.op_Implicit(ent), ent.Comp.LarvaContainerId, ref container, (ContainerManagerComponent)null))
		{
			foreach (EntityUid larva in container.ContainedEntities)
			{
				((EntitySystem)this).RemCompDeferred<BursterComponent>(larva);
				RMCTemporaryInvincibilityComponent invc = ((EntitySystem)this).EnsureComp<RMCTemporaryInvincibilityComponent>(larva);
				invc.ExpiresAt = _timing.CurTime + ent.Comp.LarvaInvincibilityTime;
				((EntitySystem)this).Dirty(larva, (IComponent)(object)invc, (MetaDataComponent)null);
			}
			_container.EmptyContainer(container, false, (EntityCoordinates?)coords, true);
		}
		((EntitySystem)this).Dirty<VictimInfectedComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RemCompDeferred<VictimInfectedComponent>(Entity<VictimInfectedComponent>.op_Implicit(ent));
		_audio.PlayPvs(ent.Comp.BurstSound, args.User, (AudioParams?)null);
	}

	private bool TryRipOffClothing(EntityUid victim, SlotFlags slotFlags, bool doPopup = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(victim), out var slots))
		{
			return true;
		}
		EntityUid? rippedOffItem = null;
		EntityUid containedEntity;
		SlotDefinition inventorySlot;
		ParasiteResistanceComponent resistance = default(ParasiteResistanceComponent);
		while (slots.NextItem(out containedEntity, out inventorySlot))
		{
			if ((inventorySlot.SlotFlags & slotFlags) == 0 && !_tagSystem.HasTag(containedEntity, ProtoId<TagPrototype>.op_Implicit("RipOffOnInfection")))
			{
				continue;
			}
			((EntitySystem)this).TryComp<ParasiteResistanceComponent>(containedEntity, ref resistance);
			if (resistance != null && resistance.Count < resistance.MaxCount)
			{
				resistance.Count += 1f;
				((EntitySystem)this).Dirty(containedEntity, (IComponent)(object)resistance, (MetaDataComponent)null);
				if (_net.IsServer && doPopup)
				{
					string popupMessage = base.Loc.GetString("rmc-xeno-infect-fail", (ValueTuple<string, object>)("target", victim), (ValueTuple<string, object>)("clothing", containedEntity));
					_popup.PopupEntity(popupMessage, victim, PopupType.SmallCaution);
				}
				return false;
			}
			_inventory.TryUnequip(victim, victim, inventorySlot.Name, silent: false, force: true);
			rippedOffItem = containedEntity;
		}
		if (_net.IsServer && doPopup && rippedOffItem.HasValue)
		{
			string popupMessage2 = base.Loc.GetString("rmc-xeno-infect-success", (ValueTuple<string, object>)("target", victim), (ValueTuple<string, object>)("clothing", rippedOffItem));
			_popup.PopupEntity(popupMessage2, victim, PopupType.MediumCaution);
		}
		return true;
	}

	public void SetBurstSpawn(Entity<VictimInfectedComponent> burst, EntProtoId spawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		burst.Comp.BurstSpawn = spawn;
		((EntitySystem)this).Dirty<VictimInfectedComponent>(burst, (MetaDataComponent)null);
	}

	public void SetBurstSound(Entity<VictimInfectedComponent> burst, SoundSpecifier sound)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		burst.Comp.BurstSound = sound;
		((EntitySystem)this).Dirty<VictimInfectedComponent>(burst, (MetaDataComponent)null);
	}

	public void TryStartBurst(Entity<VictimInfectedComponent> burst)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetBurstDelay(burst, TimeSpan.Zero);
		TryBurst(burst);
	}

	public void SetBurstDelay(Entity<VictimInfectedComponent> burst, TimeSpan time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		burst.Comp.BurstAt = _timing.CurTime + time;
		((EntitySystem)this).Dirty<VictimInfectedComponent>(burst, (MetaDataComponent)null);
	}

	public void SetHive(Entity<VictimInfectedComponent> burst, EntityUid? hive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		burst.Comp.Hive = hive;
		((EntitySystem)this).Dirty<VictimInfectedComponent>(burst, (MetaDataComponent)null);
	}

	public void SpawnLarva(Entity<VictimInfectedComponent> victim, out EntityUid spawned)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot larvaContainer = _container.EnsureContainer<ContainerSlot>(victim.Owner, victim.Comp.LarvaContainerId, (ContainerManagerComponent)null);
		spawned = ((EntitySystem)this).SpawnInContainerOrDrop(EntProtoId.op_Implicit(victim.Comp.BurstSpawn), victim.Owner, ((BaseContainer)larvaContainer).ID, (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
		LinkLarvaToVictim(victim, spawned);
	}

	public void InsertLarva(Entity<VictimInfectedComponent> victim, EntityUid spawned)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot larvaContainer = _container.EnsureContainer<ContainerSlot>(victim.Owner, victim.Comp.LarvaContainerId, (ContainerManagerComponent)null);
		_container.InsertOrDrop(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(spawned), (BaseContainer)(object)larvaContainer, (TransformComponent)null);
		LinkLarvaToVictim(victim, spawned);
	}

	private void LinkLarvaToVictim(Entity<VictimInfectedComponent> victim, EntityUid spawned)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(spawned))
		{
			_hive.SetHive(Entity<HiveMemberComponent>.op_Implicit(spawned), victim.Comp.Hive);
		}
		victim.Comp.CurrentStage = 6;
		victim.Comp.SpawnedLarva = spawned;
		((EntitySystem)this).Dirty<VictimInfectedComponent>(victim, (MetaDataComponent)null);
		BursterComponent burster = default(BursterComponent);
		((EntitySystem)this).EnsureComp<BursterComponent>(spawned, ref burster);
		burster.BurstFrom = victim.Owner;
		((EntitySystem)this).Dirty(spawned, (IComponent)(object)burster, (MetaDataComponent)null);
	}
}
