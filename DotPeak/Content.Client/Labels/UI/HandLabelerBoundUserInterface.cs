// Decompiled with JetBrains decompiler
// Type: Content.Client.Labels.UI.HandLabelerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Labels;
using Content.Shared.Labels.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Labels.UI;

public sealed class HandLabelerBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IEntityManager _entManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private HandLabelerWindow? _window;

  public HandLabelerBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<HandLabelerBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<HandLabelerWindow>((BoundUserInterface) this);
    HandLabelerComponent labelerComponent;
    if (this._entManager.TryGetComponent<HandLabelerComponent>(this.Owner, ref labelerComponent))
      this._window.SetMaxLabelLength(labelerComponent.MaxLabelChars);
    this._window.OnLabelChanged += new Action<string>(this.OnLabelChanged);
    this.Reload();
  }

  private void OnLabelChanged(string newLabel)
  {
    HandLabelerComponent labelerComponent;
    if (this._entManager.TryGetComponent<HandLabelerComponent>(this.Owner, ref labelerComponent) && labelerComponent.AssignedLabel.Equals(newLabel))
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new HandLabelerLabelChangedMessage(newLabel));
  }

  public void Reload()
  {
    HandLabelerComponent labelerComponent;
    if (this._window == null || !this._entManager.TryGetComponent<HandLabelerComponent>(this.Owner, ref labelerComponent))
      return;
    this._window.SetCurrentLabel(labelerComponent.AssignedLabel);
  }
}
