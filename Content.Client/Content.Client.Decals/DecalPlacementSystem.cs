using System;
using System.Numerics;
using Content.Client.Actions;
using Content.Client.Decals.Overlays;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Decals;

public sealed class DecalPlacementSystem : EntitySystem
{
	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private InputSystem _inputSystem;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	public static readonly EntProtoId DecalAction = EntProtoId.op_Implicit("BaseMappingDecalAction");

	private string? _decalId;

	private Color _decalColor = Color.White;

	private Angle _decalAngle = Angle.Zero;

	private bool _snap;

	private int _zIndex;

	private bool _cleanable;

	private bool _active;

	private bool _placing;

	private bool _erasing;

	public (DecalPrototype? Decal, bool Snap, Angle Angle, Color Color) GetActiveDecal()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_active || _decalId == null)
		{
			return (Decal: null, Snap: false, Angle: Angle.Zero, Color: Color.Wheat);
		}
		return (Decal: _protoMan.Index<DecalPrototype>(_decalId), Snap: _snap, Angle: _decalAngle, Color: _decalColor);
	}

	public override void Initialize()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0074: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		_overlay.AddOverlay((Overlay)(object)new DecalPlacementOverlay(this, _transform, _sprite));
		CommandBinds.Builder.Bind(EngineKeyFunctions.EditorPlaceObject, (InputCmdHandler)new PointerStateInputCmdHandler((PointerInputCmdDelegate)delegate(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (!_active || _placing || _decalId == null)
			{
				return false;
			}
			_placing = true;
			if (_snap)
			{
				Vector2 vector = new Vector2((float)((double)MathF.Round(((EntityCoordinates)(ref coords)).X - 0.5f, MidpointRounding.AwayFromZero) + 0.5), (float)((double)MathF.Round(((EntityCoordinates)(ref coords)).Y - 0.5f, MidpointRounding.AwayFromZero) + 0.5));
				coords = ((EntityCoordinates)(ref coords)).WithPosition(vector);
			}
			coords = ((EntityCoordinates)(ref coords)).Offset(new Vector2(-0.5f, -0.5f));
			if (!((EntityCoordinates)(ref coords)).IsValid((IEntityManager)(object)base.EntityManager))
			{
				return false;
			}
			Decal decal = new Decal(coords.Position, _decalId, _decalColor, _decalAngle, _zIndex, _cleanable);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestDecalPlacementEvent(decal, ((EntitySystem)this).GetNetCoordinates(coords, (MetaDataComponent)null)));
			return true;
		}, (PointerInputCmdDelegate)delegate
		{
			if (!_active)
			{
				return false;
			}
			_placing = false;
			return true;
		}, true)).Bind(EngineKeyFunctions.EditorCancelPlace, (InputCmdHandler)new PointerStateInputCmdHandler((PointerInputCmdDelegate)delegate(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (!_active || _erasing)
			{
				return false;
			}
			_erasing = true;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestDecalRemovalEvent(((EntitySystem)this).GetNetCoordinates(coords, (MetaDataComponent)null)));
			return true;
		}, (PointerInputCmdDelegate)delegate
		{
			if (!_active)
			{
				return false;
			}
			_erasing = false;
			return true;
		}, true)).Register<DecalPlacementSystem>();
		((EntitySystem)this).SubscribeLocalEvent<FillActionSlotEvent>((EntityEventHandler<FillActionSlotEvent>)OnFillSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlaceDecalActionEvent>((EntityEventHandler<PlaceDecalActionEvent>)OnPlaceDecalAction, (Type[])null, (Type[])null);
	}

	private void OnPlaceDecalAction(PlaceDecalActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _transform.GetGrid(args.Target).HasValue)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (args.Snap)
			{
				Vector2 vector = new Vector2((float)((double)MathF.Round(((EntityCoordinates)(ref args.Target)).X - 0.5f, MidpointRounding.AwayFromZero) + 0.5), (float)((double)MathF.Round(((EntityCoordinates)(ref args.Target)).Y - 0.5f, MidpointRounding.AwayFromZero) + 0.5));
				args.Target = ((EntityCoordinates)(ref args.Target)).WithPosition(vector);
			}
			args.Target = ((EntityCoordinates)(ref args.Target)).Offset(new Vector2(-0.5f, -0.5f));
			Decal decal = new Decal(args.Target.Position, args.DecalId, args.Color, Angle.FromDegrees(args.Rotation), args.ZIndex, args.Cleanable);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestDecalPlacementEvent(decal, ((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null)));
		}
	}

	private void OnFillSlot(FillActionSlotEvent ev)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		DecalPrototype decalPrototype = default(DecalPrototype);
		if (_active && !_placing && !ev.Action.HasValue && _decalId != null && _protoMan.TryIndex<DecalPrototype>(_decalId, ref decalPrototype))
		{
			PlaceDecalActionEvent ev2 = new PlaceDecalActionEvent
			{
				DecalId = _decalId,
				Color = _decalColor,
				Rotation = ((Angle)(ref _decalAngle)).Degrees,
				Snap = _snap,
				ZIndex = _zIndex,
				Cleanable = _cleanable
			};
			EntityUid val = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(DecalAction), (ComponentRegistry)null, true);
			ActionComponent item = ((EntitySystem)this).Comp<ActionComponent>(val);
			(EntityUid, ActionComponent) tuple = (val, item);
			_actions.SetEvent(val, ev2);
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(tuple), decalPrototype.Sprite);
			_actions.SetIconColor(Entity<ActionComponent>.op_Implicit(tuple), _decalColor);
			_metaData.SetEntityName(val, $"{_decalId} ({((Color)(ref _decalColor)).ToHex()}, {(int)((Angle)(ref _decalAngle)).Degrees})", (MetaDataComponent)null, true);
			ev.Action = val;
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<DecalPlacementOverlay>();
		CommandBinds.Unregister<DecalPlacementSystem>();
	}

	public void UpdateDecalInfo(string id, Color color, float rotation, bool snap, int zIndex, bool cleanable)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_decalId = id;
		_decalColor = color;
		_decalAngle = Angle.FromDegrees((double)rotation);
		_snap = snap;
		_zIndex = zIndex;
		_cleanable = cleanable;
	}

	public void SetActive(bool active)
	{
		_active = active;
		if (_active)
		{
			_inputManager.Contexts.SetActiveContext("editor");
		}
		else
		{
			_inputSystem.SetEntityContextActive();
		}
	}
}
