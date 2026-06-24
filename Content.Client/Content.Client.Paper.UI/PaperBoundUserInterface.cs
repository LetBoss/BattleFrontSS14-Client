using System;
using Content.Shared.Paper;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client.Paper.UI;

public sealed class PaperBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private PaperWindow? _window;

	public PaperBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<PaperWindow>((BoundUserInterface)(object)this);
		_window.OnSaved += InputOnTextEntered;
		_window.OnSignatureRequested += OnSignatureRequested;
		PaperComponent paperComponent = default(PaperComponent);
		if (base.EntMan.TryGetComponent<PaperComponent>(((BoundUserInterface)this).Owner, ref paperComponent))
		{
			_window.MaxInputLength = paperComponent.ContentSize;
		}
		PaperVisualsComponent visuals = default(PaperVisualsComponent);
		if (base.EntMan.TryGetComponent<PaperVisualsComponent>(((BoundUserInterface)this).Owner, ref visuals))
		{
			_window.InitVisuals(((BoundUserInterface)this).Owner, visuals);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		_window?.Populate((PaperComponent.PaperBoundUserInterfaceState)(object)state);
	}

	private void InputOnTextEntered(string text)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PaperComponent.PaperInputTextMessage(text));
		if (_window != null)
		{
			_window.Input.TextRope = (Node)(object)Leaf.Empty;
			_window.Input.CursorPosition = new CursorPos(0, (LineBreakBias)0);
		}
	}

	private void OnSignatureRequested(int signatureIndex)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PaperComponent.PaperSignatureRequestMessage(signatureIndex));
	}
}
