// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.ClientSpriteMovementSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class ClientSpriteMovementSystem : SharedSpriteMovementSystem
{
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<SpriteComponent> _spriteQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpriteMovementComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<SpriteMovementComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterAutoHandleState(
    Entity<SpriteMovementComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this._spriteQuery.TryGetComponent(Entity<SpriteMovementComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    foreach ((string key, PrototypeLayerData prototypeLayerData) in ent.Comp.IsMoving ? ent.Comp.MovementLayers : ent.Comp.NoMovementLayers)
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), key, ref num, false))
        this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), key, prototypeLayerData);
    }
  }
}
