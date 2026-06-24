using System;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Radio.Ui;

public sealed class IntercomBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private IntercomMenu? _menu;

	public IntercomBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<IntercomMenu>((BoundUserInterface)(object)this);
		IntercomComponent item = default(IntercomComponent);
		if (base.EntMan.TryGetComponent<IntercomComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			_menu.Update(Entity<IntercomComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
		_menu.OnMicPressed += delegate(bool enabled)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ToggleIntercomMicMessage(enabled));
		};
		_menu.OnSpeakerPressed += delegate(bool enabled)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ToggleIntercomSpeakerMessage(enabled));
		};
		_menu.OnChannelSelected += delegate(string channel)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SelectIntercomChannelMessage(channel));
		};
	}

	public void Update(Entity<IntercomComponent> ent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_menu?.Update(ent);
	}
}
