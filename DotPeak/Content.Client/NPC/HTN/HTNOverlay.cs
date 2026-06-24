// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.HTN.HTNOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.NPC.HTN;

public sealed class HTNOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly Font _font;
  private readonly SharedTransformSystem _transformSystem;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public HTNOverlay(IEntityManager entManager, IResourceCache resourceCache)
  {
    this._entManager = entManager;
    this._font = (Font) new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
    this._transformSystem = this._entManager.System<SharedTransformSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    foreach ((HTNComponent htnComponent, TransformComponent transformComponent) in this._entManager.EntityQuery<HTNComponent, TransformComponent>(true))
    {
      if (!string.IsNullOrEmpty(htnComponent.DebugText) && !MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Vector2 worldPosition = this._transformSystem.GetWorldPosition(transformComponent);
        if (((Box2) ref args.WorldAABB).Contains(worldPosition, true))
        {
          Vector2 screen = args.ViewportControl.WorldToScreen(worldPosition);
          screenHandle.DrawString(this._font, screen + new Vector2(0.0f, 10f), htnComponent.DebugText, Color.White);
        }
      }
    }
  }
}
