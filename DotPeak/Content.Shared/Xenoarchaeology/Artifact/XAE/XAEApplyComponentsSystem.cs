// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAEApplyComponentsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEApplyComponentsSystem : BaseXAESystem<XAEApplyComponentsComponent>
{
  [Dependency]
  private IGameTiming _timing;

  protected override void OnActivated(
    Entity<XAEApplyComponentsComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    Entity<XenoArtifactComponent> artifact = args.Artifact;
    foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> component1 in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) ent.Comp.Components)
    {
      Type type = component1.Value.Component.GetType();
      if (ent.Comp.ApplyIfAlreadyHave || !this.HasComp((EntityUid) artifact, type))
      {
        if (ent.Comp.RefreshOnReactivate)
          this.RemComp((EntityUid) artifact, type);
        IComponent component2 = this.EntityManager.ComponentFactory.GetComponent(component1.Value);
        this.AddComp<IComponent>((EntityUid) artifact, component2);
      }
    }
  }
}
