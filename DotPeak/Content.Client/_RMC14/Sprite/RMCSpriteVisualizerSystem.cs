// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Sprite.RMCSpriteVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Outline;
using Content.Shared._RMC14.Sprite;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteVisualizerSystem : VisualizerSystem<SpriteSetRenderOrderComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    ((EntitySystem) this).UpdatesAfter.Add(typeof (InteractionOutlineSystem));
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    SpriteSetRenderOrderComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    int num;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) SpriteSetRenderOrderComponent.Appearance.Key, ref num, args.Component))
      args.Sprite.RenderOrder = (uint) num;
    Vector2 vector2;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Vector2>(uid, (Enum) SpriteSetRenderOrderComponent.Appearance.Offset, ref vector2, args.Component))
      return;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), vector2);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    AllEntityQueryEnumerator<SpriteSetRenderOrderComponent, SpriteComponent> entityQueryEnumerator = ((EntitySystem) this).AllEntityQuery<SpriteSetRenderOrderComponent, SpriteComponent>();
    EntityUid entityUid;
    SpriteSetRenderOrderComponent renderOrderComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref renderOrderComponent, ref spriteComponent))
    {
      if (renderOrderComponent.RenderOrder.HasValue)
        spriteComponent.RenderOrder = (uint) renderOrderComponent.RenderOrder.Value;
      if (renderOrderComponent.Offset.HasValue)
        this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), renderOrderComponent.Offset.Value);
    }
  }
}
