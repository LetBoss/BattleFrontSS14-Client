using System;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.UI;

public sealed class TransferAmountBoundUserInterface : BoundUserInterface
{
	private IEntityManager _entManager;

	private EntityUid _owner;

	[ViewVariables]
	private TransferAmountWindow? _window;

	public TransferAmountBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		_owner = owner;
		_entManager = IoCManager.Resolve<IEntityManager>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<TransferAmountWindow>((BoundUserInterface)(object)this);
		SolutionTransferComponent solutionTransferComponent = default(SolutionTransferComponent);
		if (_entManager.TryGetComponent<SolutionTransferComponent>(_owner, ref solutionTransferComponent))
		{
			_window.SetBounds(solutionTransferComponent.MinimumTransferAmount.Int(), solutionTransferComponent.MaximumTransferAmount.Int());
		}
		((BaseButton)_window.ApplyButton).OnPressed += delegate
		{
			if (int.TryParse(_window.AmountLineEdit.Text, out var result))
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new TransferAmountSetValueMessage(FixedPoint2.New(result)));
				((BaseWindow)_window).Close();
			}
		};
	}
}
