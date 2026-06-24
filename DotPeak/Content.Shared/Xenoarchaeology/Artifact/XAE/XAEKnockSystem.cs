// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAEKnockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Magic.Events;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEKnockSystem : BaseXAESystem<XAEKnockComponent>
{
  [Dependency]
  private IGameTiming _timing;

  protected override void OnActivated(
    Entity<XAEKnockComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    KnockSpellEvent message = new KnockSpellEvent();
    message.Performer = ent.Owner;
    message.Range = ent.Comp.KnockRange;
    this.RaiseLocalEvent<KnockSpellEvent>(message);
  }
}
