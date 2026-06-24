using System;
using Content.Shared.Labels;
using Content.Shared.Labels.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Labels.UI;

public sealed class HandLabelerBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entManager;

	[ViewVariables]
	private HandLabelerWindow? _window;

	public HandLabelerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<HandLabelerBoundUserInterface>(this);
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<HandLabelerWindow>((BoundUserInterface)(object)this);
		HandLabelerComponent handLabelerComponent = default(HandLabelerComponent);
		if (_entManager.TryGetComponent<HandLabelerComponent>(((BoundUserInterface)this).Owner, ref handLabelerComponent))
		{
			_window.SetMaxLabelLength(handLabelerComponent.MaxLabelChars);
		}
		_window.OnLabelChanged += OnLabelChanged;
		Reload();
	}

	private void OnLabelChanged(string newLabel)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		HandLabelerComponent handLabelerComponent = default(HandLabelerComponent);
		if (!_entManager.TryGetComponent<HandLabelerComponent>(((BoundUserInterface)this).Owner, ref handLabelerComponent) || !handLabelerComponent.AssignedLabel.Equals(newLabel))
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new HandLabelerLabelChangedMessage(newLabel));
		}
	}

	public void Reload()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		HandLabelerComponent handLabelerComponent = default(HandLabelerComponent);
		if (_window != null && _entManager.TryGetComponent<HandLabelerComponent>(((BoundUserInterface)this).Owner, ref handLabelerComponent))
		{
			_window.SetCurrentLabel(handLabelerComponent.AssignedLabel);
		}
	}
}
