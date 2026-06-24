// Decompiled with JetBrains decompiler
// Type: Content.Client.Decals.ToggleDecalCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Decals;

public sealed class ToggleDecalCommand : LocalizedEntityCommands
{
  [Dependency]
  private DecalSystem _decal;

  public virtual string Command => "toggledecals";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    this._decal.ToggleOverlay();
  }
}
