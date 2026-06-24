// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.SharedArtifactAnalyzerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.Placeable;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment;

public abstract class SharedArtifactAnalyzerSystem : EntitySystem
{
  [Dependency]
  private SharedPowerReceiverSystem _powerReceiver;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ArtifactAnalyzerComponent, ItemPlacedEvent>(new EntityEventRefHandler<ArtifactAnalyzerComponent, ItemPlacedEvent>(this.OnItemPlaced));
    this.SubscribeLocalEvent<ArtifactAnalyzerComponent, ItemRemovedEvent>(new EntityEventRefHandler<ArtifactAnalyzerComponent, ItemRemovedEvent>(this.OnItemRemoved));
    this.SubscribeLocalEvent<ArtifactAnalyzerComponent, MapInitEvent>(new EntityEventRefHandler<ArtifactAnalyzerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<AnalysisConsoleComponent, NewLinkEvent>(new EntityEventRefHandler<AnalysisConsoleComponent, NewLinkEvent>(this.OnNewLink));
    this.SubscribeLocalEvent<AnalysisConsoleComponent, PortDisconnectedEvent>(new EntityEventRefHandler<AnalysisConsoleComponent, PortDisconnectedEvent>(this.OnPortDisconnected));
  }

  private void OnItemPlaced(Entity<ArtifactAnalyzerComponent> ent, ref ItemPlacedEvent args)
  {
    ent.Comp.CurrentArtifact = new EntityUid?(args.OtherEntity);
    this.Dirty<ArtifactAnalyzerComponent>(ent);
  }

  private void OnItemRemoved(Entity<ArtifactAnalyzerComponent> ent, ref ItemRemovedEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    EntityUid? currentArtifact = ent.Comp.CurrentArtifact;
    if ((currentArtifact.HasValue ? (otherEntity != currentArtifact.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      return;
    ent.Comp.CurrentArtifact = new EntityUid?();
    this.Dirty<ArtifactAnalyzerComponent>(ent);
  }

  private void OnMapInit(Entity<ArtifactAnalyzerComponent> ent, ref MapInitEvent args)
  {
    DeviceLinkSinkComponent comp1;
    if (!this.TryComp<DeviceLinkSinkComponent>((EntityUid) ent, out comp1))
      return;
    foreach (EntityUid linkedSource in comp1.LinkedSources)
    {
      AnalysisConsoleComponent comp2;
      if (this.TryComp<AnalysisConsoleComponent>(linkedSource, out comp2))
      {
        comp2.AnalyzerEntity = new NetEntity?(this.GetNetEntity((EntityUid) ent));
        ent.Comp.Console = new EntityUid?(linkedSource);
        this.Dirty(linkedSource, (IComponent) comp2);
        this.Dirty<ArtifactAnalyzerComponent>(ent);
        break;
      }
    }
  }

  private void OnNewLink(Entity<AnalysisConsoleComponent> ent, ref NewLinkEvent args)
  {
    ArtifactAnalyzerComponent comp;
    if (!this.TryComp<ArtifactAnalyzerComponent>(args.Sink, out comp))
      return;
    ent.Comp.AnalyzerEntity = new NetEntity?(this.GetNetEntity(args.Sink));
    comp.Console = new EntityUid?((EntityUid) ent);
    this.Dirty(args.Sink, (IComponent) comp);
    this.Dirty<AnalysisConsoleComponent>(ent);
  }

  private void OnPortDisconnected(
    Entity<AnalysisConsoleComponent> ent,
    ref PortDisconnectedEvent args)
  {
    NetEntity? analyzerEntity = ent.Comp.AnalyzerEntity;
    if ((ProtoId<SourcePortPrototype>) args.Port != ent.Comp.LinkingPort || !analyzerEntity.HasValue)
      return;
    EntityUid? entity = this.GetEntity(analyzerEntity);
    ArtifactAnalyzerComponent comp;
    if (this.TryComp<ArtifactAnalyzerComponent>(entity, out comp))
    {
      comp.Console = new EntityUid?();
      this.Dirty(entity.Value, (IComponent) comp);
    }
    ent.Comp.AnalyzerEntity = new NetEntity?();
    this.Dirty<AnalysisConsoleComponent>(ent);
  }

  public bool TryGetAnalyzer(
    Entity<AnalysisConsoleComponent> ent,
    [NotNullWhen(true)] out Entity<ArtifactAnalyzerComponent>? analyzer)
  {
    analyzer = new Entity<ArtifactAnalyzerComponent>?();
    if (!this._powerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent.Owner))
      return false;
    EntityUid? entity = this.GetEntity(ent.Comp.AnalyzerEntity);
    ArtifactAnalyzerComponent comp;
    if (!this.TryComp<ArtifactAnalyzerComponent>(entity, out comp) || !this._powerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) entity.Value))
      return false;
    analyzer = new Entity<ArtifactAnalyzerComponent>?((Entity<ArtifactAnalyzerComponent>) (entity.Value, comp));
    return true;
  }

  public bool TryGetArtifactFromConsole(
    Entity<AnalysisConsoleComponent> ent,
    [NotNullWhen(true)] out Entity<XenoArtifactComponent>? artifact)
  {
    artifact = new Entity<XenoArtifactComponent>?();
    Entity<ArtifactAnalyzerComponent>? analyzer;
    XenoArtifactComponent comp;
    if (!this.TryGetAnalyzer(ent, out analyzer) || !this.TryComp<XenoArtifactComponent>(analyzer.Value.Comp.CurrentArtifact, out comp))
      return false;
    artifact = new Entity<XenoArtifactComponent>?((Entity<XenoArtifactComponent>) (analyzer.Value.Comp.CurrentArtifact.Value, comp));
    return true;
  }

  public bool TryGetAnalysisConsole(
    Entity<ArtifactAnalyzerComponent> ent,
    [NotNullWhen(true)] out Entity<AnalysisConsoleComponent>? analysisConsole)
  {
    analysisConsole = new Entity<AnalysisConsoleComponent>?();
    AnalysisConsoleComponent comp;
    if (!this.TryComp<AnalysisConsoleComponent>(ent.Comp.Console, out comp))
      return false;
    analysisConsole = new Entity<AnalysisConsoleComponent>?((Entity<AnalysisConsoleComponent>) (ent.Comp.Console.Value, comp));
    return true;
  }
}
