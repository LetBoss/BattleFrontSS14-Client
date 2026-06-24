// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.BaseQueryUpdateXATSystem`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public abstract class BaseQueryUpdateXATSystem<T> : BaseXATSystem<T> where T : Component
{
  protected Robust.Shared.GameObjects.EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._xenoArtifactQuery = this.GetEntityQuery<XenoArtifactComponent>();
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<T, XenoArtifactNodeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<T, XenoArtifactNodeComponent>();
    EntityUid uid;
    T comp1;
    XenoArtifactNodeComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp2.Attached.HasValue)
      {
        Entity<XenoArtifactComponent> artifact = this._xenoArtifactQuery.Get(this.GetEntity(comp2.Attached.Value));
        if (this.CanTrigger(artifact, (Entity<XenoArtifactNodeComponent>) (uid, comp2)))
          this.UpdateXAT(artifact, (Entity<T, XenoArtifactNodeComponent>) (uid, comp1, comp2), frameTime);
      }
    }
  }

  protected abstract void UpdateXAT(
    Entity<XenoArtifactComponent> artifact,
    Entity<T, XenoArtifactNodeComponent> node,
    float frameTime);
}
