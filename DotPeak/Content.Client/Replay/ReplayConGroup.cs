// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.ReplayConGroup
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Console;
using System;

#nullable enable
namespace Content.Client.Replay;

public sealed class ReplayConGroup : IClientConGroupImplementation
{
  public event Action? ConGroupUpdated
  {
    add
    {
    }
    remove
    {
    }
  }

  public bool CanAdminMenu() => true;

  public bool CanAdminPlace() => true;

  public bool CanCommand(string cmdName) => true;

  public bool CanScript() => true;

  public bool CanViewVar() => true;
}
