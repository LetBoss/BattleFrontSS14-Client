// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Pheromones;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesSystem : SharedXenoPheromonesSystem
{
  [Dependency]
  private IOverlayManager _overlays;

  public override void Initialize()
  {
    base.Initialize();
    this._overlays.AddOverlay((Overlay) new XenoPheromonesOverlay());
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlays.RemoveOverlay<XenoPheromonesOverlay>();
  }
}
