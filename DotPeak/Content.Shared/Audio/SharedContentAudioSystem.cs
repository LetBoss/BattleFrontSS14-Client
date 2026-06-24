// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.SharedContentAudioSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Audio;

public abstract class SharedContentAudioSystem : EntitySystem
{
  [Dependency]
  protected SharedAudioSystem Audio;
  public const float DefaultVariation = 0.05f;

  public virtual void Initialize()
  {
    base.Initialize();
    this.Audio.OcclusionCollisionMask = 2;
  }

  protected void SilenceAudio()
  {
    AllEntityQueryEnumerator<AudioComponent> entityQueryEnumerator = this.AllEntityQuery<AudioComponent>();
    EntityUid entityUid;
    AudioComponent audioComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref audioComponent))
      this.Audio.SetGain(new EntityUid?(entityUid), 0.0f, audioComponent);
  }
}
