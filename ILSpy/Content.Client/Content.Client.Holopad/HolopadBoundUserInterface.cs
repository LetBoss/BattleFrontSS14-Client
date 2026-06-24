using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Holopad;
using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.ViewVariables;

namespace Content.Client.Holopad;

public sealed class HolopadBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private ISharedPlayerManager _playerManager;

	[ViewVariables]
	private HolopadWindow? _window;

	public HolopadBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<HolopadBoundUserInterface>(this);
	}

	protected override void Open()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<HolopadWindow>((BoundUserInterface)(object)this);
		_window.Title = Loc.GetString("holopad-window-title", new(string, object)[1] { ("title", base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName) });
		if (!(base.UiKey is HolopadUiKey))
		{
			((BoundUserInterface)this).Close();
			return;
		}
		HolopadUiKey holopadUiKey = (HolopadUiKey)(object)base.UiKey;
		if (holopadUiKey == HolopadUiKey.InteractionWindow && base.EntMan.HasComponent<StationAiHeldComponent>(_playerManager.LocalEntity))
		{
			holopadUiKey = HolopadUiKey.InteractionWindowForAi;
		}
		_window.SetState(((BoundUserInterface)this).Owner, holopadUiKey);
		_window.UpdateState(new Dictionary<NetEntity, string>());
		_window.SendHolopadStartNewCallMessageAction += SendHolopadStartNewCallMessage;
		_window.SendHolopadAnswerCallMessageAction += SendHolopadAnswerCallMessage;
		_window.SendHolopadEndCallMessageAction += SendHolopadEndCallMessage;
		_window.SendHolopadStartBroadcastMessageAction += SendHolopadStartBroadcastMessage;
		_window.SendHolopadActivateProjectorMessageAction += SendHolopadActivateProjectorMessage;
		_window.SendHolopadRequestStationAiMessageAction += SendHolopadRequestStationAiMessage;
		if (holopadUiKey == HolopadUiKey.AiRequestWindow)
		{
			((BaseWindow)_window).OpenCenteredAt(new Vector2(1f, 1f));
		}
		else
		{
			((BaseWindow)_window).OpenCenteredAt(new Vector2(0.3333f, 0.5f));
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		HolopadBoundInterfaceState holopadBoundInterfaceState = (HolopadBoundInterfaceState)(object)state;
		TransformComponent val = default(TransformComponent);
		base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val);
		_window?.UpdateState(holopadBoundInterfaceState.Holopads);
	}

	public void SendHolopadStartNewCallMessage(NetEntity receiver)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadStartNewCallMessage(receiver));
	}

	public void SendHolopadAnswerCallMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadAnswerCallMessage());
	}

	public void SendHolopadEndCallMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadEndCallMessage());
	}

	public void SendHolopadStartBroadcastMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadStartBroadcastMessage());
	}

	public void SendHolopadActivateProjectorMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadActivateProjectorMessage());
	}

	public void SendHolopadRequestStationAiMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolopadStationAiRequestMessage());
	}
}
