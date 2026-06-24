// Decompiled with JetBrains decompiler
// Type: Content.Client.Sticky.Visualizers.StickyVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Sticky.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Sticky.Visualizers;

public sealed class StickyVisualizerSystem : VisualizerSystem<StickyVisualizerComponent>
{
  private EntityQuery<SpriteComponent> _spriteQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._spriteQuery = ((EntitySystem) this).GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<StickyVisualizerComponent, ComponentInit>(new EntityEventRefHandler<StickyVisualizerComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
  }

  private void OnInit(Entity<StickyVisualizerComponent> ent, ref ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this._spriteQuery.TryComp(Entity<StickyVisualizerComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    ent.Comp.OriginalDrawDepth = spriteComponent.DrawDepth;
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    StickyVisualizerComponent comp,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) StickyVisuals.IsStuck, ref flag, args.Component))
      return;
    int num = flag ? comp.StuckDrawDepth : comp.OriginalDrawDepth;
    this.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num);
  }
}
