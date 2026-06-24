// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Auto.ShowAutoFireSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons.Ranged.Auto;
using Robust.Client.Graphics;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Auto;

public sealed class ShowAutoFireSystem : EntitySystem
{
  [Dependency]
  private IConsoleHost _console;
  [Dependency]
  private GunToggleableAutoFireSystem _autoFire;
  [Dependency]
  private IOverlayManager _overlay;

  public virtual void Initialize()
  {
    this._console.RegisterCommand("showautofire", this.Loc.GetString("cmd-showautofire-desc"), this.Loc.GetString("cmd-showautofire-help"), new ConCommandCallback(this.ShowAutoFireCommand), false);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._console.UnregisterCommand("showautofire");
    this._overlay.RemoveOverlay<ShowAutoFireOverlay>();
  }

  private void ShowAutoFireCommand(IConsoleShell shell, string argstr, string[] args)
  {
    if (!this._overlay.RemoveOverlay<ShowAutoFireOverlay>())
    {
      this._autoFire.Debug = true;
      this._overlay.AddOverlay((Overlay) new ShowAutoFireOverlay());
      shell.WriteLine(this.Loc.GetString("cmd-showautofire-enabled"));
    }
    else
    {
      this._autoFire.Debug = false;
      shell.WriteLine(this.Loc.GetString("cmd-showautofire-disabled"));
    }
  }
}
