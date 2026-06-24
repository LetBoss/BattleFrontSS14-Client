// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NightVision.NightVisionFilterOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.NightVision;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionFilterOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototypes;
  private static readonly ProtoId<ShaderPrototype> ShaderId = ProtoId<ShaderPrototype>.op_Implicit("RMCNightVision");
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public NightVisionFilterOverlay()
  {
    IoCManager.InjectDependencies<NightVisionFilterOverlay>(this);
    this._shader = this._prototypes.Index<ShaderPrototype>(NightVisionFilterOverlay.ShaderId).InstanceUnique();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    NightVisionComponent nightVisionComponent;
    if (!this._entity.TryGetComponent<NightVisionComponent>(((ISharedPlayerManager) this._players).LocalEntity, ref nightVisionComponent) || nightVisionComponent.State == NightVisionState.Off)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    if (!nightVisionComponent.Green || this.ScreenTexture == null)
      return;
    this._shader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
