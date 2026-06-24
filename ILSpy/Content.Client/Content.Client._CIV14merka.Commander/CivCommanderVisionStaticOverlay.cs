using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionStaticOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> CommanderStaticShader = ProtoId<ShaderPrototype>.op_Implicit("CivCommanderStatic");

	private static readonly ProtoId<ShaderPrototype> StencilMaskShader = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");

	private static readonly ProtoId<ShaderPrototype> StencilDrawShader = ProtoId<ShaderPrototype>.op_Implicit("StencilDraw");

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPrototypeManager _prototype;

	private readonly CivCommanderVisionSystem _system;

	private readonly EntityLookupSystem _lookup;

	private readonly SharedTransformSystem _xform;

	private IRenderTexture? _staticTexture;

	private IRenderTexture? _stencilTexture;

	public override OverlaySpace Space => (OverlaySpace)4;

	public CivCommanderVisionStaticOverlay(CivCommanderVisionSystem system)
	{
		IoCManager.InjectDependencies<CivCommanderVisionStaticOverlay>(this);
		_system = system;
		_lookup = _entities.System<EntityLookupSystem>();
		_xform = _entities.System<SharedTransformSystem>();
		((Overlay)this).ZIndex = 241;
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		((IDisposable)_staticTexture)?.Dispose();
		((IDisposable)_stencilTexture)?.Dispose();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (!_system.Active || args.MapId == MapId.Nullspace)
		{
			return;
		}
		IRenderTexture? stencilTexture = _stencilTexture;
		Vector2i? val = ((stencilTexture != null) ? new Vector2i?(stencilTexture.Texture.Size) : ((Vector2i?)null));
		Vector2i size = args.Viewport.Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_staticTexture)?.Dispose();
			((IDisposable)_stencilTexture)?.Dispose();
			_stencilTexture = _clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "civ-commander-static-stencil");
			_staticTexture = _clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "civ-commander-static");
		}
		if (_stencilTexture == null || _staticTexture == null)
		{
			return;
		}
		DrawingHandleWorld handle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2Rotated worldBounds = args.WorldBounds;
		Matrix3x2 inverseMatrix = args.Viewport.GetWorldToLocalMatrix();
		MapId mapId = args.MapId;
		((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_stencilTexture, (Action)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			EntityQueryEnumerator<MapGridComponent, TransformComponent> val2 = _entities.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
			EntityUid val3 = default(EntityUid);
			MapGridComponent grid = default(MapGridComponent);
			TransformComponent val4 = default(TransformComponent);
			while (val2.MoveNext(ref val3, ref grid, ref val4))
			{
				if (!(val4.MapID != mapId) && _system.GridChunks.TryGetValue(val3, out Dictionary<Vector2i, byte[]> value))
				{
					Matrix3x2 matrix3x = Matrix3x2.Multiply(_xform.GetWorldMatrix(val3), inverseMatrix);
					((DrawingHandleBase)handle).SetTransform(ref matrix3x);
					foreach (var (chunkIndex, states) in value)
					{
						DrawKnownChunk(handle, grid, chunkIndex, states);
					}
				}
			}
		}, (Color?)Color.Transparent);
		((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_staticTexture, (Action)delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)handle).SetTransform(ref inverseMatrix);
			ShaderInstance val2 = _prototype.Index<ShaderPrototype>(CommanderStaticShader).Instance();
			((DrawingHandleBase)handle).UseShader(val2);
			handle.DrawRect(ref worldBounds, Color.White, true);
			((DrawingHandleBase)handle).UseShader((ShaderInstance)null);
		}, (Color?)Color.Black);
		((DrawingHandleBase)handle).UseShader(_prototype.Index<ShaderPrototype>(StencilMaskShader).Instance());
		handle.DrawTextureRect(_stencilTexture.Texture, ref worldBounds, (Color?)null);
		((DrawingHandleBase)handle).UseShader(_prototype.Index<ShaderPrototype>(StencilDrawShader).Instance());
		handle.DrawTextureRect(_staticTexture.Texture, ref worldBounds, (Color?)null);
		DrawingHandleWorld obj = handle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj).SetTransform(ref identity);
		((DrawingHandleBase)handle).UseShader((ShaderInstance)null);
	}

	private void DrawKnownChunk(DrawingHandleWorld handle, MapGridComponent grid, Vector2i chunkIndex, byte[] states)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (states.Length != 256)
		{
			return;
		}
		int num = chunkIndex.X * 16;
		int num2 = chunkIndex.Y * 16;
		Vector2i val = default(Vector2i);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num3 = i * 16 + j;
				if (states[num3] != 0)
				{
					((Vector2i)(ref val))._002Ector(num + j, num2 + i);
					Box2 localBounds = _lookup.GetLocalBounds(val, grid.TileSize);
					handle.DrawRect(localBounds, Color.White, true);
				}
			}
		}
	}
}
