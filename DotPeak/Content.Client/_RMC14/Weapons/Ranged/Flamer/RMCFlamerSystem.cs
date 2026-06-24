// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Flamer.RMCFlamerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Popups;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Flamer;

public sealed class RMCFlamerSystem : SharedRMCFlamerSystem
{
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private PopupSystem _popup;

  protected override void OnIgniterAttemptShoot(
    Entity<RMCIgniterComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled)
      return;
    base.OnIgniterAttemptShoot(ent, ref args);
    if (ent.Comp.Enabled)
      return;
    IKeyBinding ikeyBinding;
    this._popup.PopupClient(this._input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref ikeyBinding) ? this.Loc.GetString(LocId.op_Implicit(ent.Comp.PopupKey), ("key", (object) ikeyBinding.GetKeyString())) : this.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)), args.User, new EntityUid?(args.User), PopupType.Small);
  }
}
