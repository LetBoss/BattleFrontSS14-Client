// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.PillSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class PillSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PillComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<PillComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    PillComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    SpriteComponent.Layer layer;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || !this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, ref layer, false))
      return;
    this._sprite.LayerSetRsiState(layer, RSI.StateId.op_Implicit($"pill{component.PillType + 1U}"), false);
  }
}
