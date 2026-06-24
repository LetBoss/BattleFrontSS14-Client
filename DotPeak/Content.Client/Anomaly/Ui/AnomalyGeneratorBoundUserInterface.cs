// Decompiled with JetBrains decompiler
// Type: Content.Client.Anomaly.Ui.AnomalyGeneratorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Anomaly;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Anomaly.Ui;

public sealed class AnomalyGeneratorBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private AnomalyGeneratorWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<AnomalyGeneratorWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.Owner);
    this._window.OnGenerateButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new AnomalyGeneratorGenerateButtonPressedEvent()));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is AnomalyGeneratorUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }
}
