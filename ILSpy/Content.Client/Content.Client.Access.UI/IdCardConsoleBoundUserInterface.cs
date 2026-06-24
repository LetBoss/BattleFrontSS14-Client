using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.CrewManifest;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI;

public sealed class IdCardConsoleBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _cfgManager;

	private readonly SharedIdCardConsoleSystem _idCardConsoleSystem;

	private IdCardConsoleWindow? _window;

	private int _maxNameLength;

	private int _maxIdJobLength;

	public IdCardConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_idCardConsoleSystem = base.EntMan.System<SharedIdCardConsoleSystem>();
		_maxNameLength = _cfgManager.GetCVar<int>(CCVars.MaxNameLength);
		_maxIdJobLength = _cfgManager.GetCVar<int>(CCVars.MaxIdJobLength);
	}

	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		IdCardConsoleComponent idCardConsoleComponent = default(IdCardConsoleComponent);
		List<ProtoId<AccessLevelPrototype>> accessLevels;
		if (base.EntMan.TryGetComponent<IdCardConsoleComponent>(((BoundUserInterface)this).Owner, ref idCardConsoleComponent))
		{
			accessLevels = idCardConsoleComponent.AccessLevels;
		}
		else
		{
			accessLevels = new List<ProtoId<AccessLevelPrototype>>();
			((EntitySystem)_idCardConsoleSystem).Log.Error($"No IdCardConsole component found for {base.EntMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BoundUserInterface)this).Owner))}!");
		}
		IdCardConsoleWindow idCardConsoleWindow = new IdCardConsoleWindow(this, _prototypeManager, accessLevels);
		((DefaultWindow)idCardConsoleWindow).Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		_window = idCardConsoleWindow;
		((BaseButton)_window.CrewManifestButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CrewManifestOpenUiMessage());
		};
		((BaseButton)_window.PrivilegedIdButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent(IdCardConsoleComponent.PrivilegedIdCardSlotId));
		};
		((BaseButton)_window.TargetIdButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent(IdCardConsoleComponent.TargetIdCardSlotId));
		};
		((BaseWindow)_window).OnClose += base.Close;
		((BaseWindow)_window).OpenCentered();
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			IdCardConsoleWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		IdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState state2 = (IdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState)(object)state;
		_window?.UpdateState(state2);
	}

	public void SubmitData(string newFullName, string newJobTitle, List<ProtoId<AccessLevelPrototype>> newAccessList, string newJobPrototype)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (newFullName.Length > _maxNameLength)
		{
			newFullName = newFullName.Substring(0, _maxNameLength);
		}
		if (newJobTitle.Length > _maxIdJobLength)
		{
			newJobTitle = newJobTitle.Substring(0, _maxIdJobLength);
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new IdCardConsoleComponent.WriteToTargetIdMessage(newFullName, newJobTitle, newAccessList, ProtoId<AccessLevelPrototype>.op_Implicit(newJobPrototype)));
	}
}
