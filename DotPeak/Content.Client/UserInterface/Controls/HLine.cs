// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.HLine
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class HLine : Container
{
  private readonly PanelContainer _line;

  public Robust.Shared.Maths.Color? Color
  {
    get
    {
      return this._line.PanelOverride is StyleBoxFlat panelOverride ? new Robust.Shared.Maths.Color?(panelOverride.BackgroundColor) : new Robust.Shared.Maths.Color?();
    }
    set
    {
      if (!(this._line.PanelOverride is StyleBoxFlat panelOverride))
        return;
      panelOverride.BackgroundColor = value.Value;
    }
  }

  public float? Thickness
  {
    get
    {
      return this._line.PanelOverride is StyleBoxFlat panelOverride ? ((StyleBox) panelOverride).ContentMarginTopOverride : new float?();
    }
    set
    {
      if (!(this._line.PanelOverride is StyleBoxFlat panelOverride))
        return;
      ((StyleBox) panelOverride).ContentMarginTopOverride = new float?(value.Value);
    }
  }

  public HLine()
  {
    this._line = new PanelContainer();
    this._line.PanelOverride = (StyleBox) new StyleBoxFlat();
    this._line.PanelOverride.ContentMarginTopOverride = this.Thickness;
    ((Control) this).AddChild((Control) this._line);
  }
}
