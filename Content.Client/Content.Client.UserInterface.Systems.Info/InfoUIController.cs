using System;
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

	private static readonly ProtoId<GuideEntryPrototype> DefaultRuleset = ProtoId<GuideEntryPrototype>.op_Implicit("DefaultRuleset");

	public ProtoId<GuideEntryPrototype> RulesEntryId = DefaultRuleset;

	protected override string SawmillName => "rules";

	public event Action? Accepted;

	public override void Initialize()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		((UIController)this).Initialize();
		_netManager.RegisterNetMessage<RulesAcceptedMessage>((ProcessMessage<RulesAcceptedMessage>)null, (NetMessageAccept)3);
		_netManager.RegisterNetMessage<SendRulesInformationMessage>((ProcessMessage<SendRulesInformationMessage>)OnRulesInformationMessage, (NetMessageAccept)3);
		((IConsoleHost)_consoleHost).RegisterCommand("fuckrules", "", "", (ConCommandCallback)delegate
		{
			OnAcceptPressed();
		}, false);
	}

	private void OnRulesInformationMessage(SendRulesInformationMessage message)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RulesEntryId = ProtoId<GuideEntryPrototype>.op_Implicit(message.CoreRules);
		if (message.ShouldShowRules)
		{
			ShowRules(message.PopupTime);
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (_infoWindow != null)
		{
			if (!((Control)_infoWindow).Disposed)
			{
				((Control)_infoWindow).Orphan();
			}
			_infoWindow = null;
		}
	}

	private void ShowRules(float time)
	{
		if (RulesPopup == null)
		{
			RulesPopup = new RulesPopup
			{
				Timer = time
			};
			RulesPopup.OnQuitPressed += OnQuitPressed;
			RulesPopup.OnAcceptPressed += OnAcceptPressed;
			((Control)base.UIManager.WindowRoot).AddChild((Control)(object)RulesPopup);
			LayoutContainer.SetAnchorPreset((Control)(object)RulesPopup, (LayoutPreset)15, false);
		}
	}

	private void OnQuitPressed()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("quit");
	}

	private void OnAcceptPressed()
	{
		_netManager.ClientSendMessage((NetMessage)(object)new RulesAcceptedMessage());
		RulesPopup rulesPopup = RulesPopup;
		if (rulesPopup != null && !((Control)rulesPopup).Disposed)
		{
			((Control)rulesPopup).Orphan();
		}
		RulesPopup = null;
		this.Accepted?.Invoke();
	}

	public GuideEntryPrototype GetCoreRuleEntry()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		GuideEntryPrototype result = default(GuideEntryPrototype);
		if (!_prototype.TryIndex<GuideEntryPrototype>(RulesEntryId, ref result))
		{
			result = _prototype.Index<GuideEntryPrototype>(DefaultRuleset);
			((UIController)this).Log.Error($"Couldn't find the following prototype: {RulesEntryId}. Falling back to {DefaultRuleset}, please check that the server has the rules set up correctly");
			return result;
		}
		return result;
	}

	public void OpenWindow()
	{
		if (_infoWindow == null || ((Control)_infoWindow).Disposed)
		{
			_infoWindow = base.UIManager.CreateWindow<RulesAndInfoWindow>();
		}
		RulesAndInfoWindow? infoWindow = _infoWindow;
		if (infoWindow != null)
		{
			((BaseWindow)infoWindow).OpenCentered();
		}
	}
}
