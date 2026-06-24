using System;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Xenoarchaeology.Ui;

public sealed class NodeScannerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private NodeScannerDisplay? _scannerDisplay;

	public NodeScannerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_scannerDisplay = BoundUserInterfaceExt.CreateWindow<NodeScannerDisplay>((BoundUserInterface)(object)this);
		_scannerDisplay.SetOwner(((BoundUserInterface)this).Owner);
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			NodeScannerDisplay? scannerDisplay = _scannerDisplay;
			if (scannerDisplay != null)
			{
				((Control)scannerDisplay).Orphan();
			}
		}
	}
}
