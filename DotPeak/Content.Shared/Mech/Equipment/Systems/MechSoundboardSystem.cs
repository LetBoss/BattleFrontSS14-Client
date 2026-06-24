// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mech.Equipment.Systems.MechSoundboardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Mech.Equipment.Systems;

public sealed class MechSoundboardSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MechSoundboardComponent, MechEquipmentUiStateReadyEvent>(new ComponentEventHandler<MechSoundboardComponent, MechEquipmentUiStateReadyEvent>(this.OnUiStateReady));
    this.SubscribeLocalEvent<MechSoundboardComponent, MechEquipmentUiMessageRelayEvent>(new ComponentEventHandler<MechSoundboardComponent, MechEquipmentUiMessageRelayEvent>(this.OnSoundboardMessage));
  }

  private void OnUiStateReady(
    EntityUid uid,
    MechSoundboardComponent comp,
    MechEquipmentUiStateReadyEvent args)
  {
    IEnumerable<string> source = comp.Sounds.Select<SoundCollectionSpecifier, string>((Func<SoundCollectionSpecifier, string>) (sound => sound.Collection));
    MechSoundboardUiState soundboardUiState = new MechSoundboardUiState()
    {
      Sounds = source.ToList<string>()
    };
    args.States.Add(this.GetNetEntity(uid), (BoundUserInterfaceState) soundboardUiState);
  }

  private void OnSoundboardMessage(
    EntityUid uid,
    MechSoundboardComponent comp,
    MechEquipmentUiMessageRelayEvent args)
  {
    MechEquipmentComponent comp1;
    UseDelayComponent comp2;
    if (!(args.Message is MechSoundboardPlayMessage message) || !this.TryComp<MechEquipmentComponent>(uid, out comp1) || !comp1.EquipmentOwner.HasValue || message.Sound >= comp.Sounds.Count || this.TryComp<UseDelayComponent>(uid, out comp2) && !this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (uid, comp2), true))
      return;
    this._audio.PlayPvs((SoundSpecifier) comp.Sounds[message.Sound], uid);
  }
}
