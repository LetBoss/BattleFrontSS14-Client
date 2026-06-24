// Decompiled with JetBrains decompiler
// Type: Content.Client.Xenoarchaeology.Equipment.ArtifactAnalyzerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Xenoarchaeology.Ui;
using Content.Shared.Xenoarchaeology.Equipment;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Xenoarchaeology.Equipment;

public sealed class ArtifactAnalyzerSystem : SharedArtifactAnalyzerSystem
{
  [Dependency]
  private UserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnalysisConsoleComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<AnalysisConsoleComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAnalysisConsoleAfterAutoHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArtifactAnalyzerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ArtifactAnalyzerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAnalyzerAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAnalysisConsoleAfterAutoHandleState(
    Entity<AnalysisConsoleComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateBuiIfCanGetAnalysisConsoleUi(ent);
  }

  private void OnAnalyzerAfterAutoHandleState(
    Entity<ArtifactAnalyzerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    Entity<AnalysisConsoleComponent>? analysisConsole;
    if (!this.TryGetAnalysisConsole(ent, out analysisConsole))
      return;
    this.UpdateBuiIfCanGetAnalysisConsoleUi(analysisConsole.Value);
  }

  private void UpdateBuiIfCanGetAnalysisConsoleUi(Entity<AnalysisConsoleComponent> analysisConsole)
  {
    AnalysisConsoleBoundUserInterface boundUserInterface;
    if (!((SharedUserInterfaceSystem) this._ui).TryGetOpenUi<AnalysisConsoleBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(analysisConsole.Owner), (Enum) ArtifactAnalyzerUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update(analysisConsole);
  }
}
