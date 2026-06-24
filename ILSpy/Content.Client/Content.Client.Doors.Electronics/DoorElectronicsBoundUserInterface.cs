using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Doors.Electronics;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Doors.Electronics;

public sealed class DoorElectronicsBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	private DoorElectronicsConfigurationMenu? _window;

	public DoorElectronicsBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<DoorElectronicsConfigurationMenu>((BoundUserInterface)(object)this);
		_window.OnAccessChanged += UpdateConfiguration;
		Reset();
	}

	public override void OnProtoReload(PrototypesReloadedEventArgs args)
	{
		((BoundUserInterface)this).OnProtoReload(args);
		if (args.WasModified<AccessLevelPrototype>())
		{
			Reset();
		}
	}

	private void Reset()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		List<ProtoId<AccessLevelPrototype>> list = new List<ProtoId<AccessLevelPrototype>>();
		foreach (AccessLevelPrototype item in _prototypeManager.EnumeratePrototypes<AccessLevelPrototype>())
		{
			if (item.Name != null)
			{
				list.Add(ProtoId<AccessLevelPrototype>.op_Implicit(item.ID));
			}
		}
		list.Sort();
		_window?.Reset(_prototypeManager, list);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		DoorElectronicsConfigurationState state2 = (DoorElectronicsConfigurationState)(object)state;
		_window?.UpdateState(state2);
	}

	public void UpdateConfiguration(List<ProtoId<AccessLevelPrototype>> newAccessList)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DoorElectronicsUpdateConfigurationMessage(newAccessList));
	}
}
