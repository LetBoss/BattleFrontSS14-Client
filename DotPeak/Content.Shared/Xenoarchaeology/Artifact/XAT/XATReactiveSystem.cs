// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATReactiveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATReactiveSystem : BaseXATSystem<XATReactiveComponent>
{
  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<ReactionEntityEvent>(new BaseXATSystem<XATReactiveComponent>.XATEventHandler<ReactionEntityEvent>(this.OnReaction));
  }

  private void OnReaction(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATReactiveComponent, XenoArtifactNodeComponent> node,
    ref ReactionEntityEvent args)
  {
    XATReactiveComponent comp1 = node.Comp1;
    if (!comp1.ReactionMethods.Contains(args.Method) || args.ReagentQuantity.Quantity < comp1.MinQuantity || !comp1.Reagents.Contains((ProtoId<ReagentPrototype>) args.Reagent.ID))
      return;
    HashSet<ProtoId<ReactiveGroupPrototype>> reactiveGroups = comp1.ReactiveGroups;
    // ISSUE: explicit non-virtual call
    if ((reactiveGroups != null ? (__nonvirtual (reactiveGroups.Count) > 0 ? 1 : 0) : 0) != 0 && !XATReactiveSystem.ReagentHaveReactiveGroup(args, comp1))
      return;
    this.Trigger(artifact, node);
  }

  private static bool ReagentHaveReactiveGroup(
    ReactionEntityEvent args,
    XATReactiveComponent reactiveTriggerComponent)
  {
    Dictionary<ProtoId<ReactiveGroupPrototype>, Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry> reactiveEffects = args.Reagent.ReactiveEffects;
    if (reactiveEffects == null)
      return false;
    foreach (ProtoId<ReactiveGroupPrototype> reactiveGroup in reactiveTriggerComponent.ReactiveGroups)
    {
      Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry reagentEffectEntry;
      if (reactiveEffects.TryGetValue(reactiveGroup, out reagentEffectEntry))
      {
        HashSet<ReactionMethod> methods = reagentEffectEntry.Methods;
        // ISSUE: explicit non-virtual call
        if ((methods != null ? (__nonvirtual (methods.Contains(args.Method)) ? 1 : 0) : 0) != 0)
          return true;
      }
    }
    return false;
  }
}
