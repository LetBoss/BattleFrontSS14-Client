// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Actions.RMCActionsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Actions;

public sealed class RMCActionsSystem : SharedRMCActionsSystem
{
  [Dependency]
  private ActionsSystem _actions;
  [Dependency]
  private IPlayerManager _player;
  private EntityUid? _sortEnt;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCActionOrderLoadedEvent>(new EntityEventHandler<RMCActionOrderLoadedEvent>(this.OnActionOrderLoaded), (Type[]) null, (Type[]) null);
  }

  private void OnActionOrderLoaded(RMCActionOrderLoadedEvent ev)
  {
    this._sortEnt = new EntityUid?();
  }

  public void ActionsChanged(List<EntityUid?> actions)
  {
    List<EntProtoId> actions1 = new List<EntProtoId>();
    foreach (EntityUid? action in actions)
    {
      if (action.HasValue)
      {
        EntityUid valueOrDefault = action.GetValueOrDefault();
        MetaDataComponent metaDataComponent;
        if (this.Exists(valueOrDefault) && this.TryComp(valueOrDefault, ref metaDataComponent))
        {
          EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
          if (entityPrototype != null)
            actions1.Add(EntProtoId.op_Implicit(entityPrototype.ID));
        }
      }
    }
    this.RaiseNetworkEvent((EntityEventArgs) new RMCActionOrderChangeEvent(actions1));
  }

  private void SortDefault(EntityUid player)
  {
    XenoComponent xenoComponent;
    if (!this.TryComp<XenoComponent>(player, ref xenoComponent))
      return;
    foreach ((EntProtoId _, EntityUid entityUid) in xenoComponent.Actions)
    {
      if (!((EntityUid) ref entityUid).IsValid())
        return;
    }
    this._sortEnt = new EntityUid?(player);
    List<Entity<ActionComponent>> source = new List<Entity<ActionComponent>>();
    foreach (Entity<ActionComponent> action in this._actions.GetActions(player))
      source.Add(action);
    List<EntityUid> xenoActions = xenoComponent.Actions.Values.ToList<EntityUid>();
    source.Sort((Comparison<Entity<ActionComponent>>) ((a, b) =>
    {
      int index1 = xenoActions.FindIndex((Predicate<EntityUid>) (e => EntityUid.op_Equality(e, a.Owner)));
      int index2 = xenoActions.FindIndex((Predicate<EntityUid>) (e => EntityUid.op_Equality(e, b.Owner)));
      return index1 != -1 && index2 != -1 ? index1 - index2 : ActionsSystem.ActionComparer(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(a), Entity<ActionComponent>.op_Implicit(a))), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(b), Entity<ActionComponent>.op_Implicit(b))));
    }));
    this._actions.SetAssignments(source.Select<Entity<ActionComponent>, ActionsSystem.SlotAssignment>((Func<Entity<ActionComponent>, int, ActionsSystem.SlotAssignment>) ((t, i) => new ActionsSystem.SlotAssignment((byte) 0, (byte) i, Entity<ActionComponent>.op_Implicit(t)))).ToList<ActionsSystem.SlotAssignment>());
  }

  public virtual void Update(float frameTime)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault1 = localEntity.GetValueOrDefault();
    EntityUid? sortEnt = this._sortEnt;
    EntityUid entityUid = valueOrDefault1;
    if ((sortEnt.HasValue ? (EntityUid.op_Equality(sortEnt.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
      return;
    this._sortEnt = new EntityUid?();
    RMCActionOrderComponent actionOrderComponent;
    if (this.TryComp<RMCActionOrderComponent>(valueOrDefault1, ref actionOrderComponent))
    {
      ImmutableArray<EntProtoId>? order = actionOrderComponent.Order;
      if (order.HasValue)
      {
        ImmutableArray<EntProtoId> valueOrDefault2 = order.GetValueOrDefault();
        if (valueOrDefault2.Length > 0)
        {
          Entity<ActionComponent>[] array1 = this._actions.GetClientActions().ToArray<Entity<ActionComponent>>();
          foreach (Entity<ActionComponent> entity in array1)
          {
            if (!((EntityUid) ref entity.Owner).IsValid())
              return;
          }
          this._sortEnt = new EntityUid?(valueOrDefault1);
          Entity<ActionComponent>[] first = new Entity<ActionComponent>[valueOrDefault2.Length];
          List<Entity<ActionComponent>> second = new List<Entity<ActionComponent>>();
          foreach (Entity<ActionComponent> entity in array1)
          {
            MetaDataComponent metaDataComponent;
            if (this.TryComp(Entity<ActionComponent>.op_Implicit(entity), ref metaDataComponent))
            {
              EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
              if (entityPrototype != null)
              {
                int index = valueOrDefault2.IndexOf(EntProtoId.op_Implicit(entityPrototype.ID));
                if (index < 0)
                {
                  second.Add(entity);
                  continue;
                }
                first[index] = entity;
                continue;
              }
            }
            second.Add(entity);
          }
          List<ActionsSystem.SlotAssignment> actions = new List<ActionsSystem.SlotAssignment>();
          Entity<ActionComponent>[] array2 = ((IEnumerable<Entity<ActionComponent>>) first).Concat<Entity<ActionComponent>>((IEnumerable<Entity<ActionComponent>>) second).Where<Entity<ActionComponent>>((Func<Entity<ActionComponent>, bool>) (a => Entity<ActionComponent>.op_Inequality(a, new Entity<ActionComponent>()))).ToArray<Entity<ActionComponent>>();
          for (int Slot = 0; Slot < array2.Length; ++Slot)
            actions.Add(new ActionsSystem.SlotAssignment((byte) 0, (byte) Slot, Entity<ActionComponent>.op_Implicit(array2[Slot])));
          this._actions.SetAssignments(actions);
          return;
        }
      }
    }
    this.SortDefault(valueOrDefault1);
  }
}
