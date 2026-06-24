// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedItemCreatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedItemCreatorSystem : EntitySystem
{
  [Dependency]
  private ActionContainerSystem _actionContainer;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemCreatorComponent, MapInitEvent>(new EntityEventRefHandler<ItemCreatorComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ItemCreatorComponent, GetItemActionsEvent>(new EntityEventRefHandler<ItemCreatorComponent, GetItemActionsEvent>(this.OnGetActions));
  }

  private void OnMapInit(Entity<ItemCreatorComponent> ent, ref MapInitEvent args)
  {
    (EntityUid entityUid, ItemCreatorComponent comp) = ent;
    if (string.IsNullOrEmpty((string) comp.Action))
      return;
    this._actionContainer.EnsureAction(entityUid, ref comp.ActionEntity, (string) comp.Action);
    this.Dirty(entityUid, (IComponent) comp);
  }

  private void OnGetActions(Entity<ItemCreatorComponent> ent, ref GetItemActionsEvent args)
  {
    if (!this.CheckItemCreator((EntityUid) ent, args.User))
      return;
    args.AddAction(ent.Comp.ActionEntity);
  }

  public bool CheckItemCreator(EntityUid uid, EntityUid user)
  {
    CheckItemCreatorEvent args = new CheckItemCreatorEvent(user);
    this.RaiseLocalEvent<CheckItemCreatorEvent>(uid, ref args);
    return !args.Cancelled;
  }
}
