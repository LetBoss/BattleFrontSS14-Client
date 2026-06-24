// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.ActivatableUIRequiresAccessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Access.Systems;

public sealed class ActivatableUIRequiresAccessSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _access;
  [Dependency]
  private SharedPopupSystem _popup;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActivatableUIRequiresAccessComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<ActivatableUIRequiresAccessComponent, ActivatableUIOpenAttemptEvent>((object) this, __methodptr(OnUIOpenAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnUIOpenAttempt(
    Entity<ActivatableUIRequiresAccessComponent> activatableUI,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || this._access.IsAllowed(args.User, Entity<ActivatableUIRequiresAccessComponent>.op_Implicit(activatableUI)))
      return;
    args.Cancel();
    if (!activatableUI.Comp.PopupMessage.HasValue)
      return;
    SharedPopupSystem popup = this._popup;
    ILocalizationManager loc = this.Loc;
    LocId? popupMessage = activatableUI.Comp.PopupMessage;
    string str = popupMessage.HasValue ? LocId.op_Implicit(popupMessage.GetValueOrDefault()) : (string) null;
    string message = loc.GetString(str);
    EntityUid uid = Entity<ActivatableUIRequiresAccessComponent>.op_Implicit(activatableUI);
    EntityUid? recipient = new EntityUid?(args.User);
    popup.PopupClient(message, uid, recipient);
  }
}
