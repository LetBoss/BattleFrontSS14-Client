using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.BarSign;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client.BarSign.Ui;

public sealed class BarSignBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private BarSignMenu? _menu;

	public BarSignBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		ProtoId<BarSignPrototype>? val = EntityManagerExt.GetComponentOrNull<BarSignComponent>(base.EntMan, ((BoundUserInterface)this).Owner)?.Current;
		object obj;
		if (val.HasValue)
		{
			ProtoId<BarSignPrototype> valueOrDefault = val.GetValueOrDefault();
			obj = _prototype.Index<BarSignPrototype>(valueOrDefault);
		}
		else
		{
			obj = null;
		}
		BarSignPrototype currentSign = (BarSignPrototype)obj;
		List<BarSignPrototype> signs = (from p in Content.Shared.BarSign.BarSignSystem.GetAllBarSigns(_prototype)
			orderby Loc.GetString(LocId.op_Implicit(p.Name))
			select p).ToList();
		_menu = new BarSignMenu(currentSign, signs);
		_menu.OnSignSelected += delegate(string id)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SetBarSignMessage(ProtoId<BarSignPrototype>.op_Implicit(id)));
		};
		((BaseWindow)_menu).OnClose += base.Close;
		((BaseWindow)_menu).OpenCentered();
	}

	public void Update(ProtoId<BarSignPrototype>? sign)
	{
		BarSignPrototype newSign = default(BarSignPrototype);
		if (_prototype.TryIndex<BarSignPrototype>(sign, ref newSign))
		{
			_menu?.UpdateState(newSign);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			BarSignMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
