// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.AdminLogPlayerButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using System;

#nullable disable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogPlayerButton : Button
{
  public AdminLogPlayerButton(Guid id)
  {
    this.Id = id;
    this.ClipText = true;
    ((BaseButton) this).ToggleMode = true;
  }

  public Guid Id { get; }
}
