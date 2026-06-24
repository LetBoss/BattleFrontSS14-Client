using System;
using System.Numerics;
using Content.Client._CIV14merka.GlobalMap;
using Content.Client.ContextMenu.UI;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderPurchasePlacementSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private CivGlobalMapSystem _globalMap;

	private string? _entryId;

	private string? _displayName;

	private EntProtoId? _prototypeId;

	private int _price;

	private bool _keepPlacing;

	private EntityUid? _preview;

	private bool _validPlacement;

	private EntityCoordinates _previewCoordinates = EntityCoordinates.Invalid;

	private MapCoordinates _previewMapCoordinates = MapCoordinates.Nullspace;

	public bool IsActive
	{
		get
		{
			if (_entryId != null)
			{
				return _prototypeId.HasValue;
			}
			return false;
		}
	}

	public string? ActiveName => _displayName;

	public override void Initialize()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true)).BindBefore(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleCancel), true, true), new Type[1] { typeof(EntityMenuUIController) }).Register<CivCommanderPurchasePlacementSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivCommanderPurchasePlacementSystem>();
		CancelPlacement();
	}

	public void BeginPlacement(string entryId, string displayName, EntProtoId prototypeId, int price, bool keepPlacing)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype val = default(EntityPrototype);
		if (_prototypeManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(prototypeId), ref val))
		{
			CancelPlacement();
			_entryId = entryId;
			_displayName = displayName;
			_prototypeId = prototypeId;
			_price = price;
			_keepPlacing = keepPlacing;
			EnsurePreview();
			UpdatePreview();
		}
	}

	public void CancelPlacement()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		_entryId = null;
		_displayName = null;
		_prototypeId = null;
		_price = 0;
		_keepPlacing = false;
		_validPlacement = false;
		_previewCoordinates = EntityCoordinates.Invalid;
		_previewMapCoordinates = MapCoordinates.Nullspace;
		EntityUid? preview = _preview;
		if (preview.HasValue)
		{
			EntityUid valueOrDefault = preview.GetValueOrDefault();
			if (((EntitySystem)this).Exists(valueOrDefault))
			{
				((EntitySystem)this).QueueDel((EntityUid?)valueOrDefault);
			}
		}
		_preview = null;
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (IsActive)
		{
			if (!IsCommander())
			{
				CancelPlacement();
				return;
			}
			EnsurePreview();
			UpdatePreview();
		}
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!IsActive || (int)args.State != 1)
		{
			return false;
		}
		if (_ui.CurrentlyHovered != null && !(_ui.CurrentlyHovered is IViewportControl))
		{
			return false;
		}
		UpdatePreview();
		if (!_validPlacement || _entryId == null)
		{
			return true;
		}
		_globalMap.RequestCommanderPlacePurchase(_entryId, _previewMapCoordinates.MapId, _previewMapCoordinates.Position);
		if (!_keepPlacing)
		{
			CancelPlacement();
		}
		return true;
	}

	private bool HandleCancel(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (!IsActive || (int)args.State != 1)
		{
			return false;
		}
		CancelPlacement();
		return true;
	}

	private void EnsurePreview()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!_preview.HasValue && _prototypeId.HasValue)
		{
			EntProtoId? prototypeId = _prototypeId;
			_preview = ((EntitySystem)this).Spawn(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		}
	}

	private void UpdatePreview()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? preview = _preview;
		if (!preview.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = preview.GetValueOrDefault();
		if (!((EntitySystem)this).Exists(valueOrDefault) || !IsActive)
		{
			return;
		}
		_validPlacement = false;
		_previewCoordinates = EntityCoordinates.Invalid;
		_previewMapCoordinates = MapCoordinates.Nullspace;
		ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
		if (!((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
		{
			_transform.SetCoordinates(valueOrDefault, EntityCoordinates.Invalid);
			return;
		}
		MapCoordinates val = _eye.PixelToMap(mouseScreenPosition);
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		if (val.MapId == MapId.Nullspace || !_mapManager.TryFindGridAt(val, ref val2, ref val3))
		{
			_transform.SetCoordinates(valueOrDefault, EntityCoordinates.Invalid);
			return;
		}
		Vector2i val4 = Vector2Helpers.Floored(Vector2.Transform(val.Position, _transform.GetInvWorldMatrix(val2)));
		EntityCoordinates val5 = default(EntityCoordinates);
		((EntityCoordinates)(ref val5))._002Ector(val2, Vector2i.op_Implicit(val4) + new Vector2(0.5f, 0.5f));
		MapCoordinates val6 = _transform.ToMapCoordinates(val5, true);
		_previewCoordinates = val5;
		_previewMapCoordinates = val6;
		_validPlacement = IsValidPlacement(val6);
		_transform.SetCoordinates(valueOrDefault, val5);
		SetPreviewColor(valueOrDefault, _validPlacement);
	}

	private bool IsValidPlacement(MapCoordinates coordinates)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) && civTeamMemberComponent.IsCommander)
			{
				if (_entryId != null && _globalMap.TryGetCommanderShopEntryPrice(_entryId, out var price))
				{
					_price = price;
				}
				if (_globalMap.GetCommanderCurrency() < _price)
				{
					return false;
				}
				EntityQueryEnumerator<CivCommanderPurchaseAnchorComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CivCommanderPurchaseAnchorComponent, TransformComponent>();
				EntityUid val2 = default(EntityUid);
				CivCommanderPurchaseAnchorComponent civCommanderPurchaseAnchorComponent = default(CivCommanderPurchaseAnchorComponent);
				TransformComponent val3 = default(TransformComponent);
				while (val.MoveNext(ref val2, ref civCommanderPurchaseAnchorComponent, ref val3))
				{
					if (string.Equals(civCommanderPurchaseAnchorComponent.RequiredSideId, civTeamMemberComponent.SideId, StringComparison.OrdinalIgnoreCase) && !(val3.MapID != coordinates.MapId) && Vector2.Distance(_transform.ToMapCoordinates(val3.Coordinates, true).Position, coordinates.Position) <= civCommanderPurchaseAnchorComponent.Range)
					{
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}

	private void SetPreviewColor(EntityUid uid, bool valid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), valid ? new Color((byte)90, (byte)220, (byte)120, (byte)190) : new Color((byte)220, (byte)90, (byte)90, (byte)170));
		}
	}

	private bool IsCommander()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent))
			{
				return civTeamMemberComponent.IsCommander;
			}
		}
		return false;
	}
}
