// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedSpriteMovementSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedSpriteMovementSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpriteMovementComponent, SpriteMoveEvent>(new EntityEventRefHandler<SpriteMovementComponent, SpriteMoveEvent>(this.OnSpriteMoveInput));
  }

  private void OnSpriteMoveInput(Entity<SpriteMovementComponent> ent, ref SpriteMoveEvent args)
  {
    if (ent.Comp.IsMoving == args.IsMoving)
      return;
    ent.Comp.IsMoving = args.IsMoving;
    this.Dirty<SpriteMovementComponent>(ent);
  }
}
