using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Xenonids.Eye;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Eye;

public sealed class QueenEyeOverlay : Overlay
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<Vector2i> _visibleTiles = new HashSet<Vector2i>();

	private IRenderTexture? _staticTexture;

	private IRenderTexture? _stencilTexture;

	private readonly float _updateRate = 1f / 30f;

	private float _accumulator;

	public override OverlaySpace Space => (OverlaySpace)4;

	public QueenEyeOverlay()
	{
		IoCManager.InjectDependencies<QueenEyeOverlay>(this);
		((Overlay)this).ZIndex = 2;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		IRenderTexture? stencilTexture = _stencilTexture;
		Vector2i? val = ((stencilTexture != null) ? new Vector2i?(stencilTexture.Texture.Size) : ((Vector2i?)null));
		Vector2i size = args.Viewport.Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_staticTexture)?.Dispose();
			((IDisposable)_stencilTexture)?.Dispose();
			_stencilTexture = _clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "queen-eye-stencil");
			_staticTexture = _clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "queen-eye-static");
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2Rotated worldBounds = args.WorldBounds;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		TransformComponent val2 = default(TransformComponent);
		_entities.TryGetComponent<TransformComponent>(localEntity, ref val2);
		EntityUid val3 = (EntityUid)(((_003F?)((val2 != null) ? val2.GridUid : ((EntityUid?)null))) ?? EntityUid.Invalid);
		MapGridComponent grid = default(MapGridComponent);
		_entities.TryGetComponent<MapGridComponent>(val3, ref grid);
		BroadphaseComponent val4 = default(BroadphaseComponent);
		_entities.TryGetComponent<BroadphaseComponent>(val3, ref val4);
		Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
		_accumulator -= (float)_timing.FrameTime.TotalSeconds;
		if (grid != null && val4 != null)
		{
			EntityLookupSystem lookups = _entities.System<EntityLookupSystem>();
			SharedTransformSystem obj = _entities.System<SharedTransformSystem>();
			if (_accumulator <= 0f)
			{
				_accumulator = MathF.Max(0f, _accumulator + _updateRate);
				_visibleTiles.Clear();
				_entities.System<QueenEyeSystem>().GetView(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((val3, val4, grid)), worldBounds, _visibleTiles);
			}
			Matrix3x2 worldMatrix = obj.GetWorldMatrix(val3);
			Matrix3x2 matty = Matrix3x2.Multiply(worldMatrix, invMatrix);
			((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_stencilTexture, (Action)delegate
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				((DrawingHandleBase)worldHandle).SetTransform(ref matty);
				foreach (Vector2i visibleTile in _visibleTiles)
				{
					Box2 localBounds = lookups.GetLocalBounds(visibleTile, grid.TileSize);
					worldHandle.DrawRect(localBounds, Color.White, true);
				}
			}, (Color?)Color.Transparent);
			((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_staticTexture, (Action)delegate
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				((DrawingHandleBase)worldHandle).SetTransform(ref invMatrix);
				worldHandle.DrawRect(ref worldBounds, Color.Black, true);
			}, (Color?)Color.Black);
		}
		else
		{
			((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_stencilTexture, (Action)delegate
			{
			}, (Color?)Color.Transparent);
			((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)_staticTexture, (Action)delegate
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				DrawingHandleWorld obj3 = worldHandle;
				Matrix3x2 identity2 = Matrix3x2.Identity;
				((DrawingHandleBase)obj3).SetTransform(ref identity2);
				worldHandle.DrawRect(ref worldBounds, Color.Black, true);
			}, (Color?)Color.Black);
		}
		((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("StencilMask")).Instance());
		worldHandle.DrawTextureRect(_stencilTexture.Texture, ref worldBounds, (Color?)null);
		((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("StencilDraw")).Instance());
		worldHandle.DrawTextureRect(_staticTexture.Texture, ref worldBounds, (Color?)null);
		DrawingHandleWorld obj2 = worldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj2).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
