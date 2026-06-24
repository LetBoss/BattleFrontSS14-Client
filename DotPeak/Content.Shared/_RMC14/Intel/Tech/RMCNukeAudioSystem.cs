// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.Tech.RMCNukeAudioSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Intel.Tech;

public sealed class RMCNukeAudioSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  private List<SoundPathSpecifier> _audios = new List<SoundPathSpecifier>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCNukeAudioEvent>(new EntityEventHandler<RMCNukeAudioEvent>(this.OnNukeAudio));
  }

  private void OnNukeAudio(RMCNukeAudioEvent ev)
  {
    int index = new Random().Next(2);
    this._audios.Add(new SoundPathSpecifier("/Audio/_RMC14/Machines/Nuke/nuke.ogg"));
    this._audios.Add(new SoundPathSpecifier("/Audio/_RMC14/Machines/Nuke/nuke2-1.ogg"));
    this._audio.PlayGlobal((SoundSpecifier) this._audios[index], Filter.Broadcast(), true);
  }
}
