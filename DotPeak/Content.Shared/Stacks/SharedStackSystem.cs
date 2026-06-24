// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stacks.SharedStackSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Stacks;

public abstract class SharedStackSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IViewVariablesManager _vvm;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedHandsSystem Hands;
  [Dependency]
  protected SharedTransformSystem Xform;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedStorageSystem _storage;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StackComponent, ComponentGetState>(new ComponentEventRefHandler<StackComponent, ComponentGetState>(this.OnStackGetState));
    this.SubscribeLocalEvent<StackComponent, ComponentHandleState>(new ComponentEventRefHandler<StackComponent, ComponentHandleState>(this.OnStackHandleState));
    this.SubscribeLocalEvent<StackComponent, ComponentStartup>(new ComponentEventHandler<StackComponent, ComponentStartup>(this.OnStackStarted));
    this.SubscribeLocalEvent<StackComponent, ExaminedEvent>(new ComponentEventHandler<StackComponent, ExaminedEvent>(this.OnStackExamined));
    this.SubscribeLocalEvent<StackComponent, InteractUsingEvent>(new ComponentEventHandler<StackComponent, InteractUsingEvent>(this.OnStackInteractUsing));
    this._vvm.GetTypeHandler<StackComponent>().AddPath<int>("Count", (ComponentPropertyGetter<StackComponent, int>) ((_, comp) => comp.Count), new ComponentPropertySetter<StackComponent, int>(this.SetCount));
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this._vvm.GetTypeHandler<StackComponent>().RemovePath("Count");
  }

  private void OnStackInteractUsing(EntityUid uid, StackComponent stack, InteractUsingEvent args)
  {
    StackComponent comp;
    if (args.Handled || !this.TryComp<StackComponent>(args.Used, out comp))
      return;
    Angle localRotation = this.Transform(args.Used).LocalRotation;
    int transferred;
    if (!this.TryMergeStacks(uid, args.Used, out transferred, stack, comp))
      return;
    args.Handled = true;
    if (!this._gameTiming.IsFirstTimePredicted)
      return;
    EntityCoordinates entityCoordinates = args.ClickLocation;
    EntityCoordinates coordinates = this.Transform(args.User).Coordinates;
    if (!entityCoordinates.IsValid((IEntityManager) this.EntityManager))
      entityCoordinates = coordinates;
    int num = transferred;
    if (num <= 0)
    {
      if (num == 0 && this.GetAvailableSpace(comp) == 0)
        this.Popup.PopupCoordinates(this.Loc.GetString("comp-stack-already-full"), entityCoordinates, Filter.Local(), false);
    }
    else
    {
      this.Popup.PopupCoordinates($"+{transferred}", entityCoordinates, Filter.Local(), false);
      if (this.GetAvailableSpace(comp) == 0)
        this.Popup.PopupCoordinates(this.Loc.GetString("comp-stack-becomes-full"), entityCoordinates.Offset(new Vector2(0.0f, -0.5f)), Filter.Local(), false);
    }
    this._storage.PlayPickupAnimation(args.Used, entityCoordinates, coordinates, localRotation, new EntityUid?(args.User));
  }

  private bool TryMergeStacks(
    EntityUid donor,
    EntityUid recipient,
    out int transferred,
    StackComponent? donorStack = null,
    StackComponent? recipientStack = null)
  {
    transferred = 0;
    if (donor == recipient || !this.Resolve<StackComponent>(recipient, ref recipientStack, false) || !this.Resolve<StackComponent>(donor, ref donorStack, false) || string.IsNullOrEmpty(recipientStack.StackTypeId) || !recipientStack.StackTypeId.Equals(donorStack.StackTypeId))
      return false;
    transferred = Math.Min(donorStack.Count, this.GetAvailableSpace(recipientStack));
    this.SetCount(donor, donorStack.Count - transferred, donorStack);
    this.SetCount(recipient, recipientStack.Count + transferred, recipientStack);
    return transferred > 0;
  }

  public void TryMergeToHands(
    EntityUid item,
    EntityUid user,
    StackComponent? itemStack = null,
    HandsComponent? hands = null)
  {
    if (!this.Resolve<HandsComponent>(user, ref hands, false))
      return;
    if (!this.Resolve<StackComponent>(item, ref itemStack, false))
    {
      this.Hands.PickupOrDrop(new EntityUid?(user), item, handsComp: hands);
    }
    else
    {
      foreach (EntityUid recipient in this.Hands.EnumerateHeld((Entity<HandsComponent>) (user, hands)))
      {
        this.TryMergeStacks(item, recipient, out int _, itemStack);
        if (itemStack.Count == 0)
          return;
      }
      this.Hands.PickupOrDrop(new EntityUid?(user), item, handsComp: hands);
    }
  }

  public virtual void SetCount(EntityUid uid, int amount, StackComponent? component = null)
  {
    if (!this.Resolve<StackComponent>(uid, ref component) || amount == component.Count)
      return;
    int count = component.Count;
    amount = Math.Min(amount, this.GetMaxCount(component));
    amount = Math.Max(amount, 0);
    component.Count = amount;
    this.Dirty(uid, (IComponent) component);
    this.Appearance.SetData(uid, (Enum) StackVisuals.Actual, (object) component.Count);
    this.RaiseLocalEvent<StackCountChangedEvent>(uid, new StackCountChangedEvent(count, component.Count));
  }

  public bool Use(EntityUid uid, int amount, StackComponent? stack = null)
  {
    if (!this.Resolve<StackComponent>(uid, ref stack) || stack.Count < amount)
      return false;
    if (!stack.Unlimited)
      this.SetCount(uid, stack.Count - amount, stack);
    return true;
  }

  public bool TryMergeToContacts(EntityUid uid, StackComponent? stack = null, TransformComponent? xform = null)
  {
    if (!this.Resolve<StackComponent, TransformComponent>(uid, ref stack, ref xform, false))
      return false;
    MapId mapId = xform.MapID;
    Box2 worldAabb = this._physics.GetWorldAABB(uid);
    HashSet<Entity<StackComponent>> entities = new HashSet<Entity<StackComponent>>();
    this._entityLookup.GetEntitiesIntersecting<StackComponent>(mapId, worldAabb, entities, LookupFlags.Dynamic | LookupFlags.Sundries);
    bool contacts = false;
    foreach (Entity<StackComponent> recipientStack in entities)
    {
      EntityUid owner = recipientStack.Owner;
      if (!this.TerminatingOrDeleted(owner) && !this.EntityManager.IsQueuedForDeletion(owner) && this.TryMergeStacks(uid, owner, out int _, stack, (StackComponent) recipientStack))
      {
        contacts = true;
        if (stack.Count <= 0)
          break;
      }
    }
    return contacts;
  }

  public int GetCount(EntityUid uid, StackComponent? component = null)
  {
    return !this.Resolve<StackComponent>(uid, ref component, false) ? 1 : component.Count;
  }

  public int GetMaxCount(string entityId)
  {
    StackComponent component;
    this._prototype.Index<EntityPrototype>(entityId).TryGetComponent<StackComponent>(out component, this.EntityManager.ComponentFactory);
    return this.GetMaxCount(component);
  }

  public int GetMaxCount(EntityUid uid) => this.GetMaxCount(this.CompOrNull<StackComponent>(uid));

  public int GetMaxCount(StackComponent? component)
  {
    if (component == null)
      return 1;
    int? nullable = component.MaxCountOverride;
    if (nullable.HasValue)
    {
      nullable = component.MaxCountOverride;
      return nullable.Value;
    }
    if (string.IsNullOrEmpty(component.StackTypeId))
      return 1;
    nullable = this._prototype.Index<StackPrototype>(component.StackTypeId).MaxCount;
    return nullable ?? int.MaxValue;
  }

  public int GetAvailableSpace(StackComponent component)
  {
    return this.GetMaxCount(component) - component.Count;
  }

  public bool TryAdd(
    EntityUid insertEnt,
    EntityUid targetEnt,
    StackComponent? insertStack = null,
    StackComponent? targetStack = null)
  {
    if (!this.Resolve<StackComponent>(insertEnt, ref insertStack) || !this.Resolve<StackComponent>(targetEnt, ref targetStack))
      return false;
    int count = insertStack.Count;
    return this.TryAdd(insertEnt, targetEnt, count, insertStack, targetStack);
  }

  public bool TryAdd(
    EntityUid insertEnt,
    EntityUid targetEnt,
    int count,
    StackComponent? insertStack = null,
    StackComponent? targetStack = null)
  {
    if (!this.Resolve<StackComponent>(insertEnt, ref insertStack) || !this.Resolve<StackComponent>(targetEnt, ref targetStack) || insertStack.StackTypeId != targetStack.StackTypeId)
      return false;
    int availableSpace = this.GetAvailableSpace(targetStack);
    if (availableSpace <= 0)
      return false;
    int num = Math.Min(availableSpace, count);
    this.SetCount(targetEnt, targetStack.Count + num, targetStack);
    this.SetCount(insertEnt, insertStack.Count - num, insertStack);
    return true;
  }

  private void OnStackStarted(EntityUid uid, StackComponent component, ComponentStartup args)
  {
    this.SetCount(uid, component.Count, component);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this.Appearance.SetData(uid, (Enum) StackVisuals.Actual, (object) component.Count, comp);
    this.Appearance.SetData(uid, (Enum) StackVisuals.MaxCount, (object) this.GetMaxCount(component), comp);
    this.Appearance.SetData(uid, (Enum) StackVisuals.Hide, (object) false, comp);
  }

  private void OnStackGetState(EntityUid uid, StackComponent component, ref ComponentGetState args)
  {
    args.State = (IComponentState) new StackComponentState(component.Count, component.MaxCountOverride, component.Lingering);
  }

  private void OnStackHandleState(
    EntityUid uid,
    StackComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is StackComponentState current))
      return;
    component.MaxCountOverride = current.MaxCount;
    component.Lingering = current.Lingering;
    this.SetCount(uid, current.Count, component);
  }

  private void OnStackExamined(EntityUid uid, StackComponent component, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("comp-stack-examine-detail-count", ("count", (object) component.Count), ("markupCountColor", (object) "lightgray")));
  }
}
