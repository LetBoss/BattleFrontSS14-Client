using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.WorldEdit;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.WorldEdit;

public sealed class WorldEditPreviewSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	private bool _isPlacing;

	private Angle _rotation = Angle.Zero;

	private readonly List<EntityUid> _previewEntities = new List<EntityUid>();

	private readonly List<WorldEditPreviewEntityData> _entityData = new List<WorldEditPreviewEntityData>();

	private int _buildingWidth;

	private int _buildingHeight;

	public bool IsPlacing => _isPlacing;

	public override void Initialize()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<WorldEditPreviewDataEvent>((EntityEventHandler<WorldEditPreviewDataEvent>)OnPreviewData, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleCancel), true, true)).Register<WorldEditPreviewSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<WorldEditPreviewSystem>();
		ClearPreview();
	}

	private void OnPreviewData(WorldEditPreviewDataEvent msg)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ClearPreview();
		_entityData.AddRange(msg.Entities);
		_buildingWidth = msg.Width;
		_buildingHeight = msg.Height;
		_rotation = Angle.FromDegrees((double)msg.Degrees);
		_isPlacing = true;
		SpawnPreviewEntities();
	}

	private void SpawnPreviewEntities()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		ClearPreviewEntities();
		SpriteComponent item = default(SpriteComponent);
		foreach (WorldEditPreviewEntityData entityDatum in _entityData)
		{
			if (_protoManager.HasIndex<EntityPrototype>(entityDatum.PrototypeId))
			{
				EntityUid val = ((EntitySystem)this).Spawn(entityDatum.PrototypeId, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
				_previewEntities.Add(val);
				if (((EntitySystem)this).TryComp<SpriteComponent>(val, ref item))
				{
					_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val, item)), new Color((byte)150, byte.MaxValue, (byte)150, (byte)200));
				}
			}
		}
	}

	private void ClearPreviewEntities()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid previewEntity in _previewEntities)
		{
			if (((EntitySystem)this).Exists(previewEntity))
			{
				((EntitySystem)this).QueueDel((EntityUid?)previewEntity);
			}
		}
		_previewEntities.Clear();
	}

	public void ClearPreview()
	{
		ClearPreviewEntities();
		_entityData.Clear();
		_isPlacing = false;
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!_isPlacing || (int)args.State != 1)
		{
			return false;
		}
		MapCoordinates val = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(val, ref val2, ref val3))
		{
			return false;
		}
		EntityCoordinates val4 = default(EntityCoordinates);
		((EntityCoordinates)(ref val4))._002Ector(val2, Vector2.Transform(val.Position, _transform.GetInvWorldMatrix(val2)));
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new WorldEditPlacePreviewEvent(((EntitySystem)this).GetNetCoordinates(val4, (MetaDataComponent)null), _rotation));
		ClearPreview();
		return true;
	}

	private bool HandleCancel(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (!_isPlacing || (int)args.State != 1)
		{
			return false;
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new WorldEditCancelPreviewEvent());
		ClearPreview();
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_isPlacing || _previewEntities.Count == 0)
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (!((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
		{
			return;
		}
		MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		if (val.MapId == MapId.Nullspace || !_mapManager.TryFindGridAt(val, ref val2, ref val3))
		{
			return;
		}
		Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(val2);
		Vector2i val4 = Vector2Helpers.Floored(Vector2.Transform(val.Position, invWorldMatrix));
		Vector2 vector = new Vector2(val4.X, val4.Y);
		for (int i = 0; i < _previewEntities.Count && i < _entityData.Count; i++)
		{
			EntityUid val5 = _previewEntities[i];
			WorldEditPreviewEntityData worldEditPreviewEntityData = _entityData[i];
			if (((EntitySystem)this).Exists(val5))
			{
				Vector2 vector2 = RotatePosition(worldEditPreviewEntityData.RelativePosition, _buildingWidth, _buildingHeight, _rotation) + vector;
				_transform.SetCoordinates(val5, new EntityCoordinates(val2, vector2));
				_transform.SetLocalRotation(val5, worldEditPreviewEntityData.Rotation + _rotation, (TransformComponent)null);
			}
		}
	}

	private static Vector2 RotatePosition(Vector2 pos, int width, int height, Angle rotation)
	{
		int num = (int)MathF.Round((float)((Angle)(ref rotation)).Degrees) % 360;
		if (num < 0)
		{
			num += 360;
		}
		return num switch
		{
			0 => pos, 
			90 => new Vector2((float)height - pos.Y, pos.X), 
			180 => new Vector2((float)width - pos.X, (float)height - pos.Y), 
			270 => new Vector2(pos.Y, (float)width - pos.X), 
			_ => pos, 
		};
	}
}
