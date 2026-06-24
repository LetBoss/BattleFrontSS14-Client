// Decompiled with JetBrains decompiler
// Type: Content.Shared.PAI.SharedPAISystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.PAI;

public abstract class SharedPAISystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PAIComponent, MapInitEvent>(new EntityEventRefHandler<PAIComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<PAIComponent, ComponentShutdown>(new EntityEventRefHandler<PAIComponent, ComponentShutdown>(this.OnShutdown));
  }

  private void OnMapInit(Entity<PAIComponent> ent, ref MapInitEvent args)
  {
    this._actions.AddAction((EntityUid) ent, (string) ent.Comp.ShopActionId);
  }

  private void OnShutdown(Entity<PAIComponent> ent, ref ComponentShutdown args)
  {
    SharedActionsSystem actions = this._actions;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) ent.Owner;
    EntityUid? shopAction = ent.Comp.ShopAction;
    Entity<ActionComponent>? action = shopAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) shopAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions.RemoveAction(owner, action);
  }
}
