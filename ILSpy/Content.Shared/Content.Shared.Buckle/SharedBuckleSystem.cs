using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Buckle;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Standing;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.Construction;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Rotation;
using Content.Shared.Standing;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Buckle;

public abstract class SharedBuckleSystem : EntitySystem
{
	public static ProtoId<AlertCategoryPrototype> BuckledAlertCategory = ProtoId<AlertCategoryPrototype>.op_Implicit("Buckled");

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private RMCBuckleSystem _rmcBuckle;

	[Dependency]
	private RMCMovementSystem _rmcMovement;

	[Dependency]
	private TagSystem _tags;

	private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ISharedPlayerManager _playerManager;

	[Dependency]
	protected ActionBlockerSystem ActionBlocker;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedJointSystem _joints;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedRotationVisualsSystem _rotationVisuals;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	private void InitializeBuckle()
	{
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, ComponentShutdown>((EntityEventRefHandler<BuckleComponent, ComponentShutdown>)OnBuckleComponentShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, MoveEvent>((EntityEventRefHandler<BuckleComponent, MoveEvent>)OnBuckleMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, EntParentChangedMessage>((EntityEventRefHandler<BuckleComponent, EntParentChangedMessage>)OnParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<BuckleComponent, EntGotInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, StartPullAttemptEvent>((EntityEventRefHandler<BuckleComponent, StartPullAttemptEvent>)OnPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, BeingPulledAttemptEvent>((EntityEventRefHandler<BuckleComponent, BeingPulledAttemptEvent>)OnBeingPulledAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, PullStartedMessage>((EntityEventRefHandler<BuckleComponent, PullStartedMessage>)OnPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, UnbuckleAlertEvent>((EntityEventRefHandler<BuckleComponent, UnbuckleAlertEvent>)OnUnbuckleAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, InsertIntoEntityStorageAttemptEvent>((ComponentEventRefHandler<BuckleComponent, InsertIntoEntityStorageAttemptEvent>)OnBuckleInsertIntoEntityStorageAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, PreventCollideEvent>((ComponentEventRefHandler<BuckleComponent, PreventCollideEvent>)OnBucklePreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, DownAttemptEvent>((ComponentEventHandler<BuckleComponent, DownAttemptEvent>)OnBuckleDownAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, StandAttemptEvent>((ComponentEventHandler<BuckleComponent, StandAttemptEvent>)OnBuckleStandAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, ThrowPushbackAttemptEvent>((ComponentEventHandler<BuckleComponent, ThrowPushbackAttemptEvent>)OnBuckleThrowPushbackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, UpdateCanMoveEvent>((ComponentEventHandler<BuckleComponent, UpdateCanMoveEvent>)OnBuckleUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, BuckleDoAfterEvent>((EntityEventRefHandler<BuckleComponent, BuckleDoAfterEvent>)OnBuckleDoafter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, DoAfterAttemptEvent<BuckleDoAfterEvent>>((ComponentEventHandler<BuckleComponent, DoAfterAttemptEvent<BuckleDoAfterEvent>>)delegate(EntityUid uid, BuckleComponent comp, DoAfterAttemptEvent<BuckleDoAfterEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			BuckleDoafterEarly(Entity<BuckleComponent>.op_Implicit((uid, comp)), ev.Event, (CancellableEntityEventArgs)(object)ev);
		}, (Type[])null, (Type[])null);
	}

	private void OnBuckleComponentShutdown(Entity<BuckleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Unbuckle(ent, null);
	}

	private void OnPullAttempt(Entity<BuckleComponent> ent, ref StartPullAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? buckledTo = ent.Comp.BuckledTo;
		EntityUid pulled = args.Pulled;
		if (buckledTo.HasValue && buckledTo.GetValueOrDefault() == pulled && !ent.Comp.PullStrap)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeingPulledAttempt(Entity<BuckleComponent> ent, ref BeingPulledAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && ent.Comp.Buckled && !CanUnbuckle(ent, args.Puller, popup: false))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnPullStarted(Entity<BuckleComponent> ent, ref PullStartedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		Unbuckle(ent, args.PullerUid);
	}

	private void OnUnbuckleAlert(Entity<BuckleComponent> ent, ref UnbuckleAlertEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryUnbuckle(Entity<BuckleComponent>.op_Implicit(ent), Entity<BuckleComponent>.op_Implicit(ent), Entity<BuckleComponent>.op_Implicit(ent));
		}
	}

	private void OnParentChanged(Entity<BuckleComponent> ent, ref EntParentChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuckleTransformCheck(ent, ((EntParentChangedMessage)(ref args)).Transform);
	}

	private void OnInserted(Entity<BuckleComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		BuckleTransformCheck(ent, ((EntitySystem)this).Transform(Entity<BuckleComponent>.op_Implicit(ent)));
	}

	private void OnBuckleMove(Entity<BuckleComponent> ent, ref MoveEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuckleTransformCheck(ent, ((MoveEvent)(ref ev)).Component);
	}

	private void BuckleTransformCheck(Entity<BuckleComponent> buckle, TransformComponent xform)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (_gameTiming.ApplyingState)
		{
			return;
		}
		EntityUid? buckledTo = buckle.Comp.BuckledTo;
		if (buckledTo.HasValue)
		{
			EntityUid strapUid = buckledTo.GetValueOrDefault();
			StrapComponent strapComp = default(StrapComponent);
			if (!((EntitySystem)this).TryComp<StrapComponent>(strapUid, ref strapComp))
			{
				((EntitySystem)this).Log.Error($"Encountered buckle entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<BuckleComponent>.op_Implicit(buckle), (MetaDataComponent)null)} without a valid strap entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(strapUid))}");
				SetBuckledTo(buckle, null);
			}
			else if (xform.ParentUid != strapUid || _container.IsEntityInContainer(Entity<BuckleComponent>.op_Implicit(buckle), (MetaDataComponent)null))
			{
				Unbuckle(buckle, Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), null);
			}
			else if ((double)(xform.LocalPosition - strapComp.BuckleOffset - _rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner))).LengthSquared() > 1E-05)
			{
				Unbuckle(buckle, Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), null);
			}
		}
	}

	public void SetBuckleOffset(Entity<StrapComponent?> strap, Vector2 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StrapComponent>(Entity<StrapComponent>.op_Implicit(strap), ref strap.Comp, false))
		{
			strap.Comp.BuckleOffset = offset;
			((EntitySystem)this).Dirty(Entity<StrapComponent>.op_Implicit(strap), (IComponent)(object)strap.Comp, (MetaDataComponent)null);
		}
	}

	private void OnBuckleInsertIntoEntityStorageAttempt(EntityUid uid, BuckleComponent component, ref InsertIntoEntityStorageAttemptEvent args)
	{
		if (component.Buckled)
		{
			args.Cancelled = true;
		}
	}

	private void OnBucklePreventCollide(EntityUid uid, BuckleComponent component, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherEntity = args.OtherEntity;
		EntityUid? buckledTo = component.BuckledTo;
		if (buckledTo.HasValue && otherEntity == buckledTo.GetValueOrDefault() && component.DontCollide)
		{
			args.Cancelled = true;
		}
		if (component.Buckled && _tags.HasTag(args.OtherEntity, WallTag))
		{
			args.Cancelled = true;
		}
	}

	private void OnBuckleDownAttempt(EntityUid uid, BuckleComponent component, DownAttemptEvent args)
	{
		if (component.Buckled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBuckleStandAttempt(EntityUid uid, BuckleComponent component, StandAttemptEvent args)
	{
		if (component.Buckled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBuckleThrowPushbackAttempt(EntityUid uid, BuckleComponent component, ThrowPushbackAttemptEvent args)
	{
		if (component.Buckled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBuckleUpdateCanMove(EntityUid uid, BuckleComponent component, UpdateCanMoveEvent args)
	{
		if (!((EntitySystem)this).HasComp<RMCAllowStrapMovementComponent>(component.BuckledTo) && component.Buckled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public bool IsBuckled(EntityUid uid, BuckleComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BuckleComponent>(uid, ref component, false))
		{
			return component.Buckled;
		}
		return false;
	}

	protected void SetBuckledTo(Entity<BuckleComponent> buckle, Entity<StrapComponent?>? strap)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent old = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(buckle.Comp.BuckledTo, ref old))
		{
			old.BuckledEntities.Remove(Entity<BuckleComponent>.op_Implicit(buckle));
			((EntitySystem)this).Dirty(buckle.Comp.BuckledTo.Value, (IComponent)(object)old, (MetaDataComponent)null);
		}
		if (strap.HasValue)
		{
			Entity<StrapComponent> strapEnt = strap.GetValueOrDefault();
			if (((EntitySystem)this).Resolve<StrapComponent>(strapEnt.Owner, ref strapEnt.Comp, true))
			{
				strapEnt.Comp.BuckledEntities.Add(Entity<BuckleComponent>.op_Implicit(buckle));
				((EntitySystem)this).Dirty<StrapComponent>(strapEnt, (MetaDataComponent)null);
				if (strapEnt.Comp.BuckledAlertType.HasValue)
				{
					_alerts.ShowAlert(Entity<BuckleComponent>.op_Implicit(buckle), strapEnt.Comp.BuckledAlertType.Value);
				}
				goto IL_00e8;
			}
		}
		_alerts.ClearAlertCategory(Entity<BuckleComponent>.op_Implicit(buckle), BuckledAlertCategory);
		goto IL_00e8;
		IL_00e8:
		BuckleComponent comp = buckle.Comp;
		Entity<StrapComponent>? val = strap;
		comp.BuckledTo = (val.HasValue ? new EntityUid?(Entity<StrapComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
		buckle.Comp.BuckleTime = _gameTiming.CurTime;
		ActionBlocker.UpdateCanMove(Entity<BuckleComponent>.op_Implicit(buckle));
		Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum)StrapVisuals.State, (object)buckle.Comp.Buckled, (AppearanceComponent)null);
		((EntitySystem)this).Dirty<BuckleComponent>(buckle, (MetaDataComponent)null);
	}

	private bool CanBuckle(EntityUid buckleUid, EntityUid? user, EntityUid strapUid, bool popup, [NotNullWhen(true)] out StrapComponent? strapComp, BuckleComponent buckleComp)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		strapComp = null;
		if (!((EntitySystem)this).Resolve<StrapComponent>(strapUid, ref strapComp, false))
		{
			return false;
		}
		if (!strapComp.Enabled)
		{
			return false;
		}
		if (!_rmcMovement.CanClimbOver(user, buckleUid, strapUid, includeTarget: false))
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(strapComp.Whitelist, buckleUid) || _whitelistSystem.IsBlacklistPass(strapComp.Blacklist, buckleUid))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("buckle-component-cannot-fit-message"), user, PopupType.Medium);
			}
			return false;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(buckleUid), Entity<TransformComponent>.op_Implicit(strapUid), buckleComp.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, delegate(EntityUid entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (!(entity == buckleUid))
			{
				EntityUid? val3 = user;
				if (!val3.HasValue || !(entity == val3.GetValueOrDefault()))
				{
					return entity == strapUid;
				}
			}
			return true;
		}, popup: true))
		{
			return false;
		}
		if (!_container.IsInSameOrNoContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(buckleUid, null, null)), Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(strapUid, null, null))))
		{
			return false;
		}
		if (user.HasValue && !((EntitySystem)this).HasComp<HandsComponent>(user))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("buckle-component-no-hands-message"), user);
			}
			return false;
		}
		if (buckleComp.Buckled && !TryUnbuckle(buckleUid, user, buckleComp))
		{
			if (popup)
			{
				ILocalizationManager loc = base.Loc;
				EntityUid val = buckleUid;
				EntityUid? val2 = user;
				string message = loc.GetString((val2.HasValue && val == val2.GetValueOrDefault()) ? "buckle-component-already-buckled-message" : "buckle-component-other-already-buckled-message", (ValueTuple<string, object>)("owner", Identity.Entity(buckleUid, (IEntityManager)(object)base.EntityManager)));
				_popup.PopupClient(message, user);
			}
			return false;
		}
		EntityUid parent = ((EntitySystem)this).Transform(strapUid).ParentUid;
		while (((EntityUid)(ref parent)).IsValid())
		{
			if (parent != buckleUid)
			{
				parent = ((EntitySystem)this).Transform(parent).ParentUid;
				continue;
			}
			if (popup)
			{
				ILocalizationManager loc2 = base.Loc;
				EntityUid val = buckleUid;
				EntityUid? val2 = user;
				string message2 = loc2.GetString((val2.HasValue && val == val2.GetValueOrDefault()) ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message", (ValueTuple<string, object>)("owner", Identity.Entity(buckleUid, (IEntityManager)(object)base.EntityManager)));
				_popup.PopupClient(message2, user);
			}
			return false;
		}
		if (!StrapHasSpace(strapUid, buckleComp, strapComp))
		{
			if (popup)
			{
				ILocalizationManager loc3 = base.Loc;
				EntityUid val = buckleUid;
				EntityUid? val2 = user;
				string message3 = loc3.GetString((val2.HasValue && val == val2.GetValueOrDefault()) ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message", (ValueTuple<string, object>)("owner", Identity.Entity(buckleUid, (IEntityManager)(object)base.EntityManager)));
				_popup.PopupClient(message3, user);
			}
			return false;
		}
		if (!_rmcBuckle.CanBuckle(user, buckleUid, popup))
		{
			return false;
		}
		BuckleAttemptEvent buckleAttempt = new BuckleAttemptEvent(Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
		((EntitySystem)this).RaiseLocalEvent<BuckleAttemptEvent>(buckleUid, ref buckleAttempt, false);
		if (buckleAttempt.Cancelled)
		{
			return false;
		}
		StrapAttemptEvent strapAttempt = new StrapAttemptEvent(Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
		((EntitySystem)this).RaiseLocalEvent<StrapAttemptEvent>(strapUid, ref strapAttempt, false);
		if (strapAttempt.Cancelled)
		{
			return false;
		}
		return true;
	}

	public bool TryBuckle(EntityUid buckle, EntityUid? user, EntityUid strap, BuckleComponent? buckleComp = null, bool popup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BuckleComponent>(buckle, ref buckleComp, false))
		{
			return false;
		}
		if (!CanBuckle(buckle, user, strap, popup, out StrapComponent strapComp, buckleComp))
		{
			return false;
		}
		Buckle(Entity<BuckleComponent>.op_Implicit((buckle, buckleComp)), Entity<StrapComponent>.op_Implicit((strap, strapComp)), user);
		return true;
	}

	private void Buckle(Entity<BuckleComponent> buckle, Entity<StrapComponent> strap, EntityUid? user)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = user;
		EntityUid owner = buckle.Owner;
		if (val.HasValue && val.GetValueOrDefault() == owner)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(23, 2);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "player", "ToPrettyString(user)");
			handler.AppendLiteral(" buckled themselves to ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (MetaDataComponent)null), "ToPrettyString(strap)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		else if (user.HasValue)
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(13, 3);
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "player", "ToPrettyString(user)");
			handler2.AppendLiteral(" buckled ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<BuckleComponent>.op_Implicit(buckle), (MetaDataComponent)null), "ToPrettyString(buckle)");
			handler2.AppendLiteral(" to ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (MetaDataComponent)null), "ToPrettyString(strap)");
			adminLogger2.Add(LogType.Action, LogImpact.Low, ref handler2);
		}
		_audio.PlayPredicted(strap.Comp.BuckleSound, Entity<StrapComponent>.op_Implicit(strap), user, (AudioParams?)null);
		SetBuckledTo(buckle, strap);
		Appearance.SetData(Entity<StrapComponent>.op_Implicit(strap), (Enum)StrapVisuals.State, (object)true, (AppearanceComponent)null);
		Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum)BuckleVisuals.Buckled, (object)true, (AppearanceComponent)null);
		_rotationVisuals.SetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit(buckle.Owner), strap.Comp.Rotation);
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<BuckleComponent>.op_Implicit(buckle));
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(Entity<StrapComponent>.op_Implicit(strap), strap.Comp.BuckleOffset + _rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner)));
		_transform.SetCoordinates(Entity<BuckleComponent>.op_Implicit(buckle), xform, coords, (Angle?)Angle.Zero, true, (TransformComponent)null, (TransformComponent)null);
		_joints.SetRelay(Entity<BuckleComponent>.op_Implicit(buckle), (EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (JointComponent)null);
		switch (strap.Comp.Position)
		{
		case StrapPosition.Stand:
			_standing.Stand(Entity<BuckleComponent>.op_Implicit(buckle), null, null, force: true);
			break;
		case StrapPosition.Down:
			_standing.Down(Entity<BuckleComponent>.op_Implicit(buckle), playSound: false, dropHeldItems: false, force: true, changeCollision: true);
			break;
		}
		StrappedEvent ev = new StrappedEvent(strap, buckle);
		((EntitySystem)this).RaiseLocalEvent<StrappedEvent>(Entity<StrapComponent>.op_Implicit(strap), ref ev, false);
		BuckledEvent gotEv = new BuckledEvent(strap, buckle);
		((EntitySystem)this).RaiseLocalEvent<BuckledEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref gotEv, true);
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<BuckleComponent>.op_Implicit(buckle), ref physics))
		{
			_physics.ResetDynamics(Entity<BuckleComponent>.op_Implicit(buckle), physics, true);
		}
	}

	public bool TryUnbuckle(EntityUid buckleUid, EntityUid? user, BuckleComponent? buckleComp = null, bool popup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TryUnbuckle(Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
	}

	public bool TryUnbuckle(Entity<BuckleComponent?> buckle, EntityUid? user, bool popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, false))
		{
			return false;
		}
		if (!CanUnbuckle(buckle, user, popup, out Entity<StrapComponent> strap))
		{
			return false;
		}
		Unbuckle(buckle, strap, user);
		return true;
	}

	public void Unbuckle(Entity<BuckleComponent?> buckle, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, false))
		{
			return;
		}
		EntityUid? buckledTo = buckle.Comp.BuckledTo;
		if (buckledTo.HasValue)
		{
			EntityUid strap = buckledTo.GetValueOrDefault();
			StrapComponent strapComp = default(StrapComponent);
			if (!((EntitySystem)this).TryComp<StrapComponent>(strap, ref strapComp))
			{
				((EntitySystem)this).Log.Error($"Encountered buckle {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(buckle.Owner))} with invalid strap entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(strap))}");
				SetBuckledTo(buckle, null);
			}
			else
			{
				Unbuckle(buckle, Entity<StrapComponent>.op_Implicit((strap, strapComp)), user);
			}
		}
	}

	private void Unbuckle(Entity<BuckleComponent> buckle, Entity<StrapComponent> strap, EntityUid? user)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = user;
		EntityUid owner = buckle.Owner;
		if (val.HasValue && val.GetValueOrDefault() == owner)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(27, 2);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" unbuckled themselves from ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (MetaDataComponent)null), "strap", "ToPrettyString(strap)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		else if (user.HasValue)
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(17, 3);
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "user", "ToPrettyString(user)");
			handler2.AppendLiteral(" unbuckled ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<BuckleComponent>.op_Implicit(buckle), (MetaDataComponent)null), "target", "ToPrettyString(buckle)");
			handler2.AppendLiteral(" from ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (MetaDataComponent)null), "strap", "ToPrettyString(strap)");
			adminLogger2.Add(LogType.Action, LogImpact.Low, ref handler2);
		}
		_audio.PlayPredicted(strap.Comp.UnbuckleSound, Entity<StrapComponent>.op_Implicit(strap), user, (AudioParams?)null);
		EntityCoordinates buckledLocation = _transform.GetMoverCoordinates(Entity<BuckleComponent>.op_Implicit(buckle));
		SetBuckledTo(buckle, null);
		TransformComponent buckleXform = ((EntitySystem)this).Transform(Entity<BuckleComponent>.op_Implicit(buckle));
		TransformComponent oldBuckledXform = ((EntitySystem)this).Transform(Entity<StrapComponent>.op_Implicit(strap));
		if (buckleXform.ParentUid == strap.Owner && !((EntitySystem)this).Terminating(oldBuckledXform.ParentUid, (MetaDataComponent)null))
		{
			_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit((Entity<BuckleComponent>.op_Implicit(buckle), buckleXform)), Entity<TransformComponent>.op_Implicit((strap.Owner, oldBuckledXform)));
			buckleXform.ActivelyLerping = false;
			Angle oldBuckledToWorldRot = _transform.GetWorldRotation(Entity<StrapComponent>.op_Implicit(strap));
			_transform.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit((Entity<BuckleComponent>.op_Implicit(buckle), buckleXform)), oldBuckledToWorldRot);
			if (strap.Comp.BuckleOffset + _rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner)) != Vector2.Zero)
			{
				_transform.SetCoordinates(Entity<BuckleComponent>.op_Implicit(buckle), buckledLocation);
			}
		}
		_rotationVisuals.ResetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit(buckle.Owner));
		Appearance.SetData(Entity<StrapComponent>.op_Implicit(strap), (Enum)StrapVisuals.State, (object)(strap.Comp.BuckledEntities.Count != 0), (AppearanceComponent)null);
		Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum)BuckleVisuals.Buckled, (object)false, (AppearanceComponent)null);
		RMCRestComponent rest = default(RMCRestComponent);
		if (((EntitySystem)this).HasComp<KnockedDownComponent>(Entity<BuckleComponent>.op_Implicit(buckle)) || _mobState.IsIncapacitated(Entity<BuckleComponent>.op_Implicit(buckle)) || (((EntitySystem)this).TryComp<RMCRestComponent>(Entity<BuckleComponent>.op_Implicit(buckle), ref rest) && rest.Resting))
		{
			_standing.Down(Entity<BuckleComponent>.op_Implicit(buckle), playSound: false, dropHeldItems: true, force: false, changeCollision: true);
		}
		else
		{
			_standing.Stand(Entity<BuckleComponent>.op_Implicit(buckle));
		}
		_joints.RefreshRelay(Entity<BuckleComponent>.op_Implicit(buckle), (JointComponent)null);
		UnbuckledEvent buckleEv = new UnbuckledEvent(strap, buckle);
		((EntitySystem)this).RaiseLocalEvent<UnbuckledEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref buckleEv, true);
		UnstrappedEvent strapEv = new UnstrappedEvent(strap, buckle);
		((EntitySystem)this).RaiseLocalEvent<UnstrappedEvent>(Entity<StrapComponent>.op_Implicit(strap), ref strapEv, false);
	}

	public bool CanUnbuckle(Entity<BuckleComponent?> buckle, EntityUid user, bool popup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Entity<StrapComponent> strap;
		return CanUnbuckle(buckle, user, popup, out strap);
	}

	private bool CanUnbuckle(Entity<BuckleComponent?> buckle, EntityUid? user, bool popup, out Entity<StrapComponent> strap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		strap = default(Entity<StrapComponent>);
		if (!((EntitySystem)this).Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, true))
		{
			return false;
		}
		EntityUid? buckledTo = buckle.Comp.BuckledTo;
		if (buckledTo.HasValue)
		{
			EntityUid strapUid = buckledTo.GetValueOrDefault();
			StrapComponent strapComp = default(StrapComponent);
			if (!((EntitySystem)this).TryComp<StrapComponent>(strapUid, ref strapComp))
			{
				((EntitySystem)this).Log.Error($"Encountered buckle {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(buckle.Owner))} with invalid strap entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<StrapComponent>.op_Implicit(strap), (MetaDataComponent)null)}");
				SetBuckledTo(buckle, null);
				return false;
			}
			strap = Entity<StrapComponent>.op_Implicit((strapUid, strapComp));
			TimeSpan curTime = _gameTiming.CurTime;
			TimeSpan? timeSpan = buckle.Comp.BuckleTime + buckle.Comp.Delay;
			if (curTime < timeSpan)
			{
				return false;
			}
			if (user.HasValue)
			{
				if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user.Value), Entity<TransformComponent>.op_Implicit(strap.Owner), buckle.Comp.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup))
				{
					return false;
				}
				if (user.Value != buckle.Owner && !ActionBlocker.CanComplexInteract(user.Value))
				{
					return false;
				}
			}
			UnbuckleAttemptEvent unbuckleAttempt = new UnbuckleAttemptEvent(strap, buckle, user, popup);
			((EntitySystem)this).RaiseLocalEvent<UnbuckleAttemptEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref unbuckleAttempt, false);
			if (unbuckleAttempt.Cancelled)
			{
				return false;
			}
			UnstrapAttemptEvent unstrapAttempt = new UnstrapAttemptEvent(strap, buckle, user, popup);
			((EntitySystem)this).RaiseLocalEvent<UnstrapAttemptEvent>(Entity<StrapComponent>.op_Implicit(strap), ref unstrapAttempt, false);
			return !unstrapAttempt.Cancelled;
		}
		return false;
	}

	private void OnBuckleDoafter(Entity<BuckleComponent> entity, ref BuckleDoAfterEvent args)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && args.Target.HasValue && args.Used.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = TryBuckle(args.Target.Value, args.User, args.Used.Value, null, popup: false);
		}
	}

	private void BuckleDoafterEarly(Entity<BuckleComponent> entity, BuckleDoAfterEvent args, CancellableEntityEventArgs ev)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		CuffableComponent targetCuffableComp = default(CuffableComponent);
		if (args.Target.HasValue && args.Used.HasValue && ((((EntitySystem)this).TryComp<CuffableComponent>(args.Target, ref targetCuffableComp) && targetCuffableComp.CuffedHandCount > 0) || _mobState.IsIncapacitated(args.Target.Value)))
		{
			ev.Cancel();
			TryBuckle(args.Target.Value, args.User, args.Used.Value, null, popup: false);
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedInteractionSystem));
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedInputSystem));
		InitializeBuckle();
		InitializeStrap();
		InitializeInteraction();
	}

	private void InitializeInteraction()
	{
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<StrapComponent, GetVerbsEvent<InteractionVerb>>)AddStrapVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, InteractHandEvent>((ComponentEventHandler<StrapComponent, InteractHandEvent>)OnStrapInteractHand, new Type[1] { typeof(InteractionPopupSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, DragDropTargetEvent>((ComponentEventRefHandler<StrapComponent, DragDropTargetEvent>)OnStrapDragDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, CanDropTargetEvent>((ComponentEventRefHandler<StrapComponent, CanDropTargetEvent>)OnCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, InteractHandEvent>((EntityEventRefHandler<BuckleComponent, InteractHandEvent>)OnBuckleInteractHand, new Type[1] { typeof(InteractionPopupSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<BuckleComponent, GetVerbsEvent<InteractionVerb>>)AddUnbuckleVerb, (Type[])null, (Type[])null);
	}

	private void OnCanDropTarget(EntityUid uid, StrapComponent component, ref CanDropTargetEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		args.CanDrop = StrapCanDragDropOn(uid, args.User, uid, args.Dragged, component);
		args.Handled = true;
	}

	private void OnStrapDragDropTarget(EntityUid uid, StrapComponent component, ref DragDropTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (!StrapCanDragDropOn(uid, args.User, uid, args.Dragged, component) || !_rmcBuckle.CanBuckle(args.User, args.Dragged))
		{
			return;
		}
		BuckleComponent buckle2 = default(BuckleComponent);
		StrapComponent strapComp;
		if (args.Dragged == args.User)
		{
			BuckleComponent buckle = default(BuckleComponent);
			if (((EntitySystem)this).TryComp<BuckleComponent>(args.User, ref buckle))
			{
				args.Handled = TryBuckle(args.User, args.User, uid, buckle);
			}
		}
		else if (((EntitySystem)this).TryComp<BuckleComponent>(args.Dragged, ref buckle2) && CanBuckle(args.Dragged, args.User, uid, popup: true, out strapComp, buckle2))
		{
			float delay = component.BuckleDoafterTime;
			if (buckle2.BuckleDelay.HasValue)
			{
				delay = buckle2.BuckleDelay.Value;
			}
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, new BuckleDoAfterEvent(), args.Dragged, args.Dragged, uid)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				AttemptFrequency = AttemptFrequency.EveryTick
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
		}
	}

	private bool StrapCanDragDropOn(EntityUid strapUid, EntityUid userUid, EntityUid targetUid, EntityUid buckleUid, StrapComponent? strapComp = null, BuckleComponent? buckleComp = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StrapComponent>(strapUid, ref strapComp, false) || !((EntitySystem)this).Resolve<BuckleComponent>(buckleUid, ref buckleComp, false))
		{
			return false;
		}
		if (!strapComp.Enabled)
		{
			return false;
		}
		return _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(targetUid), Entity<TransformComponent>.op_Implicit(buckleUid), buckleComp.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored);
		bool Ignored(EntityUid entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (!(entity == userUid) && !(entity == buckleUid))
			{
				return entity == targetUid;
			}
			return true;
		}
	}

	private void OnStrapInteractHand(EntityUid uid, StrapComponent component, InteractHandEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (((HandledEntityEventArgs)args).Handled || !component.Enabled || !((EntitySystem)this).TryComp<BuckleComponent>(args.User, ref buckle))
		{
			return;
		}
		if (!buckle.BuckledTo.HasValue && component.BuckleOnInteractHand && StrapHasSpace(uid, buckle, component))
		{
			if (_rmcBuckle.CanBuckle(args.User, args.User, popup: false))
			{
				TryBuckle(args.User, args.User, uid, buckle);
				((HandledEntityEventArgs)args).Handled = true;
			}
			return;
		}
		EntityUid? buckledTo = buckle.BuckledTo;
		EntityUid? buckled = default(EntityUid?);
		if (buckledTo.HasValue && buckledTo.GetValueOrDefault() == uid && TryUnbuckle(args.User, args.User, buckle))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
		else if (Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)component.BuckledEntities, ref buckled) && TryUnbuckle(buckled.Value, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnBuckleInteractHand(Entity<BuckleComponent> ent, ref InteractHandEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.ClickUnbuckle && ent.Comp.BuckledTo.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = TryUnbuckle(ent, args.User, popup: true);
		}
	}

	private void AddStrapVerbs(EntityUid uid, StrapComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		if (args.Hands == null || !args.CanAccess || !args.CanInteract || !component.Enabled)
		{
			return;
		}
		foreach (EntityUid entity in component.BuckledEntities)
		{
			BuckleComponent buckledComp = ((EntitySystem)this).Comp<BuckleComponent>(entity);
			if (_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target), buckledComp.Range))
			{
				InteractionVerb verb = new InteractionVerb
				{
					Act = delegate
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_001c: Unknown result type (might be due to invalid IL or missing references)
						TryUnbuckle(entity, args.User, buckledComp);
					},
					Category = VerbCategory.Unbuckle,
					Text = ((entity == args.User) ? base.Loc.GetString("verb-self-target-pronoun") : ((string)Identity.Name(entity, (IEntityManager)(object)base.EntityManager)))
				};
				args.Verbs.Add(verb);
			}
		}
		BuckleComponent buckle = default(BuckleComponent);
		EntityUid? buckledTo;
		if (((EntitySystem)this).TryComp<BuckleComponent>(args.User, ref buckle))
		{
			buckledTo = buckle.BuckledTo;
			if ((!buckledTo.HasValue || buckledTo.GetValueOrDefault() != uid) && args.User != uid && StrapHasSpace(uid, buckle, component) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target), buckle.Range))
			{
				InteractionVerb verb2 = new InteractionVerb
				{
					Act = delegate
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						TryBuckle(args.User, args.User, args.Target, buckle);
					},
					Category = VerbCategory.Buckle,
					Text = base.Loc.GetString("verb-self-target-pronoun")
				};
				args.Verbs.Add(verb2);
			}
		}
		buckledTo = args.Using;
		if (!buckledTo.HasValue)
		{
			return;
		}
		EntityUid @using = buckledTo.GetValueOrDefault();
		BuckleComponent usingBuckle = default(BuckleComponent);
		if (((EntityUid)(ref @using)).Valid && ((EntitySystem)this).TryComp<BuckleComponent>(@using, ref usingBuckle) && StrapHasSpace(uid, usingBuckle, component) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(@using), Entity<TransformComponent>.op_Implicit(args.Target), usingBuckle.Range) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(@using), Entity<TransformComponent>.op_Implicit(args.Target), usingBuckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored))
		{
			ICommonSession val = default(ICommonSession);
			bool isPlayer = _playerManager.TryGetSessionByEntity(@using, ref val);
			InteractionVerb verb3 = new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryBuckle(@using, args.User, args.Target, usingBuckle);
				},
				Category = VerbCategory.Buckle,
				Text = Identity.Name(@using, (IEntityManager)(object)base.EntityManager),
				Priority = (isPlayer ? 1 : (-1))
			};
			args.Verbs.Add(verb3);
		}
		bool Ignored(EntityUid val2)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (!(val2 == args.User) && !(val2 == args.Target))
			{
				return val2 == @using;
			}
			return true;
		}
	}

	private void AddUnbuckleVerb(EntityUid uid, BuckleComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && component.Buckled && CanUnbuckle(Entity<BuckleComponent>.op_Implicit((uid, component)), args.User, popup: false))
		{
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryUnbuckle(uid, args.User, component);
				},
				Text = base.Loc.GetString("verb-categories-unbuckle"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/unbuckle.svg.192dpi.png"))
			};
			if (args.Target == args.User && !args.Using.HasValue)
			{
				verb.Priority = 1;
			}
			args.Verbs.Add(verb);
		}
	}

	private void InitializeStrap()
	{
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, ComponentStartup>((ComponentEventHandler<StrapComponent, ComponentStartup>)OnStrapStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, ComponentShutdown>((ComponentEventHandler<StrapComponent, ComponentShutdown>)OnStrapShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, EntityTerminatingEvent>((EntityEventRefHandler<StrapComponent, EntityTerminatingEvent>)OnStrapTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, ComponentRemove>((ComponentEventHandler<StrapComponent, ComponentRemove>)delegate(EntityUid e, StrapComponent c, ComponentRemove _)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			StrapRemoveAll(e, c);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, ContainerGettingInsertedAttemptEvent>((ComponentEventHandler<StrapComponent, ContainerGettingInsertedAttemptEvent>)OnStrapContainerGettingInsertedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, DestructionEventArgs>((ComponentEventHandler<StrapComponent, DestructionEventArgs>)delegate(EntityUid e, StrapComponent c, DestructionEventArgs _)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			StrapRemoveAll(e, c);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, BreakageEventArgs>((ComponentEventHandler<StrapComponent, BreakageEventArgs>)delegate(EntityUid e, StrapComponent c, BreakageEventArgs _)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			StrapRemoveAll(e, c);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, FoldAttemptEvent>((ComponentEventRefHandler<StrapComponent, FoldAttemptEvent>)OnAttemptFold, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, MachineDeconstructedEvent>((ComponentEventHandler<StrapComponent, MachineDeconstructedEvent>)delegate(EntityUid e, StrapComponent c, MachineDeconstructedEvent _)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			StrapRemoveAll(e, c);
		}, (Type[])null, (Type[])null);
	}

	private void OnStrapStartup(EntityUid uid, StrapComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Appearance.SetData(uid, (Enum)StrapVisuals.State, (object)(component.BuckledEntities.Count != 0), (AppearanceComponent)null);
	}

	private void OnStrapShutdown(EntityUid uid, StrapComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
		{
			StrapRemoveAll(uid, component);
		}
	}

	private void OnStrapTerminating(Entity<StrapComponent> entity, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		StrapRemoveAll(Entity<StrapComponent>.op_Implicit(entity), entity.Comp);
	}

	private void OnStrapContainerGettingInsertedAttempt(EntityUid uid, StrapComponent component, ContainerGettingInsertedAttemptEvent args)
	{
		if (((ContainerAttemptEventBase)args).Container.ID == StorageComponent.ContainerId && component.BuckledEntities.Count != 0)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnAttemptFold(EntityUid uid, StrapComponent component, ref FoldAttemptEvent args)
	{
		if (!args.Cancelled)
		{
			args.Cancelled = component.BuckledEntities.Count != 0;
		}
	}

	private void StrapRemoveAll(EntityUid uid, StrapComponent strapComp)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = strapComp.BuckledEntities.ToArray();
		foreach (EntityUid entity in array)
		{
			Unbuckle(Entity<BuckleComponent>.op_Implicit(entity), entity);
		}
	}

	private bool StrapHasSpace(EntityUid strapUid, BuckleComponent buckleComp, StrapComponent? strapComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StrapComponent>(strapUid, ref strapComp, false))
		{
			return false;
		}
		int avail = strapComp.Size;
		foreach (EntityUid buckle in strapComp.BuckledEntities)
		{
			avail -= ((EntitySystem)this).CompOrNull<BuckleComponent>(buckle)?.Size ?? 0;
		}
		return avail >= buckleComp.Size;
	}

	public void StrapSetEnabled(EntityUid strapUid, bool enabled, StrapComponent? strapComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StrapComponent>(strapUid, ref strapComp, false) && strapComp.Enabled != enabled)
		{
			strapComp.Enabled = enabled;
			((EntitySystem)this).Dirty(strapUid, (IComponent)(object)strapComp, (MetaDataComponent)null);
			if (!enabled)
			{
				StrapRemoveAll(strapUid, strapComp);
			}
		}
	}
}
