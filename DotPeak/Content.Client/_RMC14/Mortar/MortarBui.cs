// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mortar.MortarBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Mortar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Mortar;

public sealed class MortarBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private MortarWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<MortarWindow>((BoundUserInterface) this);
    this.Refresh();
    MortarComponent mortarComponent;
    if (this.EntMan.TryGetComponent<MortarComponent>(this.Owner, ref mortarComponent))
    {
      SetSpinBox(this._window.TargetX, mortarComponent.MaxTarget, mortarComponent.Target.X);
      SetSpinBox(this._window.TargetY, mortarComponent.MaxTarget, mortarComponent.Target.Y);
      SetSpinBox(this._window.DialX, mortarComponent.MaxDial, mortarComponent.Dial.X);
      SetSpinBox(this._window.DialY, mortarComponent.MaxDial, mortarComponent.Dial.Y);
      ((BaseButton) this._window.SetTargetButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MortarTargetBuiMsg(Vector2i.op_Implicit((Parse(this._window.TargetX), Parse(this._window.TargetY))))));
      ((BaseButton) this._window.SetOffsetButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MortarDialBuiMsg(Vector2i.op_Implicit((Parse(this._window.DialX), Parse(this._window.DialY))))));
    }
    ((BaseButton) this._window.ViewCameraButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MortarViewCamerasMsg()));

    static int Parse(FloatSpinBox spinBox) => (int) spinBox.Value;

    static void SetSpinBox(FloatSpinBox spinBox, int limit, int value)
    {
      spinBox.Value = (float) value;
      spinBox.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => spinBox.Value = Math.Clamp(args.Value, (float) -limit, (float) limit));
    }
  }

  public void Refresh()
  {
    MortarWindow window = this._window;
    MortarComponent mortarComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<MortarComponent>(this.Owner, ref mortarComponent))
      return;
    SetValue(this._window.TargetX, mortarComponent.Target.X);
    SetValue(this._window.TargetY, mortarComponent.Target.Y);
    SetValue(this._window.DialX, mortarComponent.Dial.X);
    SetValue(this._window.DialY, mortarComponent.Dial.Y);
    this._window.MaxDialLabel.Text = Loc.GetString("rmc-mortar-offset-max", new (string, object)[1]
    {
      ("max", (object) mortarComponent.MaxDial)
    });

    static void SetValue(FloatSpinBox? spinBox, int value)
    {
      if (spinBox == null)
        return;
      spinBox.Value = (float) value;
    }
  }
}
