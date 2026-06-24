using System;
using Content.Client._RMC14.NightVision;
using Content.Client.Movement.Systems;
using Content.Shared._PUBG.Ghost;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Ghost;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Ghost;

public sealed class GhostSystem : SharedGhostSystem
{
	[Dependency]
	private IClientConsoleHost _console;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private PointLightSystem _pointLightSystem;

	[Dependency]
	private ContentEyeSystem _contentEye;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IOverlayManager _overlay;

	private bool _ghostVisibility = true;

	public int AvailableGhostRoleCount { get; private set; }

	private bool GhostVisibility
	{
		get
		{
			return _ghostVisibility;
		}
		set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (_ghostVisibility == value)
			{
				return;
			}
			_ghostVisibility = value;
			AllEntityQueryEnumerator<GhostComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<GhostComponent, SpriteComponent>();
			EntityUid val2 = default(EntityUid);
			GhostComponent ghostComponent = default(GhostComponent);
			SpriteComponent item = default(SpriteComponent);
			while (val.MoveNext(ref val2, ref ghostComponent, ref item))
			{
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val3 = Entity<SpriteComponent>.op_Implicit((val2, item));
				int num;
				if (!value)
				{
					EntityUid val4 = val2;
					EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
					num = ((localEntity.HasValue && val4 == localEntity.GetValueOrDefault()) ? 1 : 0);
				}
				else
				{
					num = 1;
				}
				sprite.SetVisible(val3, (byte)num != 0);
			}
		}
	}

	public GhostComponent? Player => ((EntitySystem)this).CompOrNull<GhostComponent>(((ISharedPlayerManager)_playerManager).LocalEntity);

	public bool IsGhost => Player != null;

	public event Action<GhostComponent>? PlayerRemoved;

	public event Action<GhostComponent>? PlayerUpdated;

	public event Action<GhostComponent>? PlayerAttached;

	public event Action? PlayerDetached;

	public event Action<GhostWarpsResponseEvent>? GhostWarpsResponse;

	public event Action<GhostUpdateGhostRoleCountEvent>? GhostRoleCountUpdated;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, ComponentStartup>((ComponentEventHandler<GhostComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, ComponentRemove>((ComponentEventHandler<GhostComponent, ComponentRemove>)OnGhostRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<GhostComponent, AfterAutoHandleStateEvent>)OnGhostState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<GhostComponent, LocalPlayerAttachedEvent>)OnGhostPlayerAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<GhostComponent, LocalPlayerDetachedEvent>)OnGhostPlayerDetach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GhostWarpsResponseEvent>((EntityEventHandler<GhostWarpsResponseEvent>)OnGhostWarpsResponse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GhostUpdateGhostRoleCountEvent>((EntityEventHandler<GhostUpdateGhostRoleCountEvent>)OnUpdateGhostRoleCount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeComponent, ToggleLightingActionEvent>((ComponentEventHandler<EyeComponent, ToggleLightingActionEvent>)OnToggleLighting, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeComponent, ToggleFoVActionEvent>((ComponentEventHandler<EyeComponent, ToggleFoVActionEvent>)OnToggleFoV, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, ToggleGhostsActionEvent>((ComponentEventHandler<GhostComponent, ToggleGhostsActionEvent>)OnToggleGhosts, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, GhostComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			SpriteSystem sprite = _sprite;
			Entity<SpriteComponent> val = Entity<SpriteComponent>.op_Implicit((uid, item));
			int num;
			if (!GhostVisibility)
			{
				EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
				num = ((localEntity.HasValue && uid == localEntity.GetValueOrDefault()) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			sprite.SetVisible(val, (byte)num != 0);
		}
	}

	private void OnToggleLighting(EntityUid uid, EyeComponent component, ToggleLightingActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			PointLightComponent val = default(PointLightComponent);
			((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val);
			if (!component.DrawLight)
			{
				Popup.PopupEntity(((EntitySystem)this).Loc.GetString("ghost-gui-toggle-lighting-manager-popup-normal"), args.Performer);
				_contentEye.RequestEye(component.DrawFov, drawLight: true);
			}
			else if (val != null && !((SharedPointLightComponent)val).Enabled && !_overlay.HasOverlay<HalfNightVisionBrightnessOverlay>())
			{
				Popup.PopupEntity(((EntitySystem)this).Loc.GetString("ghost-gui-toggle-lighting-manager-popup-personal-light"), args.Performer);
				((SharedPointLightSystem)_pointLightSystem).SetEnabled(uid, true, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
			}
			else if (val != null && ((SharedPointLightComponent)val).Enabled && !_overlay.HasOverlay<HalfNightVisionBrightnessOverlay>())
			{
				Popup.PopupEntity(((EntitySystem)this).Loc.GetString("rmc-ghost-gui-toggle-lighting-manager-popup-halfbright"), args.Performer);
				((SharedPointLightSystem)_pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
				_overlay.AddOverlay((Overlay)(object)new HalfNightVisionBrightnessOverlay());
			}
			else
			{
				Popup.PopupEntity(((EntitySystem)this).Loc.GetString("ghost-gui-toggle-lighting-manager-popup-fullbright"), args.Performer);
				_contentEye.RequestEye(component.DrawFov, drawLight: false);
				((SharedPointLightSystem)_pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
				_overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnToggleFoV(EntityUid uid, EyeComponent component, ToggleFoVActionEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			Popup.PopupEntity(((EntitySystem)this).Loc.GetString("ghost-gui-toggle-fov-popup"), args.Performer);
			_contentEye.RequestToggleFov(uid, component);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnToggleGhosts(EntityUid uid, GhostComponent component, ToggleGhostsActionEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			string text = (GhostVisibility ? "ghost-gui-toggle-ghost-visibility-popup-off" : "ghost-gui-toggle-ghost-visibility-popup-on");
			Popup.PopupEntity(((EntitySystem)this).Loc.GetString(text), args.Performer);
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (localEntity.HasValue && uid == localEntity.GetValueOrDefault())
			{
				ToggleGhostVisibility();
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGhostRemove(EntityUid uid, GhostComponent component, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? toggleLightingActionEntity = component.ToggleLightingActionEntity;
		actions.RemoveAction(performer, toggleLightingActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleLightingActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions2 = _actions;
		Entity<ActionsComponent> performer2 = Entity<ActionsComponent>.op_Implicit(uid);
		toggleLightingActionEntity = component.ToggleFoVActionEntity;
		actions2.RemoveAction(performer2, toggleLightingActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleLightingActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions3 = _actions;
		Entity<ActionsComponent> performer3 = Entity<ActionsComponent>.op_Implicit(uid);
		toggleLightingActionEntity = component.ToggleGhostsActionEntity;
		actions3.RemoveAction(performer3, toggleLightingActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleLightingActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions4 = _actions;
		Entity<ActionsComponent> performer4 = Entity<ActionsComponent>.op_Implicit(uid);
		toggleLightingActionEntity = component.ToggleGhostHearingActionEntity;
		actions4.RemoveAction(performer4, toggleLightingActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleLightingActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		toggleLightingActionEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (toggleLightingActionEntity.HasValue && !(uid != toggleLightingActionEntity.GetValueOrDefault()))
		{
			_overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
			GhostVisibility = false;
			this.PlayerRemoved?.Invoke(component);
		}
	}

	private void OnGhostPlayerAttach(EntityUid uid, GhostComponent component, LocalPlayerAttachedEvent localPlayerAttachedEvent)
	{
		GhostVisibility = true;
		this.PlayerAttached?.Invoke(component);
	}

	private void OnGhostState(EntityUid uid, GhostComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), 0, component.Color);
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(uid != localEntity.GetValueOrDefault()))
		{
			this.PlayerUpdated?.Invoke(component);
		}
	}

	private void OnGhostPlayerDetach(EntityUid uid, GhostComponent component, LocalPlayerDetachedEvent args)
	{
		GhostVisibility = false;
		this.PlayerDetached?.Invoke();
		_overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
	}

	private void OnGhostWarpsResponse(GhostWarpsResponseEvent msg)
	{
		if (IsGhost)
		{
			this.GhostWarpsResponse?.Invoke(msg);
		}
	}

	private void OnUpdateGhostRoleCount(GhostUpdateGhostRoleCountEvent msg)
	{
		AvailableGhostRoleCount = msg.AvailableGhostRoles;
		this.GhostRoleCountUpdated?.Invoke(msg);
	}

	public void RequestWarps()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new GhostWarpsRequestEvent());
	}

	public void ReturnToBody()
	{
		GhostReturnToBodyRequest ghostReturnToBodyRequest = new GhostReturnToBodyRequest();
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ghostReturnToBodyRequest);
	}

	public void OpenGhostRoles()
	{
		((IConsoleHost)_console).RemoteExecuteCommand((ICommonSession)null, "ghostroles");
	}

	public void RequestGhostBar()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgGhostBarTeleportRequestEvent());
	}

	public void RequestLobbyRespawn()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgGhostLobbyRespawnRequestEvent());
	}

	public void ToggleGhostVisibility(bool? visibility = null)
	{
		GhostVisibility = visibility ?? (!GhostVisibility);
	}
}
