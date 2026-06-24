// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.AdminLogImpactButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

#nullable disable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogImpactButton : Button
{
  public AdminLogImpactButton(LogImpact impact)
  {
    this.Impact = impact;
    ((BaseButton) this).ToggleMode = true;
    ((BaseButton) this).Pressed = true;
  }

  public LogImpact Impact { get; }
}
