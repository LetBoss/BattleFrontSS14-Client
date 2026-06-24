// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.PumpActionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class PumpActionSystem : SharedPumpActionSystem
{
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private SharedPopupSystem _popup;

  protected override void OnExamined(Entity<PumpActionComponent> ent, ref ExaminedEvent args)
  {
    IKeyBinding ikeyBinding;
    if (!this._input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref ikeyBinding))
      return;
    args.PushMarkup(this.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine)), 1);
  }

  protected override void OnAttemptShoot(
    Entity<PumpActionComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled)
      return;
    base.OnAttemptShoot(ent, ref args);
    GunComponent gunComponent;
    if (ent.Comp.Pumped || !this.TryComp<GunComponent>(ent.Owner, ref gunComponent) || gunComponent.BurstActivated)
      return;
    IKeyBinding ikeyBinding;
    this._popup.PopupClient(this._input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref ikeyBinding) ? this.Loc.GetString(LocId.op_Implicit(ent.Comp.PopupKey), ("key", (object) ikeyBinding.GetKeyString())) : this.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)), args.User, new EntityUid?(args.User));
  }
}
