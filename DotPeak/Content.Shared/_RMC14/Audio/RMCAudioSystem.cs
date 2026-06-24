// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Audio.RMCAudioSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.Audio;

public sealed class RMCAudioSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeNetworkEvent<RMCAudioPlayGlobalEvent>(new EntityEventHandler<RMCAudioPlayGlobalEvent>(this.OnPlayGlobal));
  }

  private void OnPlayGlobal(RMCAudioPlayGlobalEvent msg)
  {
    if (this._net.IsServer)
      return;
    (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayGlobal(msg.Sound, Filter.Local(), true, new AudioParams?(msg.AudioParams));
    if (!nullable.HasValue || this.EntityManager.HasComponent(nullable.Value.Entity, msg.Component, (MetaDataComponent) null))
      return;
    this.EntityManager.AddComponent(nullable.Value.Entity, msg.Component, (MetaDataComponent) null);
  }

  public void PlayGlobal<T>(SoundSpecifier sound, AudioParams audioParams) where T : IComponent, new()
  {
    ushort? netId = this._compFactory.GetRegistration<T>().NetID;
    if (netId.HasValue)
    {
      ushort valueOrDefault = netId.GetValueOrDefault();
      this.RaiseNetworkEvent((EntityEventArgs) new RMCAudioPlayGlobalEvent(sound, audioParams, valueOrDefault));
    }
    if (this._net.IsClient)
      return;
    (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayGlobal(sound, Filter.Empty(), true, new AudioParams?(audioParams));
    if (!nullable.HasValue)
      return;
    this.EnsureComp<T>(nullable.Value.Entity);
  }
}
