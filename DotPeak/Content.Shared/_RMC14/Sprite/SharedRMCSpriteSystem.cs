// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sprite.SharedRMCSpriteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Sprite;

public abstract class SharedRMCSpriteSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SpriteSetRenderOrderComponent, MapInitEvent>(new EntityEventRefHandler<SpriteSetRenderOrderComponent, MapInitEvent>(this.OnSetRenderOrderMapInit));
    this.SubscribeLocalEvent<SpriteSetRenderOrderComponent, ItemPickedUpEvent>(new EntityEventRefHandler<SpriteSetRenderOrderComponent, ItemPickedUpEvent>(this.OnSetRenderOrderPickedUp));
  }

  private void OnSetRenderOrderMapInit(
    Entity<SpriteSetRenderOrderComponent> ent,
    ref MapInitEvent args)
  {
    if (ent.Comp.RenderOrder.HasValue)
      this._appearance.SetData((EntityUid) ent, (Enum) SpriteSetRenderOrderComponent.Appearance.Key, (object) ent.Comp.RenderOrder);
    if (!ent.Comp.Offset.HasValue)
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) SpriteSetRenderOrderComponent.Appearance.Offset, (object) ent.Comp.Offset);
  }

  private void OnSetRenderOrderPickedUp(
    Entity<SpriteSetRenderOrderComponent> ent,
    ref ItemPickedUpEvent args)
  {
    if (!ent.Comp.Offset.HasValue)
      return;
    Vector2? offset = ent.Comp.Offset;
    Vector2 zero = Vector2.Zero;
    if ((offset.HasValue ? (offset.GetValueOrDefault() == zero ? 1 : 0) : 0) != 0)
      return;
    ent.Comp.Offset = new Vector2?(Vector2.Zero);
    this.Dirty<SpriteSetRenderOrderComponent>(ent);
  }

  public void SetOffset(EntityUid ent, Vector2 offset)
  {
    SpriteSetRenderOrderComponent renderOrderComponent = this.EnsureComp<SpriteSetRenderOrderComponent>(ent);
    renderOrderComponent.Offset = new Vector2?(offset);
    this.Dirty(ent, (IComponent) renderOrderComponent);
  }

  public void SetRenderOrder(EntityUid ent, int order)
  {
    SpriteSetRenderOrderComponent renderOrderComponent = this.EnsureComp<SpriteSetRenderOrderComponent>(ent);
    renderOrderComponent.RenderOrder = new int?(order);
    this.Dirty(ent, (IComponent) renderOrderComponent);
  }

  public void SetColor(Entity<SpriteColorComponent?> ent, Color color)
  {
    ent.Comp = this.EnsureComp<SpriteColorComponent>((EntityUid) ent);
    ent.Comp.Color = color;
    this.Dirty<SpriteColorComponent>(ent);
  }

  public Content.Shared.DrawDepth.DrawDepth GetDrawDepth(EntityUid ent, Content.Shared.DrawDepth.DrawDepth current = Content.Shared.DrawDepth.DrawDepth.Mobs)
  {
    GetDrawDepthEvent args = new GetDrawDepthEvent(current);
    this.RaiseLocalEvent<GetDrawDepthEvent>(ent, ref args);
    return args.DrawDepth;
  }

  public virtual Content.Shared.DrawDepth.DrawDepth UpdateDrawDepth(EntityUid sprite)
  {
    Content.Shared.DrawDepth.DrawDepth drawDepth = this.GetDrawDepth(sprite);
    this._appearance.SetData(sprite, (Enum) RMCSpriteDrawDepth.Key, (object) drawDepth);
    return drawDepth;
  }
}
