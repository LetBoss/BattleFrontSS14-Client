// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedJumpAbilitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class SharedJumpAbilitySystem : EntitySystem
{
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGravitySystem _gravity;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<JumpAbilityComponent, GravityJumpEvent>(new EntityEventRefHandler<JumpAbilityComponent, GravityJumpEvent>(this.OnGravityJump));
  }

  private void OnGravityJump(Entity<JumpAbilityComponent> entity, ref GravityJumpEvent args)
  {
    if (this._gravity.IsWeightless(args.Performer))
      return;
    TransformComponent transformComponent = this.Transform(args.Performer);
    Angle localRotation = transformComponent.LocalRotation;
    EntityCoordinates coordinates = transformComponent.Coordinates.Offset(((Angle) ref localRotation).ToWorldVec() * entity.Comp.JumpDistance);
    this._throwing.TryThrow(args.Performer, coordinates, entity.Comp.JumpThrowSpeed);
    this._audio.PlayPredicted(entity.Comp.JumpSound, args.Performer, new EntityUid?(args.Performer));
    args.Handled = true;
  }
}
