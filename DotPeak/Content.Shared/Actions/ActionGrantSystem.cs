// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.ActionGrantSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Actions;

public sealed class ActionGrantSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionGrantComponent, MapInitEvent>(new EntityEventRefHandler<ActionGrantComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionGrantComponent, ComponentShutdown>(new EntityEventRefHandler<ActionGrantComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemActionGrantComponent, GetItemActionsEvent>(new EntityEventRefHandler<ItemActionGrantComponent, GetItemActionsEvent>((object) this, __methodptr(OnItemGet)), (Type[]) null, (Type[]) null);
  }

  private void OnItemGet(Entity<ItemActionGrantComponent> ent, ref GetItemActionsEvent args)
  {
    ActionGrantComponent actionGrantComponent;
    if (!this.TryComp<ActionGrantComponent>(ent.Owner, ref actionGrantComponent) || ent.Comp.ActiveIfWorn && (!args.SlotFlags.HasValue || args.SlotFlags.GetValueOrDefault() == SlotFlags.POCKET))
      return;
    foreach (EntityUid actionEntity in actionGrantComponent.ActionEntities)
      args.AddAction(new EntityUid?(actionEntity));
  }

  private void OnMapInit(Entity<ActionGrantComponent> ent, ref MapInitEvent args)
  {
    foreach (EntProtoId action in ent.Comp.Actions)
    {
      EntityUid? actionId = new EntityUid?();
      this._actions.AddAction(ent.Owner, ref actionId, EntProtoId.op_Implicit(action), new EntityUid());
      if (actionId.HasValue)
        ent.Comp.ActionEntities.Add(actionId.Value);
    }
  }

  private void OnShutdown(Entity<ActionGrantComponent> ent, ref ComponentShutdown args)
  {
    foreach (EntityUid actionEntity in ent.Comp.ActionEntities)
      this._actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity)));
  }
}
