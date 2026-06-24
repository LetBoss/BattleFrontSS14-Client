// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivExplosionParticlesSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Particles;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivExplosionParticlesSystem : EntitySystem
{
  [Dependency]
  private readonly CivLocalParticleSystem _particles;
  private static readonly ProtoId<CivEmitterPresetPrototype> Dust = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterExplosionDust");
  private static readonly ProtoId<CivEmitterPresetPrototype> Smoke = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterExplosionSmoke");
  private HashSet<EntityUid> _seen = new HashSet<EntityUid>();
  private HashSet<EntityUid> _current = new HashSet<EntityUid>();

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    this._current.Clear();
    EntityQueryEnumerator<ExplosionVisualsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ExplosionVisualsComponent>();
    EntityUid entityUid;
    ExplosionVisualsComponent visualsComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent))
    {
      this._current.Add(entityUid);
      if (!this._seen.Contains(entityUid))
      {
        List<float> intensity = visualsComponent.Intensity;
        if (intensity != null && intensity.Count > 0)
        {
          float scale = Math.Clamp((float) visualsComponent.Intensity.Count / 4f, 0.6f, 2.5f);
          this._particles.EmitBurst(CivExplosionParticlesSystem.Dust, visualsComponent.Epicenter, scale);
          this._particles.EmitBurst(CivExplosionParticlesSystem.Smoke, visualsComponent.Epicenter, scale);
        }
      }
    }
    HashSet<EntityUid> current = this._current;
    HashSet<EntityUid> seen = this._seen;
    this._seen = current;
    this._current = seen;
  }
}
