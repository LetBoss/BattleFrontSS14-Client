// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.SharedDoAfterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

#nullable enable
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
  private static readonly ProtoId<TagPrototype> InstantDoAftersTag = (ProtoId<TagPrototype>) "InstantDoAfters";
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
  private Content.Shared.DoAfter.DoAfter[] _doAfters;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoAfterComponent, DamageChangedEvent>(new ComponentEventHandler<DoAfterComponent, DamageChangedEvent>((object) this, __methodptr(OnDamage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoAfterComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DoAfterComponent, EntityUnpausedEvent>((object) this, __methodptr(OnUnpaused)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoAfterComponent, ComponentGetState>(new ComponentEventRefHandler<DoAfterComponent, ComponentGetState>((object) this, __methodptr(OnDoAfterGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoAfterComponent, ComponentHandleState>(new ComponentEventRefHandler<DoAfterComponent, ComponentHandleState>((object) this, __methodptr(OnDoAfterHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnUnpaused(EntityUid uid, DoAfterComponent component, ref EntityUnpausedEvent args)
  {
    foreach (Content.Shared.DoAfter.DoAfter doAfter in component.DoAfters.Values)
    {
      doAfter.StartTime += args.PausedTime;
      if (doAfter.CancelledTime.HasValue)
        doAfter.CancelledTime = new TimeSpan?(doAfter.CancelledTime.Value + args.PausedTime);
    }
    ((EntitySystem) this).Dirty(uid, (IComponent) component);
  }

  private void OnDamage(EntityUid uid, DoAfterComponent component, DamageChangedEvent args)
  {
    if (!args.InterruptsDoAfters || !args.DamageIncreased || args.DamageDelta == null || this.GameTiming.ApplyingState)
      return;
    FixedPoint2 total = args.DamageDelta.GetTotal();
    bool flag = false;
    foreach (Content.Shared.DoAfter.DoAfter doAfter in component.DoAfters.Values)
    {
      if (doAfter.Args.BreakOnDamage && total >= doAfter.Args.DamageThreshold)
      {
        this.InternalCancel(doAfter, component);
        flag = true;
      }
    }
    if (!flag)
      return;
    ((EntitySystem) this).Dirty(uid, (IComponent) component);
  }

  private void RaiseDoAfterEvents(Content.Shared.DoAfter.DoAfter doAfter, DoAfterComponent component)
  {
    DoAfterEvent doAfterEvent = doAfter.Args.Event;
    doAfterEvent.Handled = false;
    doAfterEvent.Repeat = false;
    doAfterEvent.DoAfter = doAfter;
    if (((EntitySystem) this).Exists(doAfter.Args.EventTarget))
      ((EntitySystem) this).RaiseLocalEvent(doAfter.Args.EventTarget.Value, (object) doAfterEvent, doAfter.Args.Broadcast);
    else if (doAfter.Args.Broadcast)
      ((EntitySystem) this).RaiseLocalEvent((object) doAfterEvent);
    TaskCompletionSource<DoAfterStatus> completionSource;
    if (!component.AwaitedDoAfters.Remove(doAfter.Index, out completionSource))
      return;
    completionSource.SetResult(doAfter.Cancelled ? DoAfterStatus.Cancelled : DoAfterStatus.Finished);
  }

  private void OnDoAfterGetState(EntityUid uid, DoAfterComponent comp, ref ComponentGetState args)
  {
    args.State = (IComponentState) new DoAfterComponentState((IEntityManager) ((EntitySystem) this).EntityManager, comp);
  }

  private void OnDoAfterHandleState(
    EntityUid uid,
    DoAfterComponent comp,
    ref ComponentHandleState args)
  {
    if (!(args.Current is DoAfterComponentState current))
      return;
    comp.DoAfters.Clear();
    foreach ((ushort key, Content.Shared.DoAfter.DoAfter other) in current.DoAfters)
    {
      Content.Shared.DoAfter.DoAfter doAfter = new Content.Shared.DoAfter.DoAfter((IEntityManager) ((EntitySystem) this).EntityManager, other);
      comp.DoAfters.Add(key, doAfter);
      doAfter.UserPosition = ((EntitySystem) this).EnsureCoordinates<DoAfterComponent>(doAfter.NetUserPosition, uid);
      doAfter.InitialItem = ((EntitySystem) this).EnsureEntity<DoAfterComponent>(doAfter.NetInitialItem, uid);
      DoAfterArgs args1 = doAfter.Args;
      args1.Target = ((EntitySystem) this).EnsureEntity<DoAfterComponent>(args1.NetTarget, uid);
      args1.Used = ((EntitySystem) this).EnsureEntity<DoAfterComponent>(args1.NetUsed, uid);
      args1.User = ((EntitySystem) this).EnsureEntity<DoAfterComponent>(args1.NetUser, uid);
      args1.EventTarget = ((EntitySystem) this).EnsureEntity<DoAfterComponent>(args1.NetEventTarget, uid);
    }
    comp.NextId = current.NextId;
    if (comp.DoAfters.Count == 0)
      ((EntitySystem) this).RemCompDeferred<ActiveDoAfterComponent>(uid);
    else
      ((EntitySystem) this).EnsureComp<ActiveDoAfterComponent>(uid);
  }

  [Obsolete("Use the synchronous version instead.")]
  public async Task<DoAfterStatus> WaitDoAfter(DoAfterArgs doAfter, DoAfterComponent? component = null)
  {
    SharedDoAfterSystem sharedDoAfterSystem = this;
    DoAfterId? id;
    if (!((EntitySystem) sharedDoAfterSystem).Resolve<DoAfterComponent>(doAfter.User, ref component) || !sharedDoAfterSystem.TryStartDoAfter(doAfter, out id, component))
      return DoAfterStatus.Cancelled;
    if (doAfter.Delay <= TimeSpan.Zero)
    {
      ((EntitySystem) sharedDoAfterSystem).Log.Warning("Awaited instant DoAfters are not supported fully supported");
      return DoAfterStatus.Finished;
    }
    TaskCompletionSource<DoAfterStatus> completionSource = new TaskCompletionSource<DoAfterStatus>();
    component.AwaitedDoAfters.Add(id.Value.Index, completionSource);
    return await completionSource.Task;
  }

  public bool TryStartDoAfter(DoAfterArgs args, DoAfterComponent? component = null)
  {
    return this.TryStartDoAfter(args, out DoAfterId? _, component);
  }

  public bool TryStartDoAfter(DoAfterArgs args, [NotNullWhen(true)] out DoAfterId? id, DoAfterComponent? comp = null)
  {
    if (!((EntitySystem) this).Resolve<DoAfterComponent>(args.User, ref comp))
    {
      ((EntitySystem) this).Log.Error($"Attempting to start a doAfter with invalid user: {((EntitySystem) this).ToPrettyString((Entity<MetaDataComponent>) args.User)}.");
      id = new DoAfterId?();
      return false;
    }
    if (!this.ProcessDuplicates(args, comp))
    {
      id = new DoAfterId?();
      return false;
    }
    id = new DoAfterId?(new DoAfterId(args.User, comp.NextId++));
    Content.Shared.DoAfter.DoAfter doAfter = new Content.Shared.DoAfter.DoAfter(id.Value.Index, args, this.GameTiming.CurTime);
    args.NetTarget = ((EntitySystem) this).GetNetEntity(args.Target);
    args.NetUsed = ((EntitySystem) this).GetNetEntity(args.Used);
    args.NetUser = ((EntitySystem) this).GetNetEntity(args.User);
    args.NetEventTarget = ((EntitySystem) this).GetNetEntity(args.EventTarget);
    if (args.BreakOnMove)
      doAfter.UserPosition = ((EntitySystem) this).Transform(args.User).Coordinates;
    if (args.Target.HasValue && args.BreakOnMove)
    {
      EntityCoordinates coordinates = ((EntitySystem) this).Transform(args.Target.Value).Coordinates;
      doAfter.UserPosition.TryDistance((IEntityManager) ((EntitySystem) this).EntityManager, coordinates, out doAfter.TargetDistance);
    }
    doAfter.NetUserPosition = ((EntitySystem) this).GetNetCoordinates(doAfter.UserPosition);
    if (args.NeedHand && (args.BreakOnHandChange || args.BreakOnDropItem))
    {
      HandsComponent comp1;
      if (!((EntitySystem) this).TryComp<HandsComponent>(args.User, out comp1))
        return false;
      doAfter.InitialHand = comp1.ActiveHandId;
      doAfter.InitialItem = this._hands.GetActiveItem((Entity<HandsComponent>) (args.User, comp1));
    }
    doAfter.NetInitialItem = ((EntitySystem) this).GetNetEntity(doAfter.InitialItem);
    if (this.ShouldCancel(doAfter, ((EntitySystem) this).GetEntityQuery<TransformComponent>(), ((EntitySystem) this).GetEntityQuery<HandsComponent>()) || args.AttemptFrequency == AttemptFrequency.StartAndEnd && !this.TryAttemptEvent(doAfter))
      return false;
    if (args.Delay <= TimeSpan.Zero || this._tag.HasTag(args.User, SharedDoAfterSystem.InstantDoAftersTag))
    {
      this.RaiseDoAfterEvents(doAfter, comp);
      return true;
    }
    comp.DoAfters.Add(doAfter.Index, doAfter);
    ((EntitySystem) this).EnsureComp<ActiveDoAfterComponent>(args.User);
    ((EntitySystem) this).Dirty(args.User, (IComponent) comp);
    args.Event.DoAfter = doAfter;
    return true;
  }

  private bool ProcessDuplicates(DoAfterArgs args, DoAfterComponent component)
  {
    bool flag = false;
    foreach (Content.Shared.DoAfter.DoAfter doAfter in component.DoAfters.Values)
    {
      if (!doAfter.Cancelled && !doAfter.Completed && this.IsDuplicate(doAfter.Args, args))
      {
        flag = flag | args.BlockDuplicate | doAfter.Args.BlockDuplicate;
        if (args.CancelDuplicate || doAfter.Args.CancelDuplicate)
          this.Cancel(args.User, doAfter.Index, component);
      }
    }
    return !flag;
  }

  private bool IsDuplicate(DoAfterArgs args, DoAfterArgs otherArgs)
  {
    if (this.IsDuplicate(args, otherArgs, args.DuplicateCondition))
      return true;
    return args.DuplicateCondition != otherArgs.DuplicateCondition && this.IsDuplicate(args, otherArgs, otherArgs.DuplicateCondition);
  }

  private bool IsDuplicate(DoAfterArgs args, DoAfterArgs otherArgs, DuplicateConditions conditions)
  {
    EntityUid? nullable;
    if ((conditions & DuplicateConditions.SameTarget) != DuplicateConditions.None)
    {
      nullable = args.Target;
      EntityUid? target = otherArgs.Target;
      if ((nullable.HasValue == target.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != target.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        return false;
    }
    if ((conditions & DuplicateConditions.SameTool) != DuplicateConditions.None)
    {
      EntityUid? used = args.Used;
      nullable = otherArgs.Used;
      if ((used.HasValue == nullable.HasValue ? (used.HasValue ? (used.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        return false;
    }
    return (conditions & DuplicateConditions.SameEvent) == DuplicateConditions.None || args.Event.IsDuplicate(otherArgs.Event);
  }

  public void Cancel(DoAfterId? id, DoAfterComponent? comp = null)
  {
    if (!id.HasValue)
      return;
    this.Cancel(id.Value.Uid, id.Value.Index, comp);
  }

  public void Cancel(EntityUid entity, ushort id, DoAfterComponent? comp = null)
  {
    if (!((EntitySystem) this).Resolve<DoAfterComponent>(entity, ref comp, false))
      return;
    Content.Shared.DoAfter.DoAfter doAfter;
    if (!comp.DoAfters.TryGetValue(id, out doAfter))
    {
      ((EntitySystem) this).Log.Error($"Attempted to cancel do after with an invalid id ({id}) on entity {((EntitySystem) this).ToPrettyString((Entity<MetaDataComponent>) entity)}");
    }
    else
    {
      this.InternalCancel(doAfter, comp);
      ((EntitySystem) this).Dirty(entity, (IComponent) comp);
    }
  }

  private void InternalCancel(Content.Shared.DoAfter.DoAfter doAfter, DoAfterComponent component)
  {
    if (doAfter.Cancelled || doAfter.Completed)
      return;
    doAfter.CancelledTime = new TimeSpan?(this.GameTiming.CurTime);
    this.RaiseDoAfterEvents(doAfter, component);
  }

  public DoAfterStatus GetStatus(DoAfterId? id, DoAfterComponent? comp = null)
  {
    return id.HasValue ? this.GetStatus(id.Value.Uid, id.Value.Index, comp) : DoAfterStatus.Invalid;
  }

  public DoAfterStatus GetStatus(EntityUid entity, ushort id, DoAfterComponent? comp = null)
  {
    Content.Shared.DoAfter.DoAfter doAfter;
    if (!((EntitySystem) this).Resolve<DoAfterComponent>(entity, ref comp, false) || !comp.DoAfters.TryGetValue(id, out doAfter))
      return DoAfterStatus.Invalid;
    if (doAfter.Cancelled)
      return DoAfterStatus.Cancelled;
    return !doAfter.Completed ? DoAfterStatus.Running : DoAfterStatus.Finished;
  }

  public bool IsRunning(DoAfterId? id, DoAfterComponent? comp = null)
  {
    if (!id.HasValue)
      return false;
    DoAfterId doAfterId = id.Value;
    EntityUid uid = doAfterId.Uid;
    doAfterId = id.Value;
    int index = (int) doAfterId.Index;
    DoAfterComponent comp1 = comp;
    return this.GetStatus(uid, (ushort) index, comp1) == DoAfterStatus.Running;
  }

  public bool IsRunning(EntityUid entity, ushort id, DoAfterComponent? comp = null)
  {
    return this.GetStatus(entity, id, comp) == DoAfterStatus.Running;
  }

  public virtual void Update(float frameTime)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((EntitySystem) this).Update(frameTime));
    TimeSpan curTime = this.GameTiming.CurTime;
    EntityQuery<TransformComponent> entityQuery1 = ((EntitySystem) this).GetEntityQuery<TransformComponent>();
    EntityQuery<HandsComponent> entityQuery2 = ((EntitySystem) this).GetEntityQuery<HandsComponent>();
    EntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent>();
    EntityUid uid;
    ActiveDoAfterComponent comp1;
    DoAfterComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
      this.Update(uid, comp1, comp2, curTime, entityQuery1, entityQuery2);
  }

  protected void Update(
    EntityUid uid,
    ActiveDoAfterComponent active,
    DoAfterComponent comp,
    TimeSpan time,
    EntityQuery<TransformComponent> xformQuery,
    EntityQuery<HandsComponent> handsQuery)
  {
    bool flag = false;
    Dictionary<ushort, Content.Shared.DoAfter.DoAfter>.ValueCollection values = comp.DoAfters.Values;
    int count = values.Count;
    if (this._doAfters.Length < count)
      this._doAfters = new Content.Shared.DoAfter.DoAfter[count];
    values.CopyTo(this._doAfters, 0);
    for (int index = 0; index < count; ++index)
    {
      Content.Shared.DoAfter.DoAfter doAfter = this._doAfters[index];
      if (doAfter.CancelledTime.HasValue)
      {
        if (time - doAfter.CancelledTime.Value > SharedDoAfterSystem.ExcessTime)
        {
          comp.DoAfters.Remove(doAfter.Index);
          flag = true;
        }
      }
      else if (doAfter.Completed)
      {
        if (time - doAfter.StartTime > doAfter.Args.Delay + SharedDoAfterSystem.ExcessTime)
        {
          comp.DoAfters.Remove(doAfter.Index);
          flag = true;
        }
      }
      else
      {
        TransformComponent component;
        if (!doAfter.Completed && !doAfter.Cancelled && doAfter.Args.TargetEffect.HasValue && (!doAfter.LastEffectSpawnTime.HasValue || time - doAfter.LastEffectSpawnTime.Value >= TimeSpan.FromSeconds(1L)) && xformQuery.TryGetComponent(doAfter.Args.Target, out component))
        {
          if (this._net.IsServer)
          {
            EntProtoId? targetEffect = doAfter.Args.TargetEffect;
            ((EntitySystem) this).SpawnAttachedTo(targetEffect.HasValue ? (string) targetEffect.GetValueOrDefault() : (string) null, component.Coordinates, rotation: new Angle());
          }
          doAfter.LastEffectSpawnTime = new TimeSpan?(time);
        }
        if (this.ShouldCancel(doAfter, xformQuery, handsQuery))
        {
          this.InternalCancel(doAfter, comp);
          flag = true;
        }
        else if (this._rmcDoAfter.ShouldCancel(doAfter))
        {
          this.InternalCancel(doAfter, comp);
          flag = true;
        }
        else if (time - doAfter.StartTime >= doAfter.Args.Delay)
        {
          this.TryComplete(doAfter, comp);
          flag = true;
        }
      }
    }
    if (flag)
      ((EntitySystem) this).Dirty(uid, (IComponent) comp);
    if (comp.DoAfters.Count != 0)
      return;
    ((EntitySystem) this).RemCompDeferred(uid, (IComponent) active);
  }

  private bool TryAttemptEvent(Content.Shared.DoAfter.DoAfter doAfter)
  {
    DoAfterArgs args = doAfter.Args;
    Func<bool> extraCheck = args.ExtraCheck;
    if ((extraCheck != null ? (!extraCheck() ? 1 : 0) : 0) != 0)
      return false;
    if (doAfter.AttemptEvent == null)
    {
      Type type = typeof (DoAfterAttemptEvent<>).MakeGenericType(args.Event.GetType());
      doAfter.AttemptEvent = this._factory.CreateInstance(type, new object[2]
      {
        (object) doAfter,
        (object) args.Event
      });
    }
    args.Event.DoAfter = doAfter;
    if (args.EventTarget.HasValue)
      ((EntitySystem) this).RaiseLocalEvent(args.EventTarget.Value, doAfter.AttemptEvent, args.Broadcast);
    else
      ((EntitySystem) this).RaiseLocalEvent(doAfter.AttemptEvent);
    CancellableEntityEventArgs attemptEvent = (CancellableEntityEventArgs) doAfter.AttemptEvent;
    if (!attemptEvent.Cancelled)
      return true;
    attemptEvent.Uncancel();
    return false;
  }

  private void TryComplete(Content.Shared.DoAfter.DoAfter doAfter, DoAfterComponent component)
  {
    if (doAfter.Cancelled || doAfter.Completed)
      return;
    if (doAfter.Args.AttemptFrequency == AttemptFrequency.StartAndEnd && !this.TryAttemptEvent(doAfter))
    {
      this.InternalCancel(doAfter, component);
    }
    else
    {
      doAfter.Completed = true;
      this.RaiseDoAfterEvents(doAfter, component);
      if (!doAfter.Args.Event.Repeat)
        return;
      doAfter.StartTime = this.GameTiming.CurTime;
      doAfter.Completed = false;
    }
  }

  private bool ShouldCancel(
    Content.Shared.DoAfter.DoAfter doAfter,
    EntityQuery<TransformComponent> xformQuery,
    EntityQuery<HandsComponent> handsQuery)
  {
    DoAfterArgs args = doAfter.Args;
    EntityUid? used = args.Used;
    if (used.HasValue)
    {
      EntityUid valueOrDefault = used.GetValueOrDefault();
      if (!xformQuery.HasComponent(valueOrDefault))
        return true;
    }
    EntityUid? eventTarget = args.EventTarget;
    if (eventTarget.HasValue)
    {
      EntityUid valueOrDefault = eventTarget.GetValueOrDefault();
      if (valueOrDefault.Valid && !xformQuery.HasComponent(valueOrDefault))
        return true;
    }
    TransformComponent component1;
    if (!xformQuery.TryGetComponent(args.User, out component1))
      return true;
    TransformComponent component2 = (TransformComponent) null;
    EntityUid? nullable = args.Target;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      if (!xformQuery.TryGetComponent(valueOrDefault, out component2))
        return true;
    }
    TransformComponent component3 = (TransformComponent) null;
    nullable = args.Used;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      if (!xformQuery.TryGetComponent(valueOrDefault, out component3))
        return true;
    }
    float distance;
    if (args.BreakOnMove && (args.BreakOnWeightlessMove || !this._gravity.IsWeightless(args.User, xform: component1)) && (!this._transform.InRange(component1.Coordinates, doAfter.UserPosition, args.MovementThreshold) || component2 != null && component2.Coordinates.TryDistance((IEntityManager) ((EntitySystem) this).EntityManager, component1.Coordinates, out distance) && (double) Math.Abs(distance - doAfter.TargetDistance) > (double) args.MovementThreshold))
      return true;
    if (args.Target.HasValue)
    {
      if (args.DistanceThreshold.HasValue)
      {
        SharedInteractionSystem interaction = this._interaction;
        Entity<TransformComponent> user1 = (Entity<TransformComponent>) args.User;
        Entity<TransformComponent> other = (Entity<TransformComponent>) args.Target.Value;
        double range = (double) args.DistanceThreshold.Value;
        nullable = new EntityUid?();
        EntityUid? user2 = nullable;
        if (!interaction.InRangeUnobstructed(user1, other, (float) range, user: user2))
          return true;
      }
      else
      {
        SharedInteractionSystem interaction = this._interaction;
        Entity<TransformComponent> user3 = (Entity<TransformComponent>) args.User;
        Entity<TransformComponent> other = (Entity<TransformComponent>) args.Target.Value;
        nullable = new EntityUid?();
        EntityUid? user4 = nullable;
        if (!interaction.InRangeUnobstructed(user3, other, user: user4))
          return true;
      }
    }
    if (args.Used.HasValue)
    {
      if (args.DistanceThreshold.HasValue)
      {
        SharedInteractionSystem interaction = this._interaction;
        Entity<TransformComponent> user5 = (Entity<TransformComponent>) args.User;
        Entity<TransformComponent> other = (Entity<TransformComponent>) args.Used.Value;
        double range = (double) args.DistanceThreshold.Value;
        nullable = new EntityUid?();
        EntityUid? user6 = nullable;
        if (!interaction.InRangeUnobstructed(user5, other, (float) range, user: user6))
          return true;
      }
      else
      {
        SharedInteractionSystem interaction = this._interaction;
        Entity<TransformComponent> user7 = (Entity<TransformComponent>) args.User;
        Entity<TransformComponent> other = (Entity<TransformComponent>) args.Used.Value;
        nullable = new EntityUid?();
        EntityUid? user8 = nullable;
        if (!interaction.InRangeUnobstructed(user7, other, user: user8))
          return true;
      }
    }
    HandsComponent component4;
    return args.AttemptFrequency == AttemptFrequency.EveryTick && !this.TryAttemptEvent(doAfter) || args.NeedHand && (!handsQuery.TryGetComponent(args.User, out component4) || component4.Count == 0 || args.BreakOnDropItem && doAfter.InitialItem.HasValue && !this._hands.IsHolding((Entity<HandsComponent>) (args.User, component4), doAfter.InitialItem) || args.BreakOnHandChange && component4.ActiveHandId != doAfter.InitialHand) || args.RequireCanInteract && !this._actionBlocker.CanInteract(args.User, args.Target);
  }

  protected SharedDoAfterSystem() => ((EntitySystem) this).\u002Ector();
}
