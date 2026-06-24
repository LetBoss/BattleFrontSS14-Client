// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Info.InfoUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.Info;
using Content.Shared.Guidebook;
using Content.Shared.Info;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Info;

public sealed class InfoUIController : UIController, IOnStateExited<GameplayState>
{
  [Dependency]
  private IClientConsoleHost _consoleHost;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private IPrototypeManager _prototype;
  public RulesPopup? RulesPopup;
  private RulesAndInfoWindow? _infoWindow;
  private static readonly ProtoId<GuideEntryPrototype> DefaultRuleset = ProtoId<GuideEntryPrototype>.op_Implicit(nameof (DefaultRuleset));
  public ProtoId<GuideEntryPrototype> RulesEntryId = InfoUIController.DefaultRuleset;

  protected virtual string SawmillName => "rules";

  public event Action? Accepted;

  public virtual void Initialize()
  {
    base.Initialize();
    this._netManager.RegisterNetMessage<RulesAcceptedMessage>((ProcessMessage<RulesAcceptedMessage>) null, (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._netManager.RegisterNetMessage<SendRulesInformationMessage>(new ProcessMessage<SendRulesInformationMessage>((object) this, __methodptr(OnRulesInformationMessage)), (NetMessageAccept) 3);
    ((IConsoleHost) this._consoleHost).RegisterCommand("fuckrules", "", "", (ConCommandCallback) ((_1, _2, _3) => this.OnAcceptPressed()), false);
  }

  private void OnRulesInformationMessage(SendRulesInformationMessage message)
  {
    this.RulesEntryId = ProtoId<GuideEntryPrototype>.op_Implicit(message.CoreRules);
    if (!message.ShouldShowRules)
      return;
    this.ShowRules(message.PopupTime);
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._infoWindow == null)
      return;
    if (!((Control) this._infoWindow).Disposed)
      ((Control) this._infoWindow).Orphan();
    this._infoWindow = (RulesAndInfoWindow) null;
  }

  private void ShowRules(float time)
  {
    if (this.RulesPopup != null)
      return;
    this.RulesPopup = new RulesPopup() { Timer = time };
    this.RulesPopup.OnQuitPressed += new Action(this.OnQuitPressed);
    this.RulesPopup.OnAcceptPressed += new Action(this.OnAcceptPressed);
    ((Control) this.UIManager.WindowRoot).AddChild((Control) this.RulesPopup);
    LayoutContainer.SetAnchorPreset((Control) this.RulesPopup, (LayoutContainer.LayoutPreset) 15, false);
  }

  private void OnQuitPressed() => ((IConsoleHost) this._consoleHost).ExecuteCommand("quit");

  private void OnAcceptPressed()
  {
    this._netManager.ClientSendMessage((NetMessage) new RulesAcceptedMessage());
    RulesPopup rulesPopup = this.RulesPopup;
    if (rulesPopup != null && !rulesPopup.Disposed)
      rulesPopup.Orphan();
    this.RulesPopup = (RulesPopup) null;
    Action accepted = this.Accepted;
    if (accepted == null)
      return;
    accepted();
  }

  public GuideEntryPrototype GetCoreRuleEntry()
  {
    GuideEntryPrototype coreRuleEntry1;
    if (this._prototype.TryIndex<GuideEntryPrototype>(this.RulesEntryId, ref coreRuleEntry1))
      return coreRuleEntry1;
    GuideEntryPrototype coreRuleEntry2 = this._prototype.Index<GuideEntryPrototype>(InfoUIController.DefaultRuleset);
    this.Log.Error($"Couldn't find the following prototype: {this.RulesEntryId}. Falling back to {InfoUIController.DefaultRuleset}, please check that the server has the rules set up correctly");
    return coreRuleEntry2;
  }

  public void OpenWindow()
  {
    if (this._infoWindow == null || ((Control) this._infoWindow).Disposed)
      this._infoWindow = this.UIManager.CreateWindow<RulesAndInfoWindow>();
    ((BaseWindow) this._infoWindow)?.OpenCentered();
  }
}
