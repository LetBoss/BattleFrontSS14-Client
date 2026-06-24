using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Instruments.UI;
using Content.Shared.Interaction;
using Robust.Client.Audio.Midi;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Instruments.UI;

public sealed class InstrumentBoundUserInterface : BoundUserInterface
{
	[Dependency]
	public IMidiManager MidiManager;

	[Dependency]
	public IFileDialogManager FileDialogManager;

	[Dependency]
	public ILocalizationManager Loc;

	public readonly InstrumentSystem Instruments;

	public readonly ActionBlockerSystem ActionBlocker;

	public readonly SharedInteractionSystem Interactions;

	[ViewVariables]
	private InstrumentMenu? _instrumentMenu;

	[ViewVariables]
	private BandMenu? _bandMenu;

	[ViewVariables]
	private ChannelsMenu? _channelsMenu;

	public IEntityManager Entities => base.EntMan;

	public InstrumentBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<InstrumentBoundUserInterface>(this);
		Instruments = Entities.System<InstrumentSystem>();
		ActionBlocker = Entities.System<ActionBlockerSystem>();
		Interactions = Entities.System<SharedInteractionSystem>();
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (message is InstrumentBandResponseBuiMessage instrumentBandResponseBuiMessage)
		{
			_bandMenu?.Populate(instrumentBandResponseBuiMessage.Nearby, base.EntMan);
		}
	}

	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_instrumentMenu = BoundUserInterfaceExt.CreateWindow<InstrumentMenu>((BoundUserInterface)(object)this);
		((DefaultWindow)_instrumentMenu).Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		_instrumentMenu.OnOpenBand += OpenBandMenu;
		_instrumentMenu.OnOpenChannels += OpenChannelsMenu;
		_instrumentMenu.OnCloseChannels += CloseChannelsMenu;
		_instrumentMenu.OnCloseBands += CloseBandMenu;
		_instrumentMenu.SetMIDI(MidiManager.IsAvailable);
		InstrumentComponent item = default(InstrumentComponent);
		if (base.EntMan.TryGetComponent<InstrumentComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			_instrumentMenu.SetInstrument(Entity<InstrumentComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
	}

	protected override void Dispose(bool disposing)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			InstrumentComponent component = default(InstrumentComponent);
			if (base.EntMan.TryGetComponent<InstrumentComponent>(((BoundUserInterface)this).Owner, ref component))
			{
				_instrumentMenu?.RemoveInstrument(component);
			}
			BandMenu? bandMenu = _bandMenu;
			if (bandMenu != null)
			{
				((Control)bandMenu).Orphan();
			}
			ChannelsMenu? channelsMenu = _channelsMenu;
			if (channelsMenu != null)
			{
				((Control)channelsMenu).Orphan();
			}
		}
	}

	public void RefreshBands()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new InstrumentBandRequestBuiMessage());
	}

	public void OpenBandMenu()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_bandMenu == null)
		{
			_bandMenu = new BandMenu(this);
		}
		InstrumentComponent instrumentComponent = default(InstrumentComponent);
		if (base.EntMan.TryGetComponent<InstrumentComponent>(((BoundUserInterface)this).Owner, ref instrumentComponent))
		{
			_bandMenu.Master = instrumentComponent.Master;
		}
		RefreshBands();
		((BaseWindow)_bandMenu).OpenCenteredLeft();
	}

	public void CloseBandMenu()
	{
		BandMenu? bandMenu = _bandMenu;
		if (bandMenu != null && ((BaseWindow)bandMenu).IsOpen)
		{
			((BaseWindow)_bandMenu).Close();
		}
	}

	public void OpenChannelsMenu()
	{
		if (_channelsMenu == null)
		{
			_channelsMenu = new ChannelsMenu(this);
		}
		_channelsMenu.Populate();
		((BaseWindow)_channelsMenu).OpenCenteredRight();
	}

	public void CloseChannelsMenu()
	{
		ChannelsMenu? channelsMenu = _channelsMenu;
		if (channelsMenu != null && ((BaseWindow)channelsMenu).IsOpen)
		{
			((BaseWindow)_channelsMenu).Close();
		}
	}
}
