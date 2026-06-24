// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Doom.DoomOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Doom;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Doom;

public sealed class DoomOverlay : Overlay
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntityManager _entityManager;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public DoomOverlay()
  {
    IoCManager.InjectDependencies<DoomOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCDoomVision")).InstanceUnique();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    MobDoomedComponent mobDoomedComponent;
    if (!this._entityManager.TryGetComponent<MobDoomedComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref mobDoomedComponent) || this.ScreenTexture == null || args.Viewport.Eye == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._shader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
