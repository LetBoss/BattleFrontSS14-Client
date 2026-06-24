// Decompiled with JetBrains decompiler
// Type: Content.Client.Access.Commands.ShowAccessReadersCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Access.Commands;

public sealed class ShowAccessReadersCommand : LocalizedEntityCommands
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private SharedTransformSystem _xform;

  public virtual string Command => "showaccessreaders";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    bool flag = this._overlay.RemoveOverlay<AccessOverlay>();
    if (!flag)
      this._overlay.AddOverlay((Overlay) new AccessOverlay((IEntityManager) this.EntityManager, this._cache, this._xform));
    shell.WriteLine(((LocalizedCommands) this).Loc.GetString("cmd-showaccessreaders-status", ("status", (object) !flag)));
  }
}
