using System;
using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Silicons.StationAi;

public sealed class StationAiCustomizationBoundUserInterface : BoundUserInterface
{
	private StationAiCustomizationMenu? _menu;

	public StationAiCustomizationBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = new StationAiCustomizationMenu(((BoundUserInterface)this).Owner);
		((BaseWindow)_menu).OpenCentered();
		((BaseWindow)_menu).OnClose += base.Close;
		_menu.SendStationAiCustomizationMessageAction += SendStationAiCustomizationMessage;
	}

	public void SendStationAiCustomizationMessage(ProtoId<StationAiCustomizationGroupPrototype> groupProtoId, ProtoId<StationAiCustomizationPrototype> customizationProtoId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new StationAiCustomizationMessage(groupProtoId, customizationProtoId));
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			StationAiCustomizationMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
			_menu = null;
		}
	}
}
