// Decompiled with JetBrains decompiler
// Type: Content.Client.HealthAnalyzer.UI.HealthAnalyzerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.MedicalScanner;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.HealthAnalyzer.UI;

public sealed class HealthAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private HealthAnalyzerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<HealthAnalyzerWindow>((BoundUserInterface) this);
    this._window.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (this._window == null || !(message is HealthAnalyzerScannedUserMessage msg))
      return;
    this._window.Populate(msg);
  }
}
