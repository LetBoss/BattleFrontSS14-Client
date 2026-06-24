// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAERemoveCollisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAERemoveCollisionSystem : BaseXAESystem<XAERemoveCollisionComponent>
{
  [Dependency]
  private SharedPhysicsSystem _physics;

  protected override void OnActivated(
    Entity<XAERemoveCollisionComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>(ent.Owner, out comp))
      return;
    foreach (Fixture fixture in comp.Fixtures.Values)
      this._physics.SetHard(ent.Owner, fixture, false, comp);
  }
}
