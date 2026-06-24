// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleViewToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleViewToggleSystem : EntitySystem
{
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly SharedEyeSystem _eye;
  [Dependency]
  private readonly INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleViewToggleComponent, VehicleToggleViewActionEvent>(new EntityEventRefHandler<VehicleViewToggleComponent, VehicleToggleViewActionEvent>(this.OnToggleViewAction));
    this.SubscribeLocalEvent<VehicleViewToggleComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleViewToggleComponent, ComponentShutdown>(this.OnToggleViewShutdown));
  }

  public void EnableViewToggle(
    EntityUid user,
    EntityUid outsideTarget,
    EntityUid source,
    EntityUid? insideTarget,
    bool isOutside)
  {
    VehicleViewToggleComponent toggle = this.EnsureComp<VehicleViewToggleComponent>(user);
    toggle.Sources.Add(source);
    toggle.Source = new EntityUid?(source);
    toggle.OutsideTarget = new EntityUid?(outsideTarget);
    toggle.InsideTarget = insideTarget;
    toggle.IsOutside = isOutside;
    this.EnsureSingleToggleAction(user, toggle);
    EntityUid? action = toggle.Action;
    if (action.HasValue)
    {
      EntityUid valueOrDefault = action.GetValueOrDefault();
      ActionComponent comp;
      if (this.TryComp<ActionComponent>(valueOrDefault, out comp))
      {
        this._actions.SetItemIconStyle((Entity<ActionComponent>) (valueOrDefault, comp), ItemActionIconStyle.BigAction);
        this._actions.SetEntityIcon((Entity<ActionComponent>) (valueOrDefault, comp), new EntityUid?());
        this._actions.SetTemporary((Entity<ActionComponent>) (valueOrDefault, comp), false);
      }
    }
    this.UpdateActionState(toggle);
    this.Dirty(user, (IComponent) toggle);
  }

  public void DisableViewToggle(EntityUid user, EntityUid source)
  {
    VehicleViewToggleComponent comp;
    if (!this.TryComp<VehicleViewToggleComponent>(user, out comp))
      return;
    comp.Sources.Remove(source);
    if (comp.Sources.Count > 0)
    {
      using (HashSet<EntityUid>.Enumerator enumerator = comp.Sources.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          EntityUid current = enumerator.Current;
          comp.Source = new EntityUid?(current);
        }
      }
      this.EnsureSingleToggleAction(user, comp);
      this.UpdateActionState(comp);
      this.Dirty(user, (IComponent) comp);
    }
    else
    {
      EntityUid? action = comp.Action;
      if (action.HasValue)
        this.RemoveAndDeleteToggleAction(action.GetValueOrDefault(), new EntityUid?(user));
      comp.Action = new EntityUid?();
      comp.Source = new EntityUid?();
      comp.OutsideTarget = new EntityUid?();
      comp.InsideTarget = new EntityUid?();
      comp.IsOutside = false;
      this.RemComp<VehicleViewToggleComponent>(user);
    }
  }

  private void OnToggleViewShutdown(
    Entity<VehicleViewToggleComponent> ent,
    ref ComponentShutdown args)
  {
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    this.RemoveAndDeleteToggleAction(action.GetValueOrDefault(), new EntityUid?(ent.Owner));
  }

  private void OnToggleViewAction(
    Entity<VehicleViewToggleComponent> ent,
    ref VehicleToggleViewActionEvent args)
  {
    if (args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EyeComponent comp;
    if (!ent.Comp.OutsideTarget.HasValue || !this.TryComp<EyeComponent>(ent.Owner, out comp))
      return;
    EntityUid entityUid1 = ent.Comp.OutsideTarget.Value;
    EntityUid? target = comp.Target;
    EntityUid entityUid2 = entityUid1;
    if ((target.HasValue ? (target.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0)
    {
      this._eye.SetTarget(ent.Owner, ent.Comp.InsideTarget, comp);
      ent.Comp.IsOutside = false;
    }
    else
    {
      ent.Comp.InsideTarget = comp.Target;
      this._eye.SetTarget(ent.Owner, new EntityUid?(entityUid1), comp);
      ent.Comp.IsOutside = true;
    }
    this.EnsureSingleToggleAction(ent.Owner, ent.Comp);
    this.UpdateActionState(ent.Comp);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
    this.RaiseLocalEvent<VehicleViewToggledEvent>(ent.Owner, new VehicleViewToggledEvent(ent.Comp.IsOutside));
  }

  private void UpdateActionState(VehicleViewToggleComponent toggle)
  {
    if (!toggle.Action.HasValue)
      return;
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = toggle.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = toggle.IsOutside ? 1 : 0;
    actions.SetToggled(action2, num != 0);
  }

  private void EnsureSingleToggleAction(EntityUid user, VehicleViewToggleComponent toggle)
  {
    ActionsContainerComponent comp1;
    if (this.TryComp<ActionsContainerComponent>(user, out comp1))
    {
      foreach (EntityUid entityUid1 in comp1.Container.ContainedEntities.ToArray<EntityUid>())
      {
        if (IsToggleActionPrototype(entityUid1))
        {
          ActionComponent comp2;
          if (this.TryComp<ActionComponent>(entityUid1, out comp2))
          {
            EntityUid? attachedEntity = comp2.AttachedEntity;
            EntityUid entityUid2 = user;
            if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0)
              continue;
          }
          this.RemoveAndDeleteToggleAction(entityUid1, new EntityUid?(user));
        }
      }
    }
    EntityUid? nullable = new EntityUid?();
    EntityUid? action1 = toggle.Action;
    if (action1.HasValue)
    {
      EntityUid valueOrDefault = action1.GetValueOrDefault();
      if (IsLiveToggleAction(valueOrDefault))
        nullable = new EntityUid?(valueOrDefault);
    }
    ActionsComponent comp3;
    if (this.TryComp<ActionsComponent>(user, out comp3))
    {
      foreach (EntityUid entityUid in comp3.Actions.ToArray<EntityUid>())
      {
        if (IsLiveToggleAction(entityUid))
        {
          if (!nullable.HasValue)
            nullable = new EntityUid?(entityUid);
          else if (!(entityUid == nullable.Value))
            this.RemoveAndDeleteToggleAction(entityUid, new EntityUid?(user));
        }
      }
    }
    if (!nullable.HasValue)
      nullable = this._actions.AddAction(user, (string) toggle.ActionId);
    toggle.Action = nullable;
    EntityUid? action2 = toggle.Action;
    if (!action2.HasValue)
      return;
    this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) action2.GetValueOrDefault()), true);

    bool IsToggleActionPrototype(EntityUid actionUid)
    {
      MetaDataComponent comp;
      return !this.TerminatingOrDeleted(actionUid) && this.TryComp(actionUid, out comp) && comp.EntityPrototype?.ID == toggle.ActionId.ToString();
    }

    bool IsLiveToggleAction(EntityUid actionUid)
    {
      ActionComponent comp;
      if (!IsToggleActionPrototype(actionUid) || !this.TryComp<ActionComponent>(actionUid, out comp))
        return false;
      EntityUid? attachedEntity = comp.AttachedEntity;
      EntityUid user = user;
      return (attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() != user ? 1 : 0) : 1) == 0;
    }
  }

  private void RemoveAndDeleteToggleAction(EntityUid action, EntityUid? user = null)
  {
    if (this.TerminatingOrDeleted(action))
      return;
    if (user.HasValue)
      this._actions.RemoveAction((Entity<ActionsComponent>) user.GetValueOrDefault(), new Entity<ActionComponent>?((Entity<ActionComponent>) action));
    else
      this._actions.RemoveAction(new Entity<ActionComponent>?((Entity<ActionComponent>) action));
    if (this._net.IsClient || !this.Exists(action))
      return;
    this.QueueDel(new EntityUid?(action));
  }
}
