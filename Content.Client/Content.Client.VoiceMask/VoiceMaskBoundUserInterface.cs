using System;
using Content.Shared.VoiceMask;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.VoiceMask;

public sealed class VoiceMaskBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _protomanager;

	[ViewVariables]
	private VoiceMaskNameChangeWindow? _window;

	public VoiceMaskBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<VoiceMaskNameChangeWindow>((BoundUserInterface)(object)this);
		_window.ReloadVerbs(_protomanager);
		_window.AddVerbs();
		VoiceMaskNameChangeWindow? window = _window;
		window.OnNameChange = (Action<string>)Delegate.Combine(window.OnNameChange, new Action<string>(OnNameSelected));
		VoiceMaskNameChangeWindow? window2 = _window;
		window2.OnVerbChange = (Action<string>)Delegate.Combine(window2.OnVerbChange, (Action<string>)delegate(string? verb)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VoiceMaskChangeVerbMessage(verb));
		});
	}

	private void OnNameSelected(string name)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VoiceMaskChangeNameMessage(name));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is VoiceMaskBuiState voiceMaskBuiState && _window != null)
		{
			_window.UpdateState(voiceMaskBuiState.Name, voiceMaskBuiState.Verb);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		VoiceMaskNameChangeWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}
}
