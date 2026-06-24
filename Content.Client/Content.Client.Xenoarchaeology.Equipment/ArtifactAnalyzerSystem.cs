using System;
using Content.Client.Xenoarchaeology.Ui;
using Content.Shared.Xenoarchaeology.Equipment;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Xenoarchaeology.Equipment;

public sealed class ArtifactAnalyzerSystem : SharedArtifactAnalyzerSystem
{
	[Dependency]
	private UserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AnalysisConsoleComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<AnalysisConsoleComponent, AfterAutoHandleStateEvent>)OnAnalysisConsoleAfterAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactAnalyzerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ArtifactAnalyzerComponent, AfterAutoHandleStateEvent>)OnAnalyzerAfterAutoHandleState, (Type[])null, (Type[])null);
	}

	private void OnAnalysisConsoleAfterAutoHandleState(Entity<AnalysisConsoleComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateBuiIfCanGetAnalysisConsoleUi(ent);
	}

	private void OnAnalyzerAfterAutoHandleState(Entity<ArtifactAnalyzerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetAnalysisConsole(ent, out Entity<AnalysisConsoleComponent>? analysisConsole))
		{
			UpdateBuiIfCanGetAnalysisConsoleUi(analysisConsole.Value);
		}
	}

	private void UpdateBuiIfCanGetAnalysisConsoleUi(Entity<AnalysisConsoleComponent> analysisConsole)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		AnalysisConsoleBoundUserInterface analysisConsoleBoundUserInterface = default(AnalysisConsoleBoundUserInterface);
		if (((SharedUserInterfaceSystem)_ui).TryGetOpenUi<AnalysisConsoleBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(analysisConsole.Owner), (Enum)ArtifactAnalyzerUiKey.Key, ref analysisConsoleBoundUserInterface))
		{
			analysisConsoleBoundUserInterface.Update(analysisConsole);
		}
	}
}
