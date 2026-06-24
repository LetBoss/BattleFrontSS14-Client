// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATDeathSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATDeathSystem : BaseXATSystem<XATDeathComponent>
{
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._xenoArtifactQuery = this.GetEntityQuery<XenoArtifactComponent>();
    this.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnMobStateChanged(MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    EntityCoordinates coordinates1 = this.Transform(args.Target).Coordinates;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XATDeathComponent, XenoArtifactNodeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XATDeathComponent, XenoArtifactNodeComponent>();
    EntityUid uid;
    XATDeathComponent comp1;
    XenoArtifactNodeComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp2.Attached.HasValue)
      {
        Entity<XenoArtifactComponent> entity = this._xenoArtifactQuery.Get(this.GetEntity(comp2.Attached.Value));
        if (this.CanTrigger(entity, (Entity<XenoArtifactNodeComponent>) (uid, comp2)))
        {
          EntityCoordinates coordinates2 = this.Transform((EntityUid) entity).Coordinates;
          if (this._transform.InRange(coordinates1, coordinates2, comp1.Range))
            this.Trigger(entity, (Entity<XATDeathComponent, XenoArtifactNodeComponent>) (uid, comp1, comp2));
        }
      }
    }
  }
}
