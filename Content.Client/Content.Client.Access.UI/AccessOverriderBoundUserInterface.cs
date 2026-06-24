using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI;

public sealed class AccessOverriderBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly SharedAccessOverriderSystem _accessOverriderSystem;

	private AccessOverriderWindow? _window;

	public AccessOverriderBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_accessOverriderSystem = base.EntMan.System<SharedAccessOverriderSystem>();
	}

	protected override void Open()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<AccessOverriderWindow>((BoundUserInterface)(object)this);
		RefreshAccess();
		((DefaultWindow)_window).Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		_window.OnSubmit += SubmitData;
		((BaseButton)_window.PrivilegedIdButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent(AccessOverriderComponent.PrivilegedIdCardSlotId));
		};
	}

	public override void OnProtoReload(PrototypesReloadedEventArgs args)
	{
		((BoundUserInterface)this).OnProtoReload(args);
		if (args.WasModified<AccessLevelPrototype>())
		{
			RefreshAccess();
			if (((BoundUserInterface)this).State != null)
			{
				_window?.UpdateState(_prototypeManager, (AccessOverriderComponent.AccessOverriderBoundUserInterfaceState)(object)((BoundUserInterface)this).State);
			}
		}
	}

	private void RefreshAccess()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		AccessOverriderComponent accessOverriderComponent = default(AccessOverriderComponent);
		List<ProtoId<AccessLevelPrototype>> list;
		if (base.EntMan.TryGetComponent<AccessOverriderComponent>(((BoundUserInterface)this).Owner, ref accessOverriderComponent))
		{
			list = accessOverriderComponent.AccessLevels;
			list.Sort();
		}
		else
		{
			list = new List<ProtoId<AccessLevelPrototype>>();
			((EntitySystem)_accessOverriderSystem).Log.Error($"No AccessOverrider component found for {base.EntMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BoundUserInterface)this).Owner))}!");
		}
		_window?.SetAccessLevels(_prototypeManager, list);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		AccessOverriderComponent.AccessOverriderBoundUserInterfaceState state2 = (AccessOverriderComponent.AccessOverriderBoundUserInterfaceState)(object)state;
		_window?.UpdateState(_prototypeManager, state2);
	}

	public void SubmitData(List<ProtoId<AccessLevelPrototype>> newAccessList)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AccessOverriderComponent.WriteToTargetAccessReaderIdMessage(newAccessList));
	}
}
