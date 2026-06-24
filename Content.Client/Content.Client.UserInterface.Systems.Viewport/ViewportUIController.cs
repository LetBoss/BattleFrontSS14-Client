using System;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Viewport;

public sealed class ViewportUIController : UIController
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IPlayerManager _playerMan;

	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IConfigurationManager _configurationManager;

	public static readonly Vector2i ViewportSize = Vector2i.op_Implicit((672, 480));

	public const int ViewportHeight = 15;

	private MainViewport? Viewport
	{
		get
		{
			UIScreen activeScreen = base.UIManager.ActiveScreen;
			if (activeScreen == null)
			{
				return null;
			}
			return activeScreen.GetWidget<MainViewport>();
		}
	}

	public override void Initialize()
	{
		_configurationManager.OnValueChanged<int>(CCVars.ViewportMinimumWidth, (Action<int>)delegate
		{
			UpdateViewportRatio();
		}, false);
		_configurationManager.OnValueChanged<int>(CCVars.ViewportMaximumWidth, (Action<int>)delegate
		{
			UpdateViewportRatio();
		}, false);
		_configurationManager.OnValueChanged<int>(CCVars.ViewportWidth, (Action<int>)delegate
		{
			UpdateViewportRatio();
		}, false);
		_configurationManager.OnValueChanged<bool>(CCVars.ViewportVerticalFit, (Action<bool>)delegate
		{
			UpdateViewportRatio();
		}, false);
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
	}

	private void OnScreenLoad()
	{
		ReloadViewport();
	}

	private void UpdateViewportRatio()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (Viewport != null)
		{
			int cVar = _configurationManager.GetCVar<int>(CCVars.ViewportMinimumWidth);
			int cVar2 = _configurationManager.GetCVar<int>(CCVars.ViewportMaximumWidth);
			int num = _configurationManager.GetCVar<int>(CCVars.ViewportWidth);
			if (_configurationManager.GetCVar<bool>(CCVars.ViewportVerticalFit) && _configurationManager.GetCVar<bool>(CCVars.ViewportStretch))
			{
				num = cVar2;
			}
			else if (num < cVar || num > cVar2)
			{
				num = CCVars.ViewportWidth.DefaultValue;
			}
			Viewport.Viewport.ViewportSize = Vector2i.op_Implicit((32 * num, 480));
			Viewport.UpdateCfg();
		}
	}

	public void ReloadViewport()
	{
		if (Viewport != null)
		{
			UpdateViewportRatio();
			((Control)Viewport.Viewport).HorizontalExpand = true;
			((Control)Viewport.Viewport).VerticalExpand = true;
			_eyeManager.MainViewport = (IViewportControl)(object)Viewport.Viewport;
		}
	}

	public override void FrameUpdate(FrameEventArgs e)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (Viewport == null)
		{
			return;
		}
		((UIController)this).FrameUpdate(e);
		Viewport.Viewport.Eye = _eyeManager.CurrentEye;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerMan).LocalEntity;
		if (!(_eyeManager.CurrentEye.Position != default(MapCoordinates)) && localEntity.HasValue)
		{
			EyeComponent val = default(EyeComponent);
			_entMan.TryGetComponent<EyeComponent>(localEntity, ref val);
			if ((object)val?.Eye != _eyeManager.CurrentEye || !(_entMan.GetComponent<TransformComponent>(localEntity.Value).MapID == MapId.Nullspace))
			{
				((UIController)this).Log.Warning($"Main viewport's eye is in nullspace (main eye is null?). Attached entity: {_entMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(localEntity.Value))}. Entity has eye comp: {val != null}");
			}
		}
	}
}
