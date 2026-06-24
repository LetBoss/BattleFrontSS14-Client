using System;
using System.Diagnostics.CodeAnalysis;
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

namespace Content.Shared.Xenoarchaeology.Equipment;

public abstract class SharedArtifactAnalyzerSystem : EntitySystem
{
	[Dependency]
	private SharedPowerReceiverSystem _powerReceiver;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ArtifactAnalyzerComponent, ItemPlacedEvent>((EntityEventRefHandler<ArtifactAnalyzerComponent, ItemPlacedEvent>)OnItemPlaced, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactAnalyzerComponent, ItemRemovedEvent>((EntityEventRefHandler<ArtifactAnalyzerComponent, ItemRemovedEvent>)OnItemRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactAnalyzerComponent, MapInitEvent>((EntityEventRefHandler<ArtifactAnalyzerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnalysisConsoleComponent, NewLinkEvent>((EntityEventRefHandler<AnalysisConsoleComponent, NewLinkEvent>)OnNewLink, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnalysisConsoleComponent, PortDisconnectedEvent>((EntityEventRefHandler<AnalysisConsoleComponent, PortDisconnectedEvent>)OnPortDisconnected, (Type[])null, (Type[])null);
	}

	private void OnItemPlaced(Entity<ArtifactAnalyzerComponent> ent, ref ItemPlacedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.CurrentArtifact = args.OtherEntity;
		((EntitySystem)this).Dirty<ArtifactAnalyzerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnItemRemoved(Entity<ArtifactAnalyzerComponent> ent, ref ItemRemovedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherEntity = args.OtherEntity;
		EntityUid? currentArtifact = ent.Comp.CurrentArtifact;
		if (currentArtifact.HasValue && !(otherEntity != currentArtifact.GetValueOrDefault()))
		{
			ent.Comp.CurrentArtifact = null;
			((EntitySystem)this).Dirty<ArtifactAnalyzerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnMapInit(Entity<ArtifactAnalyzerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		DeviceLinkSinkComponent sink = default(DeviceLinkSinkComponent);
		if (!((EntitySystem)this).TryComp<DeviceLinkSinkComponent>(Entity<ArtifactAnalyzerComponent>.op_Implicit(ent), ref sink))
		{
			return;
		}
		AnalysisConsoleComponent analysis = default(AnalysisConsoleComponent);
		foreach (EntityUid source in sink.LinkedSources)
		{
			if (((EntitySystem)this).TryComp<AnalysisConsoleComponent>(source, ref analysis))
			{
				analysis.AnalyzerEntity = ((EntitySystem)this).GetNetEntity(Entity<ArtifactAnalyzerComponent>.op_Implicit(ent), (MetaDataComponent)null);
				ent.Comp.Console = source;
				((EntitySystem)this).Dirty(source, (IComponent)(object)analysis, (MetaDataComponent)null);
				((EntitySystem)this).Dirty<ArtifactAnalyzerComponent>(ent, (MetaDataComponent)null);
				break;
			}
		}
	}

	private void OnNewLink(Entity<AnalysisConsoleComponent> ent, ref NewLinkEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ArtifactAnalyzerComponent analyzer = default(ArtifactAnalyzerComponent);
		if (((EntitySystem)this).TryComp<ArtifactAnalyzerComponent>(args.Sink, ref analyzer))
		{
			ent.Comp.AnalyzerEntity = ((EntitySystem)this).GetNetEntity(args.Sink, (MetaDataComponent)null);
			analyzer.Console = Entity<AnalysisConsoleComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(args.Sink, (IComponent)(object)analyzer, (MetaDataComponent)null);
			((EntitySystem)this).Dirty<AnalysisConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPortDisconnected(Entity<AnalysisConsoleComponent> ent, ref PortDisconnectedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		NetEntity? analyzerNetEntity = ent.Comp.AnalyzerEntity;
		if (!(ProtoId<SourcePortPrototype>.op_Implicit(args.Port) != ent.Comp.LinkingPort) && analyzerNetEntity.HasValue)
		{
			EntityUid? analyzerEntityUid = ((EntitySystem)this).GetEntity(analyzerNetEntity);
			ArtifactAnalyzerComponent analyzer = default(ArtifactAnalyzerComponent);
			if (((EntitySystem)this).TryComp<ArtifactAnalyzerComponent>(analyzerEntityUid, ref analyzer))
			{
				analyzer.Console = null;
				((EntitySystem)this).Dirty(analyzerEntityUid.Value, (IComponent)(object)analyzer, (MetaDataComponent)null);
			}
			ent.Comp.AnalyzerEntity = null;
			((EntitySystem)this).Dirty<AnalysisConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	public bool TryGetAnalyzer(Entity<AnalysisConsoleComponent> ent, [NotNullWhen(true)] out Entity<ArtifactAnalyzerComponent>? analyzer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		analyzer = null;
		EntityUid consoleEnt = ent.Owner;
		if (!_powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(consoleEnt)))
		{
			return false;
		}
		EntityUid? analyzerUid = ((EntitySystem)this).GetEntity(ent.Comp.AnalyzerEntity);
		ArtifactAnalyzerComponent analyzerComp = default(ArtifactAnalyzerComponent);
		if (!((EntitySystem)this).TryComp<ArtifactAnalyzerComponent>(analyzerUid, ref analyzerComp))
		{
			return false;
		}
		if (!_powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(analyzerUid.Value)))
		{
			return false;
		}
		analyzer = Entity<ArtifactAnalyzerComponent>.op_Implicit((analyzerUid.Value, analyzerComp));
		return true;
	}

	public bool TryGetArtifactFromConsole(Entity<AnalysisConsoleComponent> ent, [NotNullWhen(true)] out Entity<XenoArtifactComponent>? artifact)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		artifact = null;
		if (!TryGetAnalyzer(ent, out Entity<ArtifactAnalyzerComponent>? analyzer))
		{
			return false;
		}
		XenoArtifactComponent comp = default(XenoArtifactComponent);
		if (!((EntitySystem)this).TryComp<XenoArtifactComponent>(analyzer.Value.Comp.CurrentArtifact, ref comp))
		{
			return false;
		}
		artifact = Entity<XenoArtifactComponent>.op_Implicit((analyzer.Value.Comp.CurrentArtifact.Value, comp));
		return true;
	}

	public bool TryGetAnalysisConsole(Entity<ArtifactAnalyzerComponent> ent, [NotNullWhen(true)] out Entity<AnalysisConsoleComponent>? analysisConsole)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		analysisConsole = null;
		AnalysisConsoleComponent consoleComp = default(AnalysisConsoleComponent);
		if (!((EntitySystem)this).TryComp<AnalysisConsoleComponent>(ent.Comp.Console, ref consoleComp))
		{
			return false;
		}
		analysisConsole = Entity<AnalysisConsoleComponent>.op_Implicit((ent.Comp.Console.Value, consoleComp));
		return true;
	}
}
