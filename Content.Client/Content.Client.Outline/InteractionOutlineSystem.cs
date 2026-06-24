using System;
using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Client.Interactable.Components;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Content.Shared.Interaction;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client.Outline;

public sealed class InteractionOutlineSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	private bool _enabled = true;

	private bool _cvarEnabled = true;

	private EntityUid? _lastHoveredEntity;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configManager, CCVars.OutlineEnabled, (Action<bool>)SetCvarEnabled, false);
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedEyeSystem));
	}

	public void SetCvarEnabled(bool cvarEnabled)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_cvarEnabled = cvarEnabled;
		InteractionOutlineComponent interactionOutlineComponent = default(InteractionOutlineComponent);
		if (!_cvarEnabled && _lastHoveredEntity.HasValue && !((EntitySystem)this).Deleted(_lastHoveredEntity) && ((EntitySystem)this).TryComp<InteractionOutlineComponent>(_lastHoveredEntity, ref interactionOutlineComponent))
		{
			interactionOutlineComponent.OnMouseLeave(_lastHoveredEntity.Value);
		}
	}

	public void SetEnabled(bool enabled)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (enabled != _enabled)
		{
			_enabled = enabled;
			InteractionOutlineComponent interactionOutlineComponent = default(InteractionOutlineComponent);
			if (!enabled && _lastHoveredEntity.HasValue && !((EntitySystem)this).Deleted(_lastHoveredEntity) && ((EntitySystem)this).TryComp<InteractionOutlineComponent>(_lastHoveredEntity, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.OnMouseLeave(_lastHoveredEntity.Value);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		if (!_enabled || !_cvarEnabled)
		{
			return;
		}
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		if (localSession == null || !(_stateManager.CurrentState is GameplayStateBase gameplayStateBase))
		{
			return;
		}
		EntityUid? val = null;
		int renderScale = 1;
		Control currentlyHovered = _uiManager.CurrentlyHovered;
		IViewportControl val2 = (IViewportControl)(object)((currentlyHovered is IViewportControl) ? currentlyHovered : null);
		if (val2 != null)
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				MapCoordinates coordinates = val2.PixelToMap(_inputManager.MouseScreenPosition.Position);
				if (val2 is ScalingViewport scalingViewport)
				{
					renderScale = scalingViewport.CurrentRenderScale;
					val = gameplayStateBase.GetClickedEntity(coordinates, scalingViewport.Eye);
				}
				else
				{
					val = gameplayStateBase.GetClickedEntity(coordinates);
				}
				goto IL_00ed;
			}
		}
		if (_uiManager.CurrentlyHovered is EntityMenuElement entityMenuElement)
		{
			val = entityMenuElement.Entity;
			renderScale = _eyeManager.MainViewport.GetRenderScale();
		}
		goto IL_00ed;
		IL_00ed:
		bool inInteractionRange = false;
		if (localSession.AttachedEntity.HasValue && !((EntitySystem)this).Deleted(val))
		{
			inInteractionRange = _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(localSession.AttachedEntity.Value), Entity<TransformComponent>.op_Implicit(val.Value));
		}
		EntityUid? val3 = val;
		EntityUid? lastHoveredEntity = _lastHoveredEntity;
		InteractionOutlineComponent interactionOutlineComponent = default(InteractionOutlineComponent);
		if (val3.HasValue == lastHoveredEntity.HasValue && (!val3.HasValue || val3.GetValueOrDefault() == lastHoveredEntity.GetValueOrDefault()))
		{
			if (val.HasValue && ((EntitySystem)this).TryComp<InteractionOutlineComponent>(val, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.UpdateInRange(val.Value, inInteractionRange, renderScale);
			}
			return;
		}
		if (_lastHoveredEntity.HasValue && !((EntitySystem)this).Deleted(_lastHoveredEntity) && ((EntitySystem)this).TryComp<InteractionOutlineComponent>(_lastHoveredEntity, ref interactionOutlineComponent))
		{
			interactionOutlineComponent.OnMouseLeave(_lastHoveredEntity.Value);
		}
		_lastHoveredEntity = val;
		if (_lastHoveredEntity.HasValue && ((EntitySystem)this).TryComp<InteractionOutlineComponent>(_lastHoveredEntity, ref interactionOutlineComponent))
		{
			interactionOutlineComponent.OnMouseEnter(_lastHoveredEntity.Value, inInteractionRange, renderScale);
		}
	}
}
