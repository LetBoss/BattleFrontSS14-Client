// Decompiled with JetBrains decompiler
// Type: Content.Client.Anomaly.Effects.ClientInnerBodyAnomalySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects;
using Content.Shared.Body.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Anomaly.Effects;

public sealed class ClientInnerBodyAnomalySystem : SharedInnerBodyAnomalySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InnerBodyAnomalyComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<InnerBodyAnomalyComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InnerBodyAnomalyComponent, ComponentShutdown>(new EntityEventRefHandler<InnerBodyAnomalyComponent, ComponentShutdown>((object) this, __methodptr(OnCompShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterHandleState(
    Entity<InnerBodyAnomalyComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref spriteComponent) || ent.Comp.FallbackSprite == null)
      return;
    int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), ent.Comp.LayerMap);
    BodyComponent bodyComponent;
    SpriteSpecifier spriteSpecifier;
    if (this.TryComp<BodyComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref bodyComponent) && bodyComponent.Prototype.HasValue && ent.Comp.SpeciesSprites.TryGetValue(bodyComponent.Prototype.Value, out spriteSpecifier))
      this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, spriteSpecifier);
    else
      this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, ent.Comp.FallbackSprite);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, true);
    spriteComponent.LayerSetShader(num, "unshaded");
  }

  private void OnCompShutdown(Entity<InnerBodyAnomalyComponent> ent, ref ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    int num = this._sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), ent.Comp.LayerMap);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, false);
  }
}
