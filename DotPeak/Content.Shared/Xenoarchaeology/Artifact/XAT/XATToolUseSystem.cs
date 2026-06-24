// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATToolUseSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATToolUseSystem : BaseXATSystem<XATToolUseComponent>
{
  [Dependency]
  private SharedToolSystem _tool;

  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<InteractUsingEvent>(new BaseXATSystem<XATToolUseComponent>.XATEventHandler<InteractUsingEvent>(this.OnInteractUsing));
    this.XATSubscribeDirectEvent<XATToolUseDoAfterEvent>(new BaseXATSystem<XATToolUseComponent>.XATEventHandler<XATToolUseDoAfterEvent>(this.OnToolUseComplete));
  }

  private void OnToolUseComplete(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATToolUseComponent, XenoArtifactNodeComponent> node,
    ref XATToolUseDoAfterEvent args)
  {
    if (args.Cancelled || this.GetEntity(args.Node) != node.Owner)
      return;
    this.Trigger(artifact, node);
    args.Handled = true;
  }

  private void OnInteractUsing(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATToolUseComponent, XenoArtifactNodeComponent> node,
    ref InteractUsingEvent args)
  {
    ToolComponent comp;
    if (!this.TryComp<ToolComponent>(args.Used, out comp))
      return;
    XATToolUseComponent comp1 = node.Comp1;
    args.Handled = this._tool.UseTool(args.Used, args.User, new EntityUid?((EntityUid) artifact), comp1.Delay, (string) comp1.RequiredTool, (DoAfterEvent) new XATToolUseDoAfterEvent(this.GetNetEntity((EntityUid) node)), comp1.Fuel, comp);
  }
}
