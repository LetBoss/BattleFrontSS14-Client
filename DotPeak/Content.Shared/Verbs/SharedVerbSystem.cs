// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.SharedVerbSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Admin;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Verbs;

public abstract class SharedVerbSystem : EntitySystem
{
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  protected SharedContainerSystem ContainerSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<ExecuteVerbEvent>(new EntitySessionEventHandler<ExecuteVerbEvent>(this.HandleExecuteVerb));
  }

  private void HandleExecuteVerb(ExecuteVerbEvent args, EntitySessionEventArgs eventArgs)
  {
    EntityUid? attachedEntity = eventArgs.SenderSession.AttachedEntity;
    EntityUid? entity;
    Verb actualValue;
    if (!attachedEntity.HasValue || !this.TryGetEntity(args.Target, out entity) || this.Deleted(attachedEntity) || !this.GetLocalVerbs(entity.Value, attachedEntity.Value, args.RequestedVerb.GetType()).TryGetValue(args.RequestedVerb, out actualValue))
      return;
    this.ExecuteVerb(actualValue, attachedEntity.Value, entity.Value);
  }

  public SortedSet<Verb> GetLocalVerbs(EntityUid target, EntityUid user, Type type, bool force = false)
  {
    EntityUid target1 = target;
    EntityUid user1 = user;
    List<Type> types = new List<Type>();
    types.Add(type);
    int num = force ? 1 : 0;
    return this.GetLocalVerbs(target1, user1, types, num != 0);
  }

  public SortedSet<Verb> GetLocalVerbs(
    EntityUid target,
    EntityUid user,
    List<Type> types,
    bool force = false)
  {
    return this.GetLocalVerbs(target, user, types, out List<VerbCategory> _, force);
  }

  public SortedSet<Verb> GetLocalVerbs(
    EntityUid target,
    EntityUid user,
    List<Type> types,
    out List<VerbCategory> extraCategories,
    bool force = false)
  {
    SortedSet<Verb> localVerbs = new SortedSet<Verb>();
    extraCategories = new List<VerbCategory>();
    bool canAccess = force || this._interactionSystem.InRangeAndAccessible((Entity<TransformComponent>) user, (Entity<TransformComponent>) target);
    bool canInteract = force || this._actionBlockerSystem.CanInteract(user, new EntityUid?(target));
    bool canComplexInteract = force || this._actionBlockerSystem.CanComplexInteract(user);
    EntityUid? used;
    this._interactionSystem.TryGetUsedEntity(user, out used);
    HandsComponent comp;
    this.TryComp<HandsComponent>(user, out comp);
    if (types.Contains(typeof (InteractionVerb)))
    {
      GetVerbsEvent<InteractionVerb> args = new GetVerbsEvent<InteractionVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<InteractionVerb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (UtilityVerb)) && used.HasValue)
    {
      EntityUid? nullable = used;
      EntityUid entityUid = target;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      {
        GetVerbsEvent<UtilityVerb> args = new GetVerbsEvent<UtilityVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
        this.RaiseLocalEvent<GetVerbsEvent<UtilityVerb>>(used.Value, args, true);
        localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
      }
    }
    if (types.Contains(typeof (InnateVerb)))
    {
      GetVerbsEvent<InnateVerb> args = new GetVerbsEvent<InnateVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<InnateVerb>>(user, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (AlternativeVerb)))
    {
      GetVerbsEvent<AlternativeVerb> args = new GetVerbsEvent<AlternativeVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (ActivationVerb)))
    {
      GetVerbsEvent<ActivationVerb> args = new GetVerbsEvent<ActivationVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<ActivationVerb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (ExamineVerb)))
    {
      GetVerbsEvent<ExamineVerb> args = new GetVerbsEvent<ExamineVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<ExamineVerb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (Verb)))
    {
      GetVerbsEvent<Verb> args = new GetVerbsEvent<Verb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<Verb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (EquipmentVerb)))
    {
      int num = canAccess ? 1 : (this._interactionSystem.CanAccessEquipment(user, target) ? 1 : 0);
      GetVerbsEvent<EquipmentVerb> args = new GetVerbsEvent<EquipmentVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<EquipmentVerb>>(target, args);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    if (types.Contains(typeof (RMCAdminVerb)))
    {
      GetVerbsEvent<RMCAdminVerb> args = new GetVerbsEvent<RMCAdminVerb>(user, target, used, comp, canInteract, canComplexInteract, canAccess, extraCategories);
      this.RaiseLocalEvent<GetVerbsEvent<RMCAdminVerb>>(target, args, true);
      localVerbs.UnionWith((IEnumerable<Verb>) args.Verbs);
    }
    return localVerbs;
  }

  public virtual void ExecuteVerb(Verb verb, EntityUid user, EntityUid target, bool forced = false)
  {
    Action act = verb.Act;
    if (act != null)
      act();
    if (verb.ExecutionEventArgs != null)
    {
      if (verb.EventTarget.IsValid())
        this.RaiseLocalEvent(verb.EventTarget, verb.ExecutionEventArgs);
      else
        this.RaiseLocalEvent(verb.ExecutionEventArgs);
    }
    if (this.Deleted(user) || this.Deleted(target) || ((int) verb.DoContactInteraction ?? (!verb.DefaultDoContactInteraction ? 0 : (this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target) ? 1 : 0))) == 0)
      return;
    this._interactionSystem.DoContactInteraction(user, new EntityUid?(target));
  }
}
