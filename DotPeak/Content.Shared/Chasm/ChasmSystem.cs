// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chasm.ChasmSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Chasm;

public sealed class ChasmSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChasmComponent, StepTriggeredOffEvent>(new ComponentEventRefHandler<ChasmComponent, StepTriggeredOffEvent>((object) this, __methodptr(OnStepTriggered)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChasmComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<ChasmComponent, StepTriggerAttemptEvent>((object) this, __methodptr(OnStepTriggerAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChasmFallingComponent, UpdateCanMoveEvent>(new ComponentEventHandler<ChasmFallingComponent, UpdateCanMoveEvent>((object) this, __methodptr(OnUpdateCanMove)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    EntityQueryEnumerator<ChasmFallingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ChasmFallingComponent>();
    EntityUid entity;
    ChasmFallingComponent fallingComponent;
    while (entityQueryEnumerator.MoveNext(ref entity, ref fallingComponent))
    {
      if (!(this._timing.CurTime < fallingComponent.NextDeletionTime))
      {
        MobStateComponent component;
        if (this.TryComp<MobStateComponent>(entity, ref component))
          this._mobState.ChangeMobState(entity, MobState.Dead, component);
        this.QueueDel(new EntityUid?(entity));
      }
    }
  }

  private void OnStepTriggered(
    EntityUid uid,
    ChasmComponent component,
    ref StepTriggeredOffEvent args)
  {
    if (this.HasComp<ChasmFallingComponent>(args.Tripper))
      return;
    this.StartFalling(uid, component, args.Tripper);
  }

  public void StartFalling(
    EntityUid chasm,
    ChasmComponent component,
    EntityUid tripper,
    bool playSound = true)
  {
    ChasmFallingComponent fallingComponent = this.AddComp<ChasmFallingComponent>(tripper);
    fallingComponent.NextDeletionTime = this._timing.CurTime + fallingComponent.DeletionTime;
    this._blocker.UpdateCanMove(tripper);
    if (!playSound)
      return;
    this._audio.PlayPredicted(component.FallingSound, chasm, new EntityUid?(tripper), new AudioParams?());
  }

  private void OnStepTriggerAttempt(
    EntityUid uid,
    ChasmComponent component,
    ref StepTriggerAttemptEvent args)
  {
    args.Continue = true;
  }

  private void OnUpdateCanMove(
    EntityUid uid,
    ChasmFallingComponent component,
    UpdateCanMoveEvent args)
  {
    args.Cancel();
  }
}
