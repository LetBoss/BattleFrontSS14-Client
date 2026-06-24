// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.ActivatableUIRequiresPowerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Power;

public sealed class ActivatableUIRequiresPowerSystem : SharedActivatableUIRequiresPowerSystem
{
  [Dependency]
  private SharedPopupSystem _popup;

  protected override void OnActivate(
    Entity<ActivatableUIRequiresPowerComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || this.IsPowered(ent.Owner, (IEntityManager) this.EntityManager))
      return;
    this._popup.PopupClient(this.Loc.GetString("base-computer-ui-component-not-powered", ("machine", (object) ent.Owner)), args.User, new EntityUid?(args.User));
    args.Cancel();
  }
}
