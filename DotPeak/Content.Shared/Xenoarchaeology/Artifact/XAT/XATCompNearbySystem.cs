// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATCompNearbySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATCompNearbySystem : BaseQueryUpdateXATSystem<XATCompNearbyComponent>
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<Entity<IComponent>> _entities = new HashSet<Entity<IComponent>>();

  protected override void UpdateXAT(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATCompNearbyComponent, XenoArtifactNodeComponent> node,
    float frameTime)
  {
    XATCompNearbyComponent comp1 = node.Comp1;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) artifact);
    ComponentRegistration registration = this.EntityManager.ComponentFactory.GetRegistration(comp1.RequireComponentWithName);
    this._entities.Clear();
    this._entityLookup.GetEntitiesInRange(registration.Type, mapCoordinates, comp1.Radius, this._entities);
    if (this._entities.Count < comp1.Count)
      return;
    this.Trigger(artifact, node);
  }
}
