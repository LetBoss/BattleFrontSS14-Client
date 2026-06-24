// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.ActivatableUIRequiresVisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class ActivatableUIRequiresVisionSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedUserInterfaceSystem _userInterfaceSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>(this.OnOpenAttempt));
    this.SubscribeLocalEvent<BlindableComponent, BlindnessChangedEvent>(new ComponentEventRefHandler<BlindableComponent, BlindnessChangedEvent>(this.OnBlindnessChanged));
  }

  private void OnOpenAttempt(
    EntityUid uid,
    ActivatableUIRequiresVisionComponent component,
    ActivatableUIOpenAttemptEvent args)
  {
    BlindableComponent comp;
    if (args.Cancelled || !this.TryComp<BlindableComponent>(args.User, out comp) || !comp.IsBlind)
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("blindness-fail-attempt"), new EntityUid?(args.User), PopupType.MediumCaution);
    args.Cancel();
  }

  private void OnBlindnessChanged(
    EntityUid uid,
    BlindableComponent component,
    ref BlindnessChangedEvent args)
  {
    if (!args.Blind)
      return;
    ValueList<(EntityUid, Enum)> valueList = new ValueList<(EntityUid, Enum)>();
    foreach ((EntityUid Entity, Enum Key) actorUi in this._userInterfaceSystem.GetActorUis((Entity<UserInterfaceUserComponent>) uid))
    {
      if (this.HasComp<ActivatableUIRequiresVisionComponent>(actorUi.Entity))
        valueList.Add(actorUi);
    }
    foreach ((EntityUid, Enum) valueTuple in valueList)
      this._userInterfaceSystem.CloseUi((Entity<UserInterfaceComponent>) valueTuple.Item1, valueTuple.Item2, new EntityUid?(uid));
  }
}
