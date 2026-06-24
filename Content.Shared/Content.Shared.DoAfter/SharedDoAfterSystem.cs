using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Content.Shared._RMC14.DoAfter;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Gravity;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.DoAfter;

public abstract class SharedDoAfterSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming GameTiming;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TagSystem _tag;

	private static readonly TimeSpan ExcessTime = TimeSpan.FromSeconds(0.5);

	private static readonly ProtoId<TagPrototype> InstantDoAftersTag = ProtoId<TagPrototype>.op_Implicit("InstantDoAfters");

	[Dependency]
	private IDynamicTypeFactory _factory;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCDoAfterSystem _rmcDoAfter;

	private DoAfter[] _doAfters = Array.Empty<DoAfter>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DoAfterComponent, DamageChangedEvent>((ComponentEventHandler<DoAfterComponent, DamageChangedEvent>)OnDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoAfterComponent, EntityUnpausedEvent>((ComponentEventRefHandler<DoAfterComponent, EntityUnpausedEvent>)OnUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoAfterComponent, ComponentGetState>((ComponentEventRefHandler<DoAfterComponent, ComponentGetState>)OnDoAfterGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoAfterComponent, ComponentHandleState>((ComponentEventRefHandler<DoAfterComponent, ComponentHandleState>)OnDoAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnUnpaused(EntityUid uid, DoAfterComponent component, ref EntityUnpausedEvent args)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		foreach (DoAfter doAfter in component.DoAfters.Values)
		{
			doAfter.StartTime += args.PausedTime;
			if (doAfter.CancelledTime.HasValue)
			{
				doAfter.CancelledTime = doAfter.CancelledTime.Value + args.PausedTime;
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnDamage(EntityUid uid, DoAfterComponent component, DamageChangedEvent args)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (!args.InterruptsDoAfters || !args.DamageIncreased || args.DamageDelta == null || GameTiming.ApplyingState)
		{
			return;
		}
		FixedPoint2 delta = args.DamageDelta.GetTotal();
		bool dirty = false;
		foreach (DoAfter doAfter in component.DoAfters.Values)
		{
			if (doAfter.Args.BreakOnDamage && delta >= doAfter.Args.DamageThreshold)
			{
				InternalCancel(doAfter, component);
				dirty = true;
			}
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void RaiseDoAfterEvents(DoAfter doAfter, DoAfterComponent component)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent ev = doAfter.Args.Event;
		((HandledEntityEventArgs)ev).Handled = false;
		ev.Repeat = false;
		ev.DoAfter = doAfter;
		if (((EntitySystem)this).Exists(doAfter.Args.EventTarget))
		{
			((EntitySystem)this).RaiseLocalEvent(doAfter.Args.EventTarget.Value, (object)ev, doAfter.Args.Broadcast);
		}
		else if (doAfter.Args.Broadcast)
		{
			((EntitySystem)this).RaiseLocalEvent((object)ev);
		}
		if (component.AwaitedDoAfters.Remove(doAfter.Index, out TaskCompletionSource<DoAfterStatus> tcs))
		{
			tcs.SetResult(doAfter.Cancelled ? DoAfterStatus.Cancelled : DoAfterStatus.Finished);
		}
	}

	private void OnDoAfterGetState(EntityUid uid, DoAfterComponent comp, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new DoAfterComponentState((IEntityManager)(object)base.EntityManager, comp);
	}

	private void OnDoAfterHandleState(EntityUid uid, DoAfterComponent comp, ref ComponentHandleState args)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is DoAfterComponentState state))
		{
			return;
		}
		comp.DoAfters.Clear();
		foreach (KeyValuePair<ushort, DoAfter> doAfter2 in state.DoAfters)
		{
			doAfter2.Deconstruct(out var key, out var value);
			ushort id = key;
			DoAfter doAfter = value;
			DoAfter newDoAfter = new DoAfter((IEntityManager)(object)base.EntityManager, doAfter);
			comp.DoAfters.Add(id, newDoAfter);
			newDoAfter.UserPosition = ((EntitySystem)this).EnsureCoordinates<DoAfterComponent>(newDoAfter.NetUserPosition, uid);
			newDoAfter.InitialItem = ((EntitySystem)this).EnsureEntity<DoAfterComponent>(newDoAfter.NetInitialItem, uid);
			DoAfterArgs doAfterArgs = newDoAfter.Args;
			doAfterArgs.Target = ((EntitySystem)this).EnsureEntity<DoAfterComponent>(doAfterArgs.NetTarget, uid);
			doAfterArgs.Used = ((EntitySystem)this).EnsureEntity<DoAfterComponent>(doAfterArgs.NetUsed, uid);
			doAfterArgs.User = ((EntitySystem)this).EnsureEntity<DoAfterComponent>(doAfterArgs.NetUser, uid);
			doAfterArgs.EventTarget = ((EntitySystem)this).EnsureEntity<DoAfterComponent>(doAfterArgs.NetEventTarget, uid);
		}
		comp.NextId = state.NextId;
		if (comp.DoAfters.Count == 0)
		{
			((EntitySystem)this).RemCompDeferred<ActiveDoAfterComponent>(uid);
		}
		else
		{
			((EntitySystem)this).EnsureComp<ActiveDoAfterComponent>(uid);
		}
	}

	[Obsolete("Use the synchronous version instead.")]
	public async Task<DoAfterStatus> WaitDoAfter(DoAfterArgs doAfter, DoAfterComponent? component = null)
	{
		if (!((EntitySystem)this).Resolve<DoAfterComponent>(doAfter.User, ref component, true))
		{
			return DoAfterStatus.Cancelled;
		}
		if (!TryStartDoAfter(doAfter, out var id, component))
		{
			return DoAfterStatus.Cancelled;
		}
		if (doAfter.Delay <= TimeSpan.Zero)
		{
			((EntitySystem)this).Log.Warning("Awaited instant DoAfters are not supported fully supported");
			return DoAfterStatus.Finished;
		}
		TaskCompletionSource<DoAfterStatus> tcs = new TaskCompletionSource<DoAfterStatus>();
		component.AwaitedDoAfters.Add(id.Value.Index, tcs);
		return await tcs.Task;
	}

	public bool TryStartDoAfter(DoAfterArgs args, DoAfterComponent? component = null)
	{
		DoAfterId? id;
		return TryStartDoAfter(args, out id, component);
	}

	public bool TryStartDoAfter(DoAfterArgs args, [NotNullWhen(true)] out DoAfterId? id, DoAfterComponent? comp = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoAfterComponent>(args.User, ref comp, true))
		{
			((EntitySystem)this).Log.Error($"Attempting to start a doAfter with invalid user: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User))}.");
			id = null;
			return false;
		}
		if (!ProcessDuplicates(args, comp))
		{
			id = null;
			return false;
		}
		id = new DoAfterId(args.User, comp.NextId++);
		DoAfter doAfter = new DoAfter(id.Value.Index, args, GameTiming.CurTime);
		args.NetTarget = ((EntitySystem)this).GetNetEntity(args.Target, (MetaDataComponent)null);
		args.NetUsed = ((EntitySystem)this).GetNetEntity(args.Used, (MetaDataComponent)null);
		args.NetUser = ((EntitySystem)this).GetNetEntity(args.User, (MetaDataComponent)null);
		args.NetEventTarget = ((EntitySystem)this).GetNetEntity(args.EventTarget, (MetaDataComponent)null);
		if (args.BreakOnMove)
		{
			doAfter.UserPosition = ((EntitySystem)this).Transform(args.User).Coordinates;
		}
		if (args.Target.HasValue && args.BreakOnMove)
		{
			EntityCoordinates targetPosition = ((EntitySystem)this).Transform(args.Target.Value).Coordinates;
			((EntityCoordinates)(ref doAfter.UserPosition)).TryDistance((IEntityManager)(object)base.EntityManager, targetPosition, ref doAfter.TargetDistance);
		}
		doAfter.NetUserPosition = ((EntitySystem)this).GetNetCoordinates(doAfter.UserPosition, (MetaDataComponent)null);
		if (args.NeedHand && (args.BreakOnHandChange || args.BreakOnDropItem))
		{
			HandsComponent handsComponent = default(HandsComponent);
			if (!((EntitySystem)this).TryComp<HandsComponent>(args.User, ref handsComponent))
			{
				return false;
			}
			doAfter.InitialHand = handsComponent.ActiveHandId;
			doAfter.InitialItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit((args.User, handsComponent)));
		}
		doAfter.NetInitialItem = ((EntitySystem)this).GetNetEntity(doAfter.InitialItem, (MetaDataComponent)null);
		if (ShouldCancel(doAfter, ((EntitySystem)this).GetEntityQuery<TransformComponent>(), ((EntitySystem)this).GetEntityQuery<HandsComponent>()))
		{
			return false;
		}
		if (args.AttemptFrequency == AttemptFrequency.StartAndEnd && !TryAttemptEvent(doAfter))
		{
			return false;
		}
		if (args.Delay <= TimeSpan.Zero || _tag.HasTag(args.User, InstantDoAftersTag))
		{
			RaiseDoAfterEvents(doAfter, comp);
			return true;
		}
		comp.DoAfters.Add(doAfter.Index, doAfter);
		((EntitySystem)this).EnsureComp<ActiveDoAfterComponent>(args.User);
		((EntitySystem)this).Dirty(args.User, (IComponent)(object)comp, (MetaDataComponent)null);
		args.Event.DoAfter = doAfter;
		return true;
	}

	private bool ProcessDuplicates(DoAfterArgs args, DoAfterComponent component)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		bool blocked = false;
		foreach (DoAfter existing in component.DoAfters.Values)
		{
			if (!existing.Cancelled && !existing.Completed && IsDuplicate(existing.Args, args))
			{
				blocked = blocked | args.BlockDuplicate | existing.Args.BlockDuplicate;
				if (args.CancelDuplicate || existing.Args.CancelDuplicate)
				{
					Cancel(args.User, existing.Index, component);
				}
			}
		}
		return !blocked;
	}

	private bool IsDuplicate(DoAfterArgs args, DoAfterArgs otherArgs)
	{
		if (IsDuplicate(args, otherArgs, args.DuplicateCondition))
		{
			return true;
		}
		if (args.DuplicateCondition == otherArgs.DuplicateCondition)
		{
			return false;
		}
		return IsDuplicate(args, otherArgs, otherArgs.DuplicateCondition);
	}

	private bool IsDuplicate(DoAfterArgs args, DoAfterArgs otherArgs, DuplicateConditions conditions)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if ((conditions & DuplicateConditions.SameTarget) != DuplicateConditions.None)
		{
			EntityUid? target = args.Target;
			EntityUid? target2 = otherArgs.Target;
			if (target.HasValue != target2.HasValue || (target.HasValue && target.GetValueOrDefault() != target2.GetValueOrDefault()))
			{
				return false;
			}
		}
		if ((conditions & DuplicateConditions.SameTool) != DuplicateConditions.None)
		{
			EntityUid? target2 = args.Used;
			EntityUid? target = otherArgs.Used;
			if (target2.HasValue != target.HasValue || (target2.HasValue && target2.GetValueOrDefault() != target.GetValueOrDefault()))
			{
				return false;
			}
		}
		if ((conditions & DuplicateConditions.SameEvent) != DuplicateConditions.None && !args.Event.IsDuplicate(otherArgs.Event))
		{
			return false;
		}
		return true;
	}

	public void Cancel(DoAfterId? id, DoAfterComponent? comp = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (id.HasValue)
		{
			Cancel(id.Value.Uid, id.Value.Index, comp);
		}
	}

	public void Cancel(EntityUid entity, ushort id, DoAfterComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoAfterComponent>(entity, ref comp, false))
		{
			if (!comp.DoAfters.TryGetValue(id, out DoAfter doAfter))
			{
				((EntitySystem)this).Log.Error($"Attempted to cancel do after with an invalid id ({id}) on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity))}");
			}
			else
			{
				InternalCancel(doAfter, comp);
				((EntitySystem)this).Dirty(entity, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	private void InternalCancel(DoAfter doAfter, DoAfterComponent component)
	{
		if (!doAfter.Cancelled && !doAfter.Completed)
		{
			doAfter.CancelledTime = GameTiming.CurTime;
			RaiseDoAfterEvents(doAfter, component);
		}
	}

	public DoAfterStatus GetStatus(DoAfterId? id, DoAfterComponent? comp = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (id.HasValue)
		{
			return GetStatus(id.Value.Uid, id.Value.Index, comp);
		}
		return DoAfterStatus.Invalid;
	}

	public DoAfterStatus GetStatus(EntityUid entity, ushort id, DoAfterComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoAfterComponent>(entity, ref comp, false))
		{
			return DoAfterStatus.Invalid;
		}
		if (!comp.DoAfters.TryGetValue(id, out DoAfter doAfter))
		{
			return DoAfterStatus.Invalid;
		}
		if (doAfter.Cancelled)
		{
			return DoAfterStatus.Cancelled;
		}
		if (!doAfter.Completed)
		{
			return DoAfterStatus.Running;
		}
		return DoAfterStatus.Finished;
	}

	public bool IsRunning(DoAfterId? id, DoAfterComponent? comp = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!id.HasValue)
		{
			return false;
		}
		return GetStatus(id.Value.Uid, id.Value.Index, comp) == DoAfterStatus.Running;
	}

	public bool IsRunning(EntityUid entity, ushort id, DoAfterComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetStatus(entity, id, comp) == DoAfterStatus.Running;
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan time = GameTiming.CurTime;
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		EntityQuery<HandsComponent> handsQuery = ((EntitySystem)this).GetEntityQuery<HandsComponent>();
		EntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent>();
		EntityUid uid = default(EntityUid);
		ActiveDoAfterComponent active = default(ActiveDoAfterComponent);
		DoAfterComponent comp = default(DoAfterComponent);
		while (enumerator.MoveNext(ref uid, ref active, ref comp))
		{
			Update(uid, active, comp, time, xformQuery, handsQuery);
		}
	}

	protected void Update(EntityUid uid, ActiveDoAfterComponent active, DoAfterComponent comp, TimeSpan time, EntityQuery<TransformComponent> xformQuery, EntityQuery<HandsComponent> handsQuery)
	{
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		bool dirty = false;
		Dictionary<ushort, DoAfter>.ValueCollection values = comp.DoAfters.Values;
		int count = values.Count;
		if (_doAfters.Length < count)
		{
			_doAfters = new DoAfter[count];
		}
		values.CopyTo(_doAfters, 0);
		TransformComponent targetXform = default(TransformComponent);
		for (int i = 0; i < count; i++)
		{
			DoAfter doAfter = _doAfters[i];
			if (doAfter.CancelledTime.HasValue)
			{
				if (time - doAfter.CancelledTime.Value > ExcessTime)
				{
					comp.DoAfters.Remove(doAfter.Index);
					dirty = true;
				}
				continue;
			}
			if (doAfter.Completed)
			{
				if (time - doAfter.StartTime > doAfter.Args.Delay + ExcessTime)
				{
					comp.DoAfters.Remove(doAfter.Index);
					dirty = true;
				}
				continue;
			}
			if (!doAfter.Completed && !doAfter.Cancelled && doAfter.Args.TargetEffect.HasValue && (!doAfter.LastEffectSpawnTime.HasValue || time - doAfter.LastEffectSpawnTime.Value >= TimeSpan.FromSeconds(1L)) && xformQuery.TryGetComponent(doAfter.Args.Target, ref targetXform))
			{
				if (_net.IsServer)
				{
					EntProtoId? targetEffect = doAfter.Args.TargetEffect;
					((EntitySystem)this).SpawnAttachedTo(targetEffect.HasValue ? EntProtoId.op_Implicit(targetEffect.GetValueOrDefault()) : null, targetXform.Coordinates, (ComponentRegistry)null, default(Angle));
				}
				doAfter.LastEffectSpawnTime = time;
			}
			if (ShouldCancel(doAfter, xformQuery, handsQuery))
			{
				InternalCancel(doAfter, comp);
				dirty = true;
			}
			else if (_rmcDoAfter.ShouldCancel(doAfter))
			{
				InternalCancel(doAfter, comp);
				dirty = true;
			}
			else if (time - doAfter.StartTime >= doAfter.Args.Delay)
			{
				TryComplete(doAfter, comp);
				dirty = true;
			}
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
		if (comp.DoAfters.Count == 0)
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)active);
		}
	}

	private bool TryAttemptEvent(DoAfter doAfter)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		DoAfterArgs args = doAfter.Args;
		Func<bool>? extraCheck = args.ExtraCheck;
		if (extraCheck != null && !extraCheck())
		{
			return false;
		}
		if (doAfter.AttemptEvent == null)
		{
			Type evType = typeof(DoAfterAttemptEvent<>).MakeGenericType(((object)args.Event).GetType());
			doAfter.AttemptEvent = _factory.CreateInstance(evType, new object[2] { doAfter, args.Event }, false, true);
		}
		args.Event.DoAfter = doAfter;
		if (args.EventTarget.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent(args.EventTarget.Value, doAfter.AttemptEvent, args.Broadcast);
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent(doAfter.AttemptEvent);
		}
		CancellableEntityEventArgs ev = (CancellableEntityEventArgs)doAfter.AttemptEvent;
		if (!ev.Cancelled)
		{
			return true;
		}
		ev.Uncancel();
		return false;
	}

	private void TryComplete(DoAfter doAfter, DoAfterComponent component)
	{
		if (doAfter.Cancelled || doAfter.Completed)
		{
			return;
		}
		if (doAfter.Args.AttemptFrequency == AttemptFrequency.StartAndEnd && !TryAttemptEvent(doAfter))
		{
			InternalCancel(doAfter, component);
			return;
		}
		doAfter.Completed = true;
		RaiseDoAfterEvents(doAfter, component);
		if (doAfter.Args.Event.Repeat)
		{
			doAfter.StartTime = GameTiming.CurTime;
			doAfter.Completed = false;
		}
	}

	private bool ShouldCancel(DoAfter doAfter, EntityQuery<TransformComponent> xformQuery, EntityQuery<HandsComponent> handsQuery)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs args = doAfter.Args;
		EntityUid? used = args.Used;
		if (used.HasValue)
		{
			EntityUid used2 = used.GetValueOrDefault();
			if (!xformQuery.HasComponent(used2))
			{
				return true;
			}
		}
		used = args.EventTarget;
		if (used.HasValue)
		{
			EntityUid eventTarget = used.GetValueOrDefault();
			if (((EntityUid)(ref eventTarget)).Valid && !xformQuery.HasComponent(eventTarget))
			{
				return true;
			}
		}
		TransformComponent userXform = default(TransformComponent);
		if (!xformQuery.TryGetComponent(args.User, ref userXform))
		{
			return true;
		}
		TransformComponent targetXform = null;
		used = args.Target;
		if (used.HasValue)
		{
			EntityUid target = used.GetValueOrDefault();
			if (!xformQuery.TryGetComponent(target, ref targetXform))
			{
				return true;
			}
		}
		TransformComponent usedXform = null;
		used = args.Used;
		if (used.HasValue)
		{
			EntityUid @using = used.GetValueOrDefault();
			if (!xformQuery.TryGetComponent(@using, ref usedXform))
			{
				return true;
			}
		}
		if (args.BreakOnMove && (args.BreakOnWeightlessMove || !_gravity.IsWeightless(args.User, null, userXform)))
		{
			if (!_transform.InRange(userXform.Coordinates, doAfter.UserPosition, args.MovementThreshold))
			{
				return true;
			}
			if (targetXform != null)
			{
				EntityCoordinates coordinates = targetXform.Coordinates;
				float distance = default(float);
				if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, userXform.Coordinates, ref distance) && Math.Abs(distance - doAfter.TargetDistance) > args.MovementThreshold)
				{
					return true;
				}
			}
		}
		if (args.Target.HasValue)
		{
			if (args.DistanceThreshold.HasValue)
			{
				if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target.Value), args.DistanceThreshold.Value))
				{
					return true;
				}
			}
			else if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target.Value)))
			{
				return true;
			}
		}
		if (args.Used.HasValue)
		{
			if (args.DistanceThreshold.HasValue)
			{
				if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Used.Value), args.DistanceThreshold.Value))
				{
					return true;
				}
			}
			else if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Used.Value)))
			{
				return true;
			}
		}
		if (args.AttemptFrequency == AttemptFrequency.EveryTick && !TryAttemptEvent(doAfter))
		{
			return true;
		}
		if (args.NeedHand)
		{
			HandsComponent hands = default(HandsComponent);
			if (!handsQuery.TryGetComponent(args.User, ref hands) || hands.Count == 0)
			{
				return true;
			}
			if (args.BreakOnDropItem && doAfter.InitialItem.HasValue && !_hands.IsHolding(Entity<HandsComponent>.op_Implicit((args.User, hands)), doAfter.InitialItem))
			{
				return true;
			}
			if (args.BreakOnHandChange && hands.ActiveHandId != doAfter.InitialHand)
			{
				return true;
			}
		}
		if (args.RequireCanInteract && !_actionBlocker.CanInteract(args.User, args.Target))
		{
			return true;
		}
		return false;
	}
}
