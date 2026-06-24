using System;
using Content.Shared.Implants;
using Content.Shared.Timing;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Implants.UI;

public sealed class ChameleonControllerBoundUserInterface : BoundUserInterface
{
	private readonly UseDelaySystem _delay;

	[ViewVariables]
	private ChameleonControllerMenu? _menu;

	public ChameleonControllerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_delay = base.EntMan.System<UseDelaySystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ChameleonControllerMenu>((BoundUserInterface)(object)this);
		_menu.OnJobSelected += OnJobSelected;
	}

	private void OnJobSelected(ProtoId<ChameleonOutfitPrototype> outfit)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent item = default(UseDelayComponent);
		if (base.EntMan.TryGetComponent<UseDelayComponent>(((BoundUserInterface)this).Owner, ref item) && _delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)), checkDelayed: true))
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChameleonControllerSelectedOutfitMessage(outfit));
			if (_delay.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)), out UseDelayInfo info) && _menu != null)
			{
				_menu._lockedUntil = DateTime.Now.Add(info.Length);
				_menu.UpdateGrid(disabled: true);
			}
		}
	}
}
