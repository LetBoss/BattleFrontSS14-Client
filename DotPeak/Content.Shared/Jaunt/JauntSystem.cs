// Decompiled with JetBrains decompiler
// Type: Content.Shared.Jaunt.JauntSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Jaunt;

public sealed class JauntSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<JauntComponent, MapInitEvent>(new EntityEventRefHandler<JauntComponent, MapInitEvent>(this.OnInit));
    this.SubscribeLocalEvent<JauntComponent, ComponentShutdown>(new EntityEventRefHandler<JauntComponent, ComponentShutdown>(this.OnShutdown));
  }

  private void OnInit(Entity<JauntComponent> ent, ref MapInitEvent args)
  {
    this._actions.AddAction(ent.Owner, ref ent.Comp.Action, (string) ent.Comp.JauntAction);
  }

  private void OnShutdown(Entity<JauntComponent> ent, ref ComponentShutdown args)
  {
    SharedActionsSystem actions = this._actions;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) ent.Owner;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions.RemoveAction(owner, action2);
  }
}
