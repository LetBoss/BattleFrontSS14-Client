using System;
using System.Numerics;
using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Alerts.Controls;

public sealed class AlertControl : BaseButton
{
	[Dependency]
	private IEntityManager _entityManager;

	private readonly SpriteSystem _sprite;

	private (TimeSpan Start, TimeSpan End)? _cooldown;

	private string? _dynamicMessage;

	private short? _severity;

	private readonly SpriteView _icon;

	private readonly CooldownGraphic _cooldownGraphic;

	private EntityUid _spriteViewEntity;

	public AlertPrototype Alert { get; }

	public (TimeSpan Start, TimeSpan End)? Cooldown
	{
		get
		{
			return _cooldown;
		}
		set
		{
			_cooldown = value;
			if (((Control)this).SuppliedTooltip is ActionAlertTooltip actionAlertTooltip)
			{
				actionAlertTooltip.Cooldown = value;
			}
		}
	}

	public string? DynamicMessage
	{
		get
		{
			return _dynamicMessage;
		}
		set
		{
			_dynamicMessage = value;
			if (((Control)this).SuppliedTooltip is ActionAlertTooltip actionAlertTooltip)
			{
				actionAlertTooltip.DynamicMessage = value;
			}
		}
	}

	public AlertControl(AlertPrototype alert, short? severity)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		((BaseButton)this).MuteSounds = true;
		IoCManager.InjectDependencies<AlertControl>(this);
		_sprite = _entityManager.System<SpriteSystem>();
		((Control)this).TooltipSupplier = new TooltipSupplier(SupplyTooltip);
		Alert = alert;
		((Control)this).HorizontalAlignment = (HAlignment)1;
		_severity = severity;
		_icon = new SpriteView
		{
			Scale = new Vector2(2f, 2f),
			MaxSize = new Vector2(64f, 64f),
			Stretch = (StretchMode)0,
			HorizontalAlignment = (HAlignment)1
		};
		SetupIcon();
		((Control)this).Children.Add((Control)(object)_icon);
		CooldownGraphic cooldownGraphic = new CooldownGraphic();
		((Control)cooldownGraphic).MaxSize = new Vector2(64f, 64f);
		_cooldownGraphic = cooldownGraphic;
		((Control)this).Children.Add((Control)(object)_cooldownGraphic);
	}

	private Control SupplyTooltip(Control? sender)
	{
		FormattedMessage name = FormattedMessage.FromMarkupOrThrow(Loc.GetString(Alert.Name));
		FormattedMessage desc = FormattedMessage.FromMarkupOrThrow(Loc.GetString(Alert.Description));
		return (Control)(object)new ActionAlertTooltip(name, desc)
		{
			Cooldown = Cooldown,
			DynamicMessage = DynamicMessage
		};
	}

	public void SetSeverity(short? severity)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (_severity == severity)
		{
			return;
		}
		_severity = severity;
		SpriteComponent item = default(SpriteComponent);
		if (_entityManager.TryGetComponent<SpriteComponent>(_spriteViewEntity, ref item))
		{
			SpriteSpecifier icon = Alert.GetIcon(_severity);
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((_spriteViewEntity, item)), (Enum)AlertVisualLayers.Base, ref num, false))
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((_spriteViewEntity, item)), num, icon);
			}
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		((Control)this).UserInterfaceManager.GetUIController<AlertsUIController>().UpdateAlertSpriteEntity(_spriteViewEntity, Alert);
		if (!Cooldown.HasValue)
		{
			((Control)_cooldownGraphic).Visible = false;
			_cooldownGraphic.Progress = 0f;
		}
		else
		{
			_cooldownGraphic.FromTime(Cooldown.Value.Start, Cooldown.Value.End);
		}
	}

	private void SetupIcon()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!_entityManager.Deleted(_spriteViewEntity))
		{
			_entityManager.QueueDeleteEntity((EntityUid?)_spriteViewEntity);
		}
		_spriteViewEntity = _entityManager.Spawn(EntProtoId.op_Implicit(Alert.AlertViewEntity), (ComponentRegistry)null, true);
		SpriteComponent item = default(SpriteComponent);
		if (_entityManager.TryGetComponent<SpriteComponent>(_spriteViewEntity, ref item))
		{
			SpriteSpecifier icon = Alert.GetIcon(_severity);
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((_spriteViewEntity, item)), (Enum)AlertVisualLayers.Base, ref num, false))
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((_spriteViewEntity, item)), num, icon);
			}
		}
		_icon.SetEntity((EntityUid?)_spriteViewEntity);
	}

	protected override void EnteredTree()
	{
		((Control)this).EnteredTree();
		SetupIcon();
	}

	protected override void ExitedTree()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).ExitedTree();
		if (!_entityManager.Deleted(_spriteViewEntity))
		{
			_entityManager.QueueDeleteEntity((EntityUid?)_spriteViewEntity);
		}
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((BaseButton)this).Dispose(disposing);
		if (!_entityManager.Deleted(_spriteViewEntity))
		{
			_entityManager.QueueDeleteEntity((EntityUid?)_spriteViewEntity);
		}
	}
}
