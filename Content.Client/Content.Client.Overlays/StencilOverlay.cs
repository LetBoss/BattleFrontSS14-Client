using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Parallax;
using Content.Client.Weather;
using Content.Shared.Light.Components;
using Content.Shared.Salvage;
using Content.Shared.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Overlays;

public sealed class StencilOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("WorldGradientCircle");

	private static readonly ProtoId<ShaderPrototype> StencilMask = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");

	private static readonly ProtoId<ShaderPrototype> StencilDraw = ProtoId<ShaderPrototype>.op_Implicit("StencilDraw");

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	private readonly ParallaxSystem _parallax;

	private readonly SharedTransformSystem _transform;

	private readonly SharedMapSystem _map;

	private readonly SpriteSystem _sprite;

	private readonly WeatherSystem _weather;

	private IRenderTexture? _blep;

	private readonly ShaderInstance _shader;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override OverlaySpace Space => (OverlaySpace)8;

	public StencilOverlay(ParallaxSystem parallax, SharedTransformSystem transform, SharedMapSystem map, SpriteSystem sprite, WeatherSystem weather)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		((Overlay)this).ZIndex = 1;
		_parallax = parallax;
		_transform = transform;
		_map = map;
		_sprite = sprite;
		_weather = weather;
		IoCManager.InjectDependencies<StencilOverlay>(this);
		_shader = _protoManager.Index<ShaderPrototype>(CircleShader).InstanceUnique();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid mapOrInvalid = _map.GetMapOrInvalid((MapId?)args.MapId);
		Matrix3x2 worldToLocalMatrix = args.Viewport.GetWorldToLocalMatrix();
		IRenderTexture? blep = _blep;
		Vector2i? val = ((blep != null) ? new Vector2i?(blep.Texture.Size) : ((Vector2i?)null));
		Vector2i size = args.Viewport.Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_blep)?.Dispose();
			_blep = _clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "weather-stencil");
		}
		WeatherComponent weatherComponent = default(WeatherComponent);
		if (_entManager.TryGetComponent<WeatherComponent>(mapOrInvalid, ref weatherComponent))
		{
			WeatherPrototype weatherProto = default(WeatherPrototype);
			foreach (var (val3, component) in weatherComponent.Weather)
			{
				if (_protoManager.TryIndex<WeatherPrototype>(val3, ref weatherProto))
				{
					float percent = _weather.GetPercent(component, mapOrInvalid);
					DrawWeather(in args, weatherProto, percent, worldToLocalMatrix);
				}
			}
		}
		RestrictedRangeComponent rangeComp = default(RestrictedRangeComponent);
		if (_entManager.TryGetComponent<RestrictedRangeComponent>(mapOrInvalid, ref rangeComp))
		{
			DrawRestrictedRange(in args, rangeComp, worldToLocalMatrix);
		}
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).UseShader((ShaderInstance)null);
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private void DrawRestrictedRange(in OverlayDrawArgs args, RestrictedRangeComponent rangeComp, Matrix3x2 invMatrix)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		float x = args.Viewport.RenderScale.X;
		IEye eye = args.Viewport.Eye;
		Vector2 obj = ((eye != null) ? eye.Zoom : Vector2.One);
		float x2 = obj.X;
		float num = MathF.Min(10f, rangeComp.Range);
		Vector2 vector = Vector2.Transform(rangeComp.Origin, invMatrix);
		int y = args.Viewport.Size.Y;
		float num2 = rangeComp.Range * x / x2 * 32f;
		float num3 = num * x / x2 * 32f;
		float num4 = num2 - num3;
		_shader.SetParameter("position", new Vector2(vector.X, (float)y - vector.Y));
		_shader.SetParameter("maxRange", num2);
		_shader.SetParameter("minRange", num4);
		_shader.SetParameter("bufferRange", num3);
		_shader.SetParameter("gradient", 0.8f);
		Box2 worldAABB = args.WorldAABB;
		Box2Rotated worldBounds = args.WorldBounds;
		IEye eye2 = args.Viewport.Eye;
		Vector2 position = ((eye2 != null) ? eye2.Position.Position : Vector2.Zero);
		Box2 localAABB = Matrix3Helpers.TransformBox(invMatrix, ref worldAABB);
		((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_blep, (Action)delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawRect(localAABB, Color.White, true);
		}, (Color?)Color.Transparent);
		DrawingHandleWorld obj2 = worldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj2).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader(_protoManager.Index<ShaderPrototype>(StencilMask).Instance());
		worldHandle.DrawTextureRect(_blep.Texture, ref worldBounds, (Color?)null);
		TimeSpan realTime = _timing.RealTime;
		Texture frame = _sprite.GetFrame((SpriteSpecifier)new Texture(new ResPath("/Textures/Parallaxes/noise.png")), realTime, true);
		((DrawingHandleBase)worldHandle).UseShader(_protoManager.Index<ShaderPrototype>(StencilDraw).Instance());
		_parallax.DrawParallax(worldHandle, worldAABB, frame, realTime, position, new Vector2(0.5f, 0f));
	}

	private void DrawWeather(in OverlayDrawArgs args, WeatherPrototype weatherProto, float alpha, Matrix3x2 invMatrix)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		MapId mapId = args.MapId;
		Box2 worldAABB = args.WorldAABB;
		Box2Rotated worldBounds = args.WorldBounds;
		IEye eye = args.Viewport.Eye;
		Vector2 position = ((eye != null) ? eye.Position.Position : Vector2.Zero);
		((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_blep, (Action)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
			_grids.Clear();
			_mapManager.FindGridsIntersecting(mapId, worldAABB, ref _grids, false, true);
			RoofComponent roofComp = default(RoofComponent);
			Box2 val2 = default(Box2);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				Matrix3x2 matrix3x = Matrix3x2.Multiply(_transform.GetWorldMatrix(Entity<MapGridComponent>.op_Implicit(grid), entityQuery), invMatrix);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				_entManager.TryGetComponent<RoofComponent>(grid.Owner, ref roofComp);
				foreach (TileRef item in _map.GetTilesIntersecting(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), worldAABB, true, (Predicate<TileRef>)null))
				{
					if (!_weather.CanWeatherAffect(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), item, roofComp))
					{
						((Box2)(ref val2))._002Ector(Vector2i.op_Implicit(item.GridIndices * (int)grid.Comp.TileSize), Vector2i.op_Implicit((item.GridIndices + Vector2i.One) * (int)grid.Comp.TileSize));
						worldHandle.DrawRect(val2, Color.White, true);
					}
				}
			}
		}, (Color?)Color.Transparent);
		DrawingHandleWorld obj = worldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader(_protoManager.Index<ShaderPrototype>(StencilMask).Instance());
		worldHandle.DrawTextureRect(_blep.Texture, ref worldBounds, (Color?)null);
		TimeSpan realTime = _timing.RealTime;
		Texture frame = _sprite.GetFrame(weatherProto.Sprite, realTime, true);
		((DrawingHandleBase)worldHandle).UseShader(_protoManager.Index<ShaderPrototype>(StencilDraw).Instance());
		ParallaxSystem parallax = _parallax;
		DrawingHandleWorld worldHandle2 = worldHandle;
		Box2 worldAABB2 = worldAABB;
		Vector2 zero = Vector2.Zero;
		Color val = (Color)(((_003F?)weatherProto.Color) ?? Color.White);
		parallax.DrawParallax(worldHandle2, worldAABB2, frame, realTime, position, zero, 1f, 0f, ((Color)(ref val)).WithAlpha(alpha));
		DrawingHandleWorld obj2 = worldHandle;
		identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj2).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
