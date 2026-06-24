// Decompiled with JetBrains decompiler
// Type: Content.Client.Physics.JointVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Physics;

public sealed class JointVisualsSystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new JointVisualsOverlay((IEntityManager) this.EntityManager));
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<JointVisualsOverlay>();
  }
}
