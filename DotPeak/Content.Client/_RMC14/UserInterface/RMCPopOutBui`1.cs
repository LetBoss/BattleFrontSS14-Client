// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.UserInterface.RMCPopOutBui`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.UserInterface;

public abstract class RMCPopOutBui<T>(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
  where T : RMCPopOutWindow
{
  [Robust.Shared.ViewVariables.ViewVariables]
  protected abstract T? Window { get; set; }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    this.Window?.DisposePopOut();
  }
}
