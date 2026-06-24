// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATItemLandSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Throwing;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATItemLandSystem : BaseXATSystem<XATItemLandComponent>
{
  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<LandEvent>(new BaseXATSystem<XATItemLandComponent>.XATEventHandler<LandEvent>(this.OnLand));
  }

  private void OnLand(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATItemLandComponent, XenoArtifactNodeComponent> node,
    ref LandEvent args)
  {
    this.Trigger(artifact, node);
  }
}
