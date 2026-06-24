// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.DropshipSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dropship;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Dropship;

public sealed class DropshipSystem : SharedDropshipSystem
{
  public readonly List<DropshipNavigationBui> Uis = new List<DropshipNavigationBui>();

  public virtual void FrameUpdate(float frameTime)
  {
    foreach (BoundUserInterface ui in this.Uis)
      ui.Update();
  }
}
