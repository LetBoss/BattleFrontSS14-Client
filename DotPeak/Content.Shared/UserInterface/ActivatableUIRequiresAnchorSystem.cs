// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.ActivatableUIRequiresAnchorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared.UserInterface;

public sealed class ActivatableUIRequiresAnchorSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActivatableUIRequiresAnchorComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<ActivatableUIRequiresAnchorComponent, ActivatableUIOpenAttemptEvent>(this.OnActivatableUIOpenAttempt));
    this.SubscribeLocalEvent<ActivatableUIRequiresAnchorComponent, BoundUserInterfaceCheckRangeEvent>(new EntityEventRefHandler<ActivatableUIRequiresAnchorComponent, BoundUserInterfaceCheckRangeEvent>(this.OnUICheck));
  }

  private void OnUICheck(
    Entity<ActivatableUIRequiresAnchorComponent> ent,
    ref BoundUserInterfaceCheckRangeEvent args)
  {
    if (args.Result == BoundUserInterfaceRangeResult.Fail || this.Transform(ent.Owner).Anchored)
      return;
    args.Result = BoundUserInterfaceRangeResult.Fail;
  }

  private void OnActivatableUIOpenAttempt(
    Entity<ActivatableUIRequiresAnchorComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || this.Transform(ent.Owner).Anchored)
      return;
    if (ent.Comp.Popup.HasValue)
    {
      SharedPopupSystem popup1 = this._popup;
      ILocalizationManager loc = this.Loc;
      LocId? popup2 = ent.Comp.Popup;
      string valueOrDefault = popup2.HasValue ? (string) popup2.GetValueOrDefault() : (string) null;
      string message = loc.GetString(valueOrDefault);
      EntityUid? recipient = new EntityUid?(args.User);
      popup1.PopupClient(message, recipient);
    }
    args.Cancel();
  }
}
