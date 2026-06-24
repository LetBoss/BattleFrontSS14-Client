// Decompiled with JetBrains decompiler
// Type: Content.Client.CartridgeLoader.Cartridges.NewsReaderUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NewsReaderUi : 
  UIFragment,
  ISerializationGenerated<NewsReaderUi>,
  ISerializationGenerated
{
  private NewsReaderUiFragment? _fragment;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    this._fragment = new NewsReaderUiFragment();
    this._fragment.OnNextButtonPressed += (Action) (() => this.SendNewsReaderMessage(NewsReaderUiAction.Next, userInterface));
    this._fragment.OnPrevButtonPressed += (Action) (() => this.SendNewsReaderMessage(NewsReaderUiAction.Prev, userInterface));
    this._fragment.OnNotificationSwithPressed += (Action) (() => this.SendNewsReaderMessage(NewsReaderUiAction.NotificationSwitch, userInterface));
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    switch (state)
    {
      case NewsReaderBoundUserInterfaceState userInterfaceState1:
        this._fragment?.UpdateState(userInterfaceState1.Article, userInterfaceState1.TargetNum, userInterfaceState1.TotalNum, userInterfaceState1.NotificationOn);
        break;
      case NewsReaderEmptyBoundUserInterfaceState userInterfaceState2:
        this._fragment?.UpdateEmptyState(userInterfaceState2.NotificationOn);
        break;
    }
  }

  private void SendNewsReaderMessage(NewsReaderUiAction action, BoundUserInterface userInterface)
  {
    CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage((CartridgeMessageEvent) new NewsReaderUiMessageEvent(action));
    userInterface.SendMessage((BoundUserInterfaceMessage) cartridgeUiMessage);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NewsReaderUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NewsReaderUi) target1;
    serialization.TryCustomCopy<NewsReaderUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NewsReaderUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref UIFragment target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NewsReaderUi target1 = (NewsReaderUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (UIFragment) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NewsReaderUi target1 = (NewsReaderUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NewsReaderUi UIFragment.Instantiate() => new NewsReaderUi();
}
