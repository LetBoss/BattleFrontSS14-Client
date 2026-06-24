// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringWindowEntry
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Power;

public sealed class PowerMonitoringWindowEntry : PowerMonitoringWindowBaseEntry
{
  public BoxContainer MainContainer;
  public BoxContainer SourcesContainer;
  public BoxContainer LoadsContainer;

  public PowerMonitoringWindowEntry(PowerMonitoringConsoleEntry entry)
    : base(entry)
  {
    this.Entry = entry;
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    ((Control) this.Button).StyleClasses.Add("OpenLeft");
    ((Control) this).AddChild((Control) this.Button);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(8f, 0.0f, 0.0f, 0.0f);
    ((Control) boxContainer1).Visible = false;
    this.MainContainer = boxContainer1;
    ((Control) this).AddChild((Control) this.MainContainer);
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).HorizontalExpand = true;
    this.SourcesContainer = boxContainer2;
    ((Control) this.MainContainer).AddChild((Control) this.SourcesContainer);
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalExpand = true;
    this.LoadsContainer = boxContainer3;
    ((Control) this.MainContainer).AddChild((Control) this.LoadsContainer);
  }
}
