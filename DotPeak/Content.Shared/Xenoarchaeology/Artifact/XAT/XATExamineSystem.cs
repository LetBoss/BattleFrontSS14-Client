// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATExamineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATExamineSystem : BaseXATSystem<XATExamineComponent>
{
  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<ExaminedEvent>(new BaseXATSystem<XATExamineComponent>.XATEventHandler<ExaminedEvent>(this.OnExamine));
  }

  private void OnExamine(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATExamineComponent, XenoArtifactNodeComponent> node,
    ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || this.HasComp<GhostComponent>(args.Examiner))
      return;
    this.Trigger(artifact, node);
    args.PushMarkup(this.Loc.GetString("artifact-examine-trigger-desc"));
  }
}
