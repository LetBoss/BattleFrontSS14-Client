// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Commands.ShowBiomeCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Parallax.Commands;

public sealed class ShowBiomeCommand : LocalizedCommands
{
  [Dependency]
  private IOverlayManager _overlayMgr;

  public virtual string Command => "showbiome";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._overlayMgr.HasOverlay<BiomeDebugOverlay>())
      this._overlayMgr.RemoveOverlay<BiomeDebugOverlay>();
    else
      this._overlayMgr.AddOverlay((Overlay) new BiomeDebugOverlay());
  }
}
