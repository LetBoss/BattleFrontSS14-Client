// Decompiled with JetBrains decompiler
// Type: Content.Client.CombatMode.ColoredScreenBorderOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client.CombatMode;

public sealed class ColoredScreenBorderOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("ColoredScreenBorder");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public ColoredScreenBorderOverlay()
  {
    IoCManager.InjectDependencies<ColoredScreenBorderOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(ColoredScreenBorderOverlay.Shader).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(args.WorldAABB, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
