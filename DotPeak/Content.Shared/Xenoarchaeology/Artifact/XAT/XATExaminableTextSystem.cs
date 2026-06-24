// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATExaminableTextSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATExaminableTextSystem : BaseXATSystem<XATExaminableTextComponent>
{
  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<ExaminedEvent>(new BaseXATSystem<XATExaminableTextComponent>.XATEventHandler<ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATExaminableTextComponent, XenoArtifactNodeComponent> node,
    ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString((string) node.Comp1.ExamineText));
  }
}
