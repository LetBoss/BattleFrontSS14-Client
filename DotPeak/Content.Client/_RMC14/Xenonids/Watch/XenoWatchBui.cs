// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Watch.XenoWatchBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Watch;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Watch;

public sealed class XenoWatchBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoWatchWindow? _window;
  private readonly SpriteSystem _sprite;

  public XenoWatchBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this.EnsureWindow();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is XenoWatchBuiState xenoWatchBuiState))
      return;
    this._window = this.EnsureWindow();
    this._window.BurrowedLarvaLabel.Text = $"Burrowed Larva: {xenoWatchBuiState.BurrowedLarva}";
    ((Control) this._window.XenoContainer).DisposeAllChildren();
    foreach (Xeno xeno1 in xenoWatchBuiState.Xenos)
    {
      Xeno xeno = xeno1;
      Texture texture = (Texture) null;
      EntityPrototype entityPrototype;
      if (xeno.Id.HasValue && this._prototype.TryIndex(xeno.Id.Value, ref entityPrototype))
        texture = this._sprite.Frame0(entityPrototype);
      XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
      xenoChoiceControl.Set(xeno.Name, texture);
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoWatchBuiMsg(xeno.Entity)));
      ((Control) this._window.XenoContainer).AddChild((Control) xenoChoiceControl);
    }
  }

  private XenoWatchWindow EnsureWindow()
  {
    if (this._window != null)
      return this._window;
    this._window = BoundUserInterfaceExt.CreateWindow<XenoWatchWindow>((BoundUserInterface) this);
    this._window.SearchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearchBarChanged);
    return this._window;
  }

  private void OnSearchBarChanged(LineEdit.LineEditEventArgs args)
  {
    XenoWatchWindow window = this._window;
    if (window == null || ((Control) window).Disposed)
      return;
    foreach (Control child in ((Control) this._window.XenoContainer).Children)
    {
      if (child is XenoChoiceControl xenoChoiceControl1)
      {
        if (string.IsNullOrWhiteSpace(args.Text))
        {
          xenoChoiceControl1.Visible = true;
        }
        else
        {
          XenoChoiceControl xenoChoiceControl = xenoChoiceControl1;
          string message = xenoChoiceControl1.NameLabel.GetMessage();
          int num = message != null ? (message.Contains(args.Text, StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 0;
          xenoChoiceControl.Visible = num != 0;
        }
      }
    }
  }
}
