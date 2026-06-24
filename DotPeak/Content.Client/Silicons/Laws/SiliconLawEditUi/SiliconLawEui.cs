// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Laws.SiliconLawEditUi.SiliconLawEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Silicons.Laws;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Silicons.Laws.SiliconLawEditUi;

public sealed class SiliconLawEui : BaseEui
{
  private readonly EntityManager _entityManager;
  private SiliconLawUi _siliconLawUi;
  private EntityUid _target;

  public SiliconLawEui()
  {
    this._entityManager = IoCManager.Resolve<EntityManager>();
    this._siliconLawUi = new SiliconLawUi();
    this._siliconLawUi.OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
    ((BaseButton) this._siliconLawUi.Save).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new SiliconLawsSaveMessage(this._siliconLawUi.GetLaws(), this._entityManager.GetNetEntity(this._target, (MetaDataComponent) null))));
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is SiliconLawsEuiState siliconLawsEuiState))
      return;
    this._target = this._entityManager.GetEntity(siliconLawsEuiState.Target);
    this._siliconLawUi.SetLaws(siliconLawsEuiState.Laws);
  }

  public override void Opened() => this._siliconLawUi.OpenCentered();
}
