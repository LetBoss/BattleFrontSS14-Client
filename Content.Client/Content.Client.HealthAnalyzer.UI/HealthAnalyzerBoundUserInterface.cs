using System;
using Content.Shared.MedicalScanner;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.HealthAnalyzer.UI;

public sealed class HealthAnalyzerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private HealthAnalyzerWindow? _window;

	public HealthAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<HealthAnalyzerWindow>((BoundUserInterface)(object)this);
		_window.Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (_window != null && message is HealthAnalyzerScannedUserMessage msg)
		{
			_window.Populate(msg);
		}
	}
}
