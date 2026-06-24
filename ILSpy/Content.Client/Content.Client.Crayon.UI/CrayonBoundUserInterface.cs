using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Crayon;
using Content.Shared.Decals;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Crayon.UI;

public sealed class CrayonBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _protoManager;

	[ViewVariables]
	private CrayonWindow? _menu;

	public CrayonBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<CrayonWindow>((BoundUserInterface)(object)this);
		_menu.OnColorSelected += SelectColor;
		_menu.OnSelected += Select;
		PopulateCrayons();
	}

	private void PopulateCrayons()
	{
		IEnumerable<DecalPrototype> source = from x in _protoManager.EnumeratePrototypes<DecalPrototype>()
			where x.Tags.Contains("crayon")
			select x;
		_menu?.Populate(source.ToList());
	}

	public override void OnProtoReload(PrototypesReloadedEventArgs args)
	{
		((BoundUserInterface)this).OnProtoReload(args);
		if (args.WasModified<DecalPrototype>())
		{
			PopulateCrayons();
		}
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		((BoundUserInterface)this).ReceiveMessage(message);
		if (_menu != null && message is CrayonUsedMessage crayonUsedMessage)
		{
			_menu.AdvanceState(crayonUsedMessage.DrawnDecal);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		_menu?.UpdateState((CrayonBoundUserInterfaceState)(object)state);
	}

	public void Select(string state)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CrayonSelectMessage(state));
	}

	public void SelectColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CrayonColorMessage(color));
	}
}
