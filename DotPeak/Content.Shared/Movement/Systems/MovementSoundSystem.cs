// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.MovementSoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class MovementSoundSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MovementSoundComponent, MoveInputEvent>(new EntityEventRefHandler<MovementSoundComponent, MoveInputEvent>(this.OnMoveInput));
  }

  private void OnMoveInput(Entity<MovementSoundComponent> ent, ref MoveInputEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    int num1 = (SharedMoverController.GetNormalizedMovement(args.OldMovement) & MoveButtons.AnyDirection) != 0 ? 1 : 0;
    bool flag = (SharedMoverController.GetNormalizedMovement(args.Entity.Comp.HeldMoveButtons) & MoveButtons.AnyDirection) != 0;
    int num2 = flag ? 1 : 0;
    if (num1 == num2)
      return;
    if (flag)
    {
      MovementSoundComponent comp = ent.Comp;
      (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPredicted(ent.Comp.Sound, ent.Owner, new EntityUid?(ent.Owner));
      ref (EntityUid, AudioComponent)? local = ref nullable1;
      EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      comp.SoundEntity = nullable2;
    }
    else
      ent.Comp.SoundEntity = this._audio.Stop(ent.Comp.SoundEntity);
  }
}
