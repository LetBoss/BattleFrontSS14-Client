using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Silicons.Laws.Ui;

public sealed class SiliconLawBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SiliconLawMenu? _menu;

	private EntityUid _owner;

	private List<SiliconLaw>? _laws;

	public SiliconLawBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		_owner = owner;
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<SiliconLawMenu>((BoundUserInterface)(object)this);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is SiliconLawBuiState siliconLawBuiState))
		{
			return;
		}
		if (_laws != null && _laws.Count == siliconLawBuiState.Laws.Count)
		{
			bool flag = true;
			foreach (SiliconLaw law in siliconLawBuiState.Laws)
			{
				if (!_laws.Contains(law))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return;
			}
		}
		_laws = siliconLawBuiState.Laws.ToList();
		_menu?.Update(_owner, siliconLawBuiState);
	}
}
