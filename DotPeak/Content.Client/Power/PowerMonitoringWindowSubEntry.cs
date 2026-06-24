// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringWindowSubEntry
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Power;

public sealed class PowerMonitoringWindowSubEntry : PowerMonitoringWindowBaseEntry
{
  public TextureRect? Icon;

  public PowerMonitoringWindowSubEntry(PowerMonitoringConsoleEntry entry)
    : base(entry)
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) this).HorizontalExpand = true;
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) textureRect).Margin = new Thickness(0.0f, 0.0f, 2f, 0.0f);
    this.Icon = textureRect;
    ((Control) this).AddChild((Control) this.Icon);
    ((Control) this.Button).StyleClasses.Add("OpenBoth");
    ((Control) this).AddChild((Control) this.Button);
  }
}
