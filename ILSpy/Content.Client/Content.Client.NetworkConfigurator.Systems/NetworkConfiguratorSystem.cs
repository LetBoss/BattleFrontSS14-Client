using System;
using Content.Client.Actions;
using Content.Client.Items;
using Content.Client.Message;
using Content.Shared.Actions.Components;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.NetworkConfigurator.Systems;

public sealed class NetworkConfiguratorSystem : SharedNetworkConfiguratorSystem
{
	private sealed class StatusControl : Control
	{
		private readonly RichTextLabel _label;

		private readonly NetworkConfiguratorComponent _configurator;

		private readonly string _keyBindingName;

		private bool? _linkModeActive;

		public StatusControl(NetworkConfiguratorComponent configurator, string keyBindingName)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			_configurator = configurator;
			_keyBindingName = keyBindingName;
			_label = new RichTextLabel
			{
				StyleClasses = { "ItemStatus" }
			};
			((Control)this).AddChild((Control)(object)_label);
		}

		protected override void FrameUpdate(FrameEventArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((Control)this).FrameUpdate(args);
			if (!_linkModeActive.HasValue || _linkModeActive != _configurator.LinkModeActive)
			{
				_linkModeActive = _configurator.LinkModeActive;
				string text = ((_linkModeActive == true) ? "network-configurator-examine-mode-link" : "network-configurator-examine-mode-list");
				_label.SetMarkup(Loc.GetString("network-configurator-item-status-label", new(string, object)[2]
				{
					("mode", Loc.GetString(text)),
					("keybinding", _keyBindingName)
				}));
			}
		}
	}

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private ActionsSystem _actions;

	[Dependency]
	private IInputManager _inputManager;

	private static readonly EntProtoId Action = EntProtoId.op_Implicit("ActionClearNetworkLinkOverlays");

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ClearAllOverlaysEvent>((EntityEventHandler<ClearAllOverlaysEvent>)delegate
		{
			ClearAllOverlays();
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.ItemStatus<NetworkConfiguratorComponent>((Func<Entity<NetworkConfiguratorComponent>, Control?>)OnCollectItemStatus);
	}

	private Control OnCollectItemStatus(Entity<NetworkConfiguratorComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		IKeyBinding val = default(IKeyBinding);
		_inputManager.TryGetKeyBinding(ContentKeyFunctions.AltUseItemInHand, ref val);
		return (Control)(object)new StatusControl(Entity<NetworkConfiguratorComponent>.op_Implicit(entity), ((val != null) ? val.GetKeyString() : null) ?? "");
	}

	public bool ConfiguredListIsTracked(EntityUid uid, NetworkConfiguratorComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<NetworkConfiguratorComponent>(uid, ref component, true) && component.ActiveDeviceList.HasValue)
		{
			return ((EntitySystem)this).HasComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
		}
		return false;
	}

	public void ToggleVisualization(EntityUid uid, bool toggle, NetworkConfiguratorComponent? component = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (!((ISharedPlayerManager)_playerManager).LocalEntity.HasValue || !((EntitySystem)this).Resolve<NetworkConfiguratorComponent>(uid, ref component, true) || !component.ActiveDeviceList.HasValue)
		{
			return;
		}
		if (!toggle)
		{
			((EntitySystem)this).RemComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
			NetworkConfiguratorLinkOverlay networkConfiguratorLinkOverlay = default(NetworkConfiguratorLinkOverlay);
			if (_overlay.TryGetOverlay<NetworkConfiguratorLinkOverlay>(ref networkConfiguratorLinkOverlay))
			{
				networkConfiguratorLinkOverlay.Colors.Remove(component.ActiveDeviceList.Value);
				if (networkConfiguratorLinkOverlay.Colors.Count <= 0)
				{
					ActionsSystem actions = _actions;
					EntityUid? action = networkConfiguratorLinkOverlay.Action;
					actions.RemoveAction(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
					_overlay.RemoveOverlay<NetworkConfiguratorLinkOverlay>();
				}
			}
		}
		else
		{
			if (!_overlay.HasOverlay<NetworkConfiguratorLinkOverlay>())
			{
				NetworkConfiguratorLinkOverlay networkConfiguratorLinkOverlay2 = new NetworkConfiguratorLinkOverlay();
				_overlay.AddOverlay((Overlay)(object)networkConfiguratorLinkOverlay2);
				EntityUid value = ((ISharedPlayerManager)_playerManager).LocalEntity.Value;
				networkConfiguratorLinkOverlay2.Action = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(Action), (ComponentRegistry)null, true);
				_actions.AddActionDirect(Entity<ActionsComponent>.op_Implicit(value), Entity<ActionComponent>.op_Implicit(networkConfiguratorLinkOverlay2.Action.Value));
			}
			((EntitySystem)this).EnsureComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
		}
	}

	public void ClearAllOverlays()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		NetworkConfiguratorLinkOverlay networkConfiguratorLinkOverlay = default(NetworkConfiguratorLinkOverlay);
		if (_overlay.TryGetOverlay<NetworkConfiguratorLinkOverlay>(ref networkConfiguratorLinkOverlay))
		{
			EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent> val = ((EntitySystem)this).EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent>();
			EntityUid val2 = default(EntityUid);
			NetworkConfiguratorActiveLinkOverlayComponent networkConfiguratorActiveLinkOverlayComponent = default(NetworkConfiguratorActiveLinkOverlayComponent);
			while (val.MoveNext(ref val2, ref networkConfiguratorActiveLinkOverlayComponent))
			{
				((EntitySystem)this).RemCompDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(val2);
			}
			ActionsSystem actions = _actions;
			EntityUid? action = networkConfiguratorLinkOverlay.Action;
			actions.RemoveAction(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			_overlay.RemoveOverlay((Overlay)(object)networkConfiguratorLinkOverlay);
		}
	}
}
