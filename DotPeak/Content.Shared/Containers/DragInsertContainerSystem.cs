// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.DragInsertContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Climbing.Systems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Containers;

public sealed class DragInsertContainerSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private ClimbSystem _climb;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DragInsertContainerComponent, DragDropTargetEvent>(new EntityEventRefHandler<DragInsertContainerComponent, DragDropTargetEvent>((object) this, __methodptr(OnDragDropOn)), new Type[1]
    {
      typeof (ClimbSystem)
    }, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DragInsertContainerComponent, DragInsertContainerSystem.DragInsertContainerDoAfterEvent>(new EntityEventRefHandler<DragInsertContainerComponent, DragInsertContainerSystem.DragInsertContainerDoAfterEvent>((object) this, __methodptr(OnDragFinished)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DragInsertContainerComponent, CanDropTargetEvent>(new EntityEventRefHandler<DragInsertContainerComponent, CanDropTargetEvent>((object) this, __methodptr(OnCanDragDropOn)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DragInsertContainerComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DragInsertContainerComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(OnGetAlternativeVerb)), (Type[]) null, (Type[]) null);
  }

  private void OnDragDropOn(Entity<DragInsertContainerComponent> ent, ref DragDropTargetEvent args)
  {
    if (args.Handled)
      return;
    EntityUid entityUid;
    DragInsertContainerComponent containerComponent1;
    ent.Deconstruct(ref entityUid, ref containerComponent1);
    DragInsertContainerComponent containerComponent2 = containerComponent1;
    BaseContainer container;
    if (!this._container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), containerComponent2.ContainerId, ref container, (ContainerManagerComponent) null))
      return;
    if (containerComponent2.EntryDelay <= TimeSpan.Zero || !containerComponent2.DelaySelfEntry && EntityUid.op_Equality(args.User, args.Dragged))
    {
      args.Handled = this.Insert(args.Dragged, args.User, Entity<DragInsertContainerComponent>.op_Implicit(ent), container);
    }
    else
    {
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, containerComponent2.EntryDelay, (DoAfterEvent) new DragInsertContainerSystem.DragInsertContainerDoAfterEvent(), new EntityUid?(Entity<DragInsertContainerComponent>.op_Implicit(ent)), new EntityUid?(args.Dragged), new EntityUid?(Entity<DragInsertContainerComponent>.op_Implicit(ent)))
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = false
      });
      args.Handled = true;
    }
  }

  private void OnDragFinished(
    Entity<DragInsertContainerComponent> ent,
    ref DragInsertContainerSystem.DragInsertContainerDoAfterEvent args)
  {
    BaseContainer container;
    if (args.Handled || args.Cancelled || !args.Args.Target.HasValue || !this._container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent) null))
      return;
    this.Insert(args.Args.Target.Value, args.User, Entity<DragInsertContainerComponent>.op_Implicit(ent), container);
  }

  private void OnCanDragDropOn(
    Entity<DragInsertContainerComponent> ent,
    ref CanDropTargetEvent args)
  {
    EntityUid entityUid;
    DragInsertContainerComponent containerComponent1;
    ent.Deconstruct(ref entityUid, ref containerComponent1);
    DragInsertContainerComponent containerComponent2 = containerComponent1;
    BaseContainer baseContainer;
    if (!this._container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), containerComponent2.ContainerId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    args.Handled = true;
    args.CanDrop |= this._container.CanInsert(args.Dragged, baseContainer, false, (TransformComponent) null);
  }

  private void OnGetAlternativeVerb(
    Entity<DragInsertContainerComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    EntityUid entityUid1;
    DragInsertContainerComponent containerComponent1;
    ent.Deconstruct(ref entityUid1, ref containerComponent1);
    EntityUid entityUid2 = entityUid1;
    DragInsertContainerComponent containerComponent2 = containerComponent1;
    BaseContainer container;
    if (!containerComponent2.UseVerbs || !args.CanInteract || !args.CanAccess || args.Hands == null || !this._container.TryGetContainer(entityUid2, containerComponent2.ContainerId, ref container, (ContainerManagerComponent) null))
      return;
    EntityUid user = args.User;
    if (!this._actionBlocker.CanInteract(user, new EntityUid?(Entity<DragInsertContainerComponent>.op_Implicit(ent))))
      return;
    if (container.ContainedEntities.Count > 0)
    {
      int num = 0;
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
      {
        if (this._container.CanRemove(containedEntity, container))
          ++num;
      }
      if (num > 0)
      {
        AlternativeVerb alternativeVerb1 = new AlternativeVerb();
        alternativeVerb1.Act = (Action) (() =>
        {
          ISharedAdminLogManager adminLog = this._adminLog;
          LogStringHandler logStringHandler = new LogStringHandler(19, 2);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" emptied container ");
          logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<DragInsertContainerComponent>.op_Implicit(ent)), (MetaDataComponent) null), "ToPrettyString(ent)");
          ref LogStringHandler local = ref logStringHandler;
          adminLog.Add(LogType.Action, LogImpact.Low, ref local);
          foreach (EntityUid uid in this._container.EmptyContainer(container, false, new EntityCoordinates?(), true))
            this._climb.ForciblySetClimbing(uid, Entity<DragInsertContainerComponent>.op_Implicit(ent));
        });
        alternativeVerb1.Category = VerbCategory.Eject;
        alternativeVerb1.Text = this.Loc.GetString("container-verb-text-empty");
        alternativeVerb1.Priority = 1;
        AlternativeVerb alternativeVerb2 = alternativeVerb1;
        args.Verbs.Add(alternativeVerb2);
      }
    }
    if (!this._container.CanInsert(user, container, false, (TransformComponent) null) || !this._actionBlocker.CanMove(user))
      return;
    AlternativeVerb alternativeVerb3 = new AlternativeVerb();
    alternativeVerb3.Act = (Action) (() => this.Insert(user, user, Entity<DragInsertContainerComponent>.op_Implicit(ent), container));
    alternativeVerb3.Text = this.Loc.GetString("container-verb-text-enter");
    alternativeVerb3.Priority = 2;
    AlternativeVerb alternativeVerb4 = alternativeVerb3;
    args.Verbs.Add(alternativeVerb4);
  }

  public bool Insert(
    EntityUid target,
    EntityUid user,
    EntityUid containerEntity,
    BaseContainer container)
  {
    if (!this._container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(target), container, (TransformComponent) null, false))
      return false;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(26, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" inserted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" into container ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(containerEntity)), "ToPrettyString(containerEntity)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
    return true;
  }

  [NetSerializable]
  [Serializable]
  public sealed class DragInsertContainerDoAfterEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<DragInsertContainerSystem.DragInsertContainerDoAfterEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref DragInsertContainerSystem.DragInsertContainerDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (DragInsertContainerSystem.DragInsertContainerDoAfterEvent) target1;
      serialization.TryCustomCopy<DragInsertContainerSystem.DragInsertContainerDoAfterEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref DragInsertContainerSystem.DragInsertContainerDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DragInsertContainerSystem.DragInsertContainerDoAfterEvent target1 = (DragInsertContainerSystem.DragInsertContainerDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DragInsertContainerSystem.DragInsertContainerDoAfterEvent target1 = (DragInsertContainerSystem.DragInsertContainerDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual DragInsertContainerSystem.DragInsertContainerDoAfterEvent SimpleDoAfterEvent.Instantiate()
    {
      return new DragInsertContainerSystem.DragInsertContainerDoAfterEvent();
    }
  }
}
