// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.ButtonHelpers
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.UserInterface;

public static class ButtonHelpers
{
  public static void SetButtonDisabledRecursive(Control parent, bool val)
  {
    foreach (Control child in parent.Children)
    {
      if (child is Button button)
        ((BaseButton) button).Disabled = val;
      else if (child.ChildCount > 0)
        ButtonHelpers.SetButtonDisabledRecursive(child, val);
    }
  }
}
