using System;
using System.Numerics;
using Content.Client.Atmos.Components;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Atmos.Overlays;

public sealed class GasTileOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	private readonly IEntityManager _entManager;

	private readonly IMapManager _mapManager;

	private readonly SharedMapSystem _mapSystem;

	private readonly SharedTransformSystem _xformSys;

	private readonly ShaderInstance _shader;

	private readonly float[] _timer;

	private readonly float[][] _frameDelays;

	private readonly int[] _frameCounter;

	private readonly Texture[][] _frames;

	private const int FireStates = 3;

	private const string FireRsiPath = "/Textures/Effects/fire.rsi";

	private readonly float[] _fireTimer = new float[3];

	private readonly float[][] _fireFrameDelays = new float[3][];

	private readonly int[] _fireFrameCounter = new int[3];

	private readonly Texture[][] _fireFrames = new Texture[3][];

	private int _gasCount;

	public const int GasOverlayZIndex = 11;

	public override OverlaySpace Space => (OverlaySpace)272;

	public GasTileOverlay(GasTileOverlaySystem system, IEntityManager entManager, IResourceCache resourceCache, IPrototypeManager protoMan, SpriteSystem spriteSys, SharedTransformSystem xformSys)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		_entManager = entManager;
		_mapManager = IoCManager.Resolve<IMapManager>();
		_mapSystem = entManager.System<SharedMapSystem>();
		_xformSys = xformSys;
		_shader = protoMan.Index<ShaderPrototype>(UnshadedShader).Instance();
		((Overlay)this).ZIndex = 11;
		_gasCount = system.VisibleGasId.Length;
		_timer = new float[_gasCount];
		_frameDelays = new float[_gasCount][];
		_frameCounter = new int[_gasCount];
		_frames = new Texture[_gasCount][];
		State val4 = default(State);
		for (int i = 0; i < _gasCount; i++)
		{
			GasPrototype gasPrototype = protoMan.Index<GasPrototype>(system.VisibleGasId[i].ToString());
			SpriteSpecifier val;
			if (!string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState))
			{
				val = (SpriteSpecifier)new Rsi(new ResPath(gasPrototype.GasOverlaySprite), gasPrototype.GasOverlayState);
			}
			else
			{
				if (string.IsNullOrEmpty(gasPrototype.GasOverlayTexture))
				{
					continue;
				}
				val = (SpriteSpecifier)new Texture(new ResPath(gasPrototype.GasOverlayTexture));
			}
			Rsi val2 = (Rsi)(object)((val is Rsi) ? val : null);
			if (val2 == null)
			{
				Texture val3 = (Texture)(object)((val is Texture) ? val : null);
				if (val3 != null)
				{
					_frames[i] = (Texture[])(object)new Texture[1] { spriteSys.Frame0((SpriteSpecifier)(object)val3) };
					_frameDelays[i] = Array.Empty<float>();
				}
				continue;
			}
			RSI rSI = resourceCache.GetResource<RSIResource>(val2.RsiPath, true).RSI;
			string rsiState = val2.RsiState;
			if (rSI.TryGetState(StateId.op_Implicit(rsiState), ref val4))
			{
				_frames[i] = val4.GetFrames((RsiDirection)0);
				_frameDelays[i] = val4.GetDelays();
				_frameCounter[i] = 0;
			}
		}
		RSI rSI2 = resourceCache.GetResource<RSIResource>("/Textures/Effects/fire.rsi", true).RSI;
		State val5 = default(State);
		for (int j = 0; j < 3; j++)
		{
			if (!rSI2.TryGetState(StateId.op_Implicit((j + 1).ToString()), ref val5))
			{
				throw new ArgumentOutOfRangeException($"Fire RSI doesn't have state \"{j}\"!");
			}
			_fireFrames[j] = val5.GetFrames((RsiDirection)0);
			_fireFrameDelays[j] = val5.GetDelays();
			_fireFrameCounter[j] = 0;
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Overlay)this).FrameUpdate(args);
		for (int i = 0; i < _gasCount; i++)
		{
			float[] array = _frameDelays[i];
			if (array.Length != 0)
			{
				int num = _frameCounter[i];
				_timer[i] += ((FrameEventArgs)(ref args)).DeltaSeconds;
				float num2 = array[num];
				if (!(_timer[i] < num2))
				{
					_timer[i] -= num2;
					_frameCounter[i] = (num + 1) % _frames[i].Length;
				}
			}
		}
		for (int j = 0; j < 3; j++)
		{
			float[] array2 = _fireFrameDelays[j];
			if (array2.Length != 0)
			{
				int num3 = _fireFrameCounter[j];
				_fireTimer[j] += ((FrameEventArgs)(ref args)).DeltaSeconds;
				float num4 = array2[num3];
				if (!(_fireTimer[j] < num4))
				{
					_fireTimer[j] -= num4;
					_fireFrameCounter[j] = (num3 + 1) % _fireFrames[j].Length;
				}
			}
		}
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (args.MapId == MapId.Nullspace)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		EntityQuery<GasTileOverlayComponent> entityQuery2 = _entManager.GetEntityQuery<GasTileOverlayComponent>();
		(Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem) tuple = (args.WorldBounds, ((OverlayDrawArgs)(ref args)).WorldHandle, _gasCount, _frames, _frameCounter, _fireFrames, _fireFrameCounter, _shader, entityQuery2, entityQuery, _xformSys);
		EntityUid mapOrInvalid = _mapSystem.GetMapOrInvalid((MapId?)args.MapId);
		MapAtmosphereComponent atmos = default(MapAtmosphereComponent);
		if (_entManager.TryGetComponent<MapAtmosphereComponent>(mapOrInvalid, ref atmos))
		{
			DrawMapOverlay(worldHandle, args, mapOrInvalid, atmos);
		}
		if ((int)args.Space != 16)
		{
			return;
		}
		_mapManager.FindGridsIntersecting<(Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem)>(args.MapId, args.WorldAABB, ref tuple, (GridCallback<(Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem)>)delegate(EntityUid uid, MapGridComponent grid, ref (Box2Rotated WorldBounds, DrawingHandleWorld drawHandle, int gasCount, Texture[][] frames, int[] frameCounter, Texture[][] fireFrames, int[] fireFrameCounter, ShaderInstance shader, EntityQuery<GasTileOverlayComponent> overlayQuery, EntityQuery<TransformComponent> xformQuery, SharedTransformSystem xformSys) state)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			GasTileOverlayComponent gasTileOverlayComponent = default(GasTileOverlayComponent);
			TransformComponent val = default(TransformComponent);
			if (!state.overlayQuery.TryGetComponent(uid, ref gasTileOverlayComponent) || !state.xformQuery.TryGetComponent(uid, ref val))
			{
				return true;
			}
			ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = state.xformSys.GetWorldPositionRotationMatrixWithInv(val);
			Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
			Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item4;
			((DrawingHandleBase)state.drawHandle).SetTransform(ref item);
			Box2 val2 = Matrix3Helpers.TransformBox(item2, ref state.WorldBounds);
			Box2 val3 = ((Box2)(ref val2)).Enlarged((float)(int)grid.TileSize);
			Box2i val4 = default(Box2i);
			((Box2i)(ref val4))._002Ector((int)MathF.Floor(val3.Left), (int)MathF.Floor(val3.Bottom), (int)MathF.Ceiling(val3.Right), (int)MathF.Ceiling(val3.Top));
			((DrawingHandleBase)state.drawHandle).UseShader((ShaderInstance)null);
			foreach (GasOverlayChunk value in gasTileOverlayComponent.Chunks.Values)
			{
				GasChunkEnumerator gasChunkEnumerator = new GasChunkEnumerator(value);
				SharedGasTileOverlaySystem.GasOverlayData gas;
				while (gasChunkEnumerator.MoveNext(out gas))
				{
					if (gas.Opacity != null)
					{
						Vector2i val5 = value.Origin + Vector2i.op_Implicit((gasChunkEnumerator.X, gasChunkEnumerator.Y));
						if (((Box2i)(ref val4)).Contains(val5, true))
						{
							for (int i = 0; i < state.gasCount; i++)
							{
								byte b = gas.Opacity[i];
								if (b > 0)
								{
									DrawingHandleWorld item3 = state.drawHandle;
									Texture obj = state.frames[i][state.frameCounter[i]];
									Vector2 vector = Vector2i.op_Implicit(val5);
									Color white = Color.White;
									((DrawingHandleBase)item3).DrawTexture(obj, vector, (Color?)((Color)(ref white)).WithAlpha(b));
								}
							}
						}
					}
				}
			}
			((DrawingHandleBase)state.drawHandle).UseShader(state.shader);
			foreach (GasOverlayChunk value2 in gasTileOverlayComponent.Chunks.Values)
			{
				GasChunkEnumerator gasChunkEnumerator2 = new GasChunkEnumerator(value2);
				SharedGasTileOverlaySystem.GasOverlayData gas2;
				while (gasChunkEnumerator2.MoveNext(out gas2))
				{
					if (gas2.FireState != 0)
					{
						Vector2i val6 = value2.Origin + Vector2i.op_Implicit((gasChunkEnumerator2.X, gasChunkEnumerator2.Y));
						if (((Box2i)(ref val4)).Contains(val6, true))
						{
							int num = gas2.FireState - 1;
							Texture val7 = state.fireFrames[num][state.fireFrameCounter[num]];
							((DrawingHandleBase)state.drawHandle).DrawTexture(val7, Vector2i.op_Implicit(val6), (Color?)null);
						}
					}
				}
			}
			return true;
		}, false, true);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private void DrawMapOverlay(DrawingHandleWorld handle, OverlayDrawArgs args, EntityUid map, MapAtmosphereComponent atmos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _entManager.HasComponent<MapGridComponent>(map);
		if ((flag && (int)args.Space != 16) || (!flag && (int)args.Space != 256))
		{
			return;
		}
		Vector2i val = Vector2Helpers.Floored(args.WorldAABB.BottomLeft);
		Vector2i val2 = Vector2Helpers.Ceiled(args.WorldAABB.TopRight);
		for (int i = val.X; i <= val2.X; i++)
		{
			for (int j = val.Y; j <= val2.Y; j++)
			{
				Vector2 vector = new Vector2(i, j);
				for (int k = 0; k < atmos.OverlayData.Opacity.Length; k++)
				{
					byte b = atmos.OverlayData.Opacity[k];
					if (b > 0)
					{
						Texture obj = _frames[k][_frameCounter[k]];
						Color white = Color.White;
						((DrawingHandleBase)handle).DrawTexture(obj, vector, (Color?)((Color)(ref white)).WithAlpha(b));
					}
				}
			}
		}
	}
}
