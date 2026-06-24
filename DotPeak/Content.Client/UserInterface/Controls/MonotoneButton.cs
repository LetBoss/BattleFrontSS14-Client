// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.MonotoneButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public sealed class MonotoneButton : Button
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color AltTextColor { set; get; } = new Color(0.2f, 0.2f, 0.2f, 1f);

  public MonotoneButton()
  {
    ((Control) this).RemoveStyleClass("button");
    this.UpdateAppearance();
  }

  private void UpdateAppearance()
  {
    if (this.Label != null)
      ((Control) this.Label).ModulateSelfOverride = ((BaseButton) this).DrawMode == 1 ? new Color?(this.AltTextColor) : new Color?();
    ((Control) this).Modulate = ((BaseButton) this).Disabled ? Color.Gray : Color.White;
  }

  protected virtual void StylePropertiesChanged()
  {
    base.StylePropertiesChanged();
    this.UpdateAppearance();
  }

  protected virtual void DrawModeChanged()
  {
    ((ContainerButton) this).DrawModeChanged();
    this.UpdateAppearance();
  }
}
