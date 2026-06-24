using System;
using Content.Shared._RMC14.Radio;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Radio;

public sealed class RMCRadioFilterBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	[ViewVariables]
	private RMCRadioFilterWindow? _window;

	public RMCRadioFilterBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCRadioFilterWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		RMCRadioFilterComponent rMCRadioFilterComponent = default(RMCRadioFilterComponent);
		EncryptionKeyHolderComponent encryptionKeyHolderComponent = default(EncryptionKeyHolderComponent);
		if (_window == null || !base.EntMan.TryGetComponent<RMCRadioFilterComponent>(((BoundUserInterface)this).Owner, ref rMCRadioFilterComponent) || !base.EntMan.TryGetComponent<EncryptionKeyHolderComponent>(((BoundUserInterface)this).Owner, ref encryptionKeyHolderComponent))
		{
			return;
		}
		RadioChannelPrototype radioChannelPrototype = default(RadioChannelPrototype);
		foreach (string channel in encryptionKeyHolderComponent.Channels)
		{
			if (_prototype.TryIndex<RadioChannelPrototype>(channel, ref radioChannelPrototype))
			{
				CheckBox val = new CheckBox
				{
					Text = Loc.GetString(LocId.op_Implicit(radioChannelPrototype.Name)),
					Pressed = !rMCRadioFilterComponent.DisabledChannels.Contains(ProtoId<RadioChannelPrototype>.op_Implicit(channel))
				};
				((BaseButton)val).OnToggled += delegate(ButtonToggledEventArgs args)
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCRadioFilterBuiMsg(ProtoId<RadioChannelPrototype>.op_Implicit(channel), args.Pressed));
				};
				((Control)_window.CheckboxContainer).AddChild((Control)(object)val);
			}
		}
	}
}
