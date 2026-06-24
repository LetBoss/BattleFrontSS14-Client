// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.UserInterface.UIExtensions
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.UserInterface;

public static class UIExtensions
{
  public static FloatSpinBox CreateDialSpinBox(
    float value = 0.0f,
    Action<FloatSpinBox.FloatSpinBoxEventArgs>? onValueChanged = null,
    bool buttons = true,
    int minWidth = 130)
  {
    FloatSpinBox floatSpinBox = new FloatSpinBox(1f, (byte) 0);
    ((Control) floatSpinBox).MinWidth = (float) minWidth;
    FloatSpinBox parent = floatSpinBox;
    parent.Value = value;
    parent.OnValueChanged += onValueChanged;
    if (!buttons)
    {
      foreach (Control control in ((Control) parent).GetControlOfType<Button>())
        control.Visible = false;
    }
    return parent;
  }

  public static T CreatePopOutableWindow<T>(this BoundUserInterface bui) where T : RMCPopOutWindow, new()
  {
    T disposableControl = BoundUserInterfaceExt.CreateDisposableControl<T>(bui);
    disposableControl.OnFinalClose += new Action(bui.Close);
    Vector2 vector2;
    if (((SharedUserInterfaceSystem) IoCManager.Resolve<IEntityManager>().System<UserInterfaceSystem>()).TryGetPosition(Entity<UserInterfaceComponent>.op_Implicit(bui.Owner), bui.UiKey, ref vector2))
      ((BaseWindow) (object) disposableControl).Open(vector2);
    else
      ((BaseWindow) (object) disposableControl).OpenCentered();
    return disposableControl;
  }

  public static void RemoveChildExcept(this Control parent, Control except)
  {
    for (int index = parent.ChildCount - 1; index >= 0; --index)
    {
      if (parent.GetChild(index) != except)
        parent.RemoveChild(index);
    }
  }

  public static void RemoveChildrenAfter(this Control parent, int after)
  {
    for (int index = parent.ChildCount - 1; index >= after; --index)
      parent.RemoveChild(index);
  }

  public static void SetTabVisibleAfter(this Control parent, int after, bool visible)
  {
    for (int index = parent.ChildCount - 1; index >= after; --index)
      TabContainer.SetTabVisible(parent.GetChild(index), visible);
  }

  public static void SetVisibleAfter(this Control parent, int after, bool visible)
  {
    for (int index = parent.ChildCount - 1; index >= after; --index)
      parent.GetChild(index).Visible = visible;
  }
}
