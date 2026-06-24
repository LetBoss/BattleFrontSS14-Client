using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
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

public sealed class CivCommanderVisionOverlay : Overlay
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPrototypeManager _prototype;

	private readonly CivCommanderVisionSystem _system;

	private readonly SharedTransformSystem _xform;

	private ShaderInstance? _maskShader;

	private IRenderTexture? _maskTarget;

	private readonly List<(Box2 Bounds, Color Color)> _tileBatch = new List<(Box2, Color)>(4096);

	private const float SeenMaskValue = 0.45f;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public CivCommanderVisionOverlay(CivCommanderVisionSystem system)
	{
		IoCManager.InjectDependencies<CivCommanderVisionOverlay>(this);
		_system = system;
		_xform = _entities.System<SharedTransformSystem>();
		InitializeShaders();
		((Overlay)this).ZIndex = 240;
	}

	private void InitializeShaders()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ShaderInstance? maskShader = _maskShader;
		if (maskShader != null)
		{
			maskShader.Dispose();
		}
		_maskShader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("CivCommanderVisionMask")).InstanceUnique();
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		((IDisposable)_maskTarget)?.Dispose();
		ShaderInstance? maskShader = _maskShader;
		if (maskShader != null)
		{
			maskShader.Dispose();
		}
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!_system.Active || base.ScreenTexture == null)
		{
			return false;
		}
		if (_maskShader == null)
		{
			InitializeShaders();
		}
		Vector2i size = args.Viewport.Size;
		if (_maskTarget == null || ((IRenderTarget)_maskTarget).Size != size)
		{
			((IDisposable)_maskTarget)?.Dispose();
			_maskTarget = _clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "civ-commander-vision-mask");
		}
		if (_maskTarget != null)
		{
			return _maskShader != null;
		}
		return false;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!_system.Active || args.MapId == MapId.Nullspace || base.ScreenTexture == null || _maskTarget == null || _maskShader == null || args.Viewport.Eye == null)
		{
			return;
		}
		DrawingHandleWorld handle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2Rotated worldBounds = args.WorldBounds;
		Matrix3x2 inverseMatrix = args.Viewport.GetWorldToLocalMatrix();
		MapId mapId = args.MapId;
		((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_maskTarget, (Action)delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)handle).SetTransform(ref inverseMatrix);
			handle.DrawRect(ref worldBounds, Color.Black, true);
			EntityQueryEnumerator<MapGridComponent, TransformComponent> val = _entities.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
			EntityUid val2 = default(EntityUid);
			MapGridComponent grid = default(MapGridComponent);
			TransformComponent val3 = default(TransformComponent);
			while (val.MoveNext(ref val2, ref grid, ref val3))
			{
				if (!(val3.MapID != mapId) && _system.GridChunks.TryGetValue(val2, out Dictionary<Vector2i, byte[]> value))
				{
					Matrix3x2 matrix3x = Matrix3x2.Multiply(_xform.GetWorldMatrix(val2), inverseMatrix);
					((DrawingHandleBase)handle).SetTransform(ref matrix3x);
					_tileBatch.Clear();
					foreach (var (chunkIndex, states) in value)
					{
						CollectVisibleChunk(grid, chunkIndex, states);
					}
					foreach (var (val5, val6) in _tileBatch)
					{
						handle.DrawRect(val5, val6, true);
					}
				}
			}
		}, (Color?)Color.Transparent);
		_maskShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
		_maskShader.SetParameter("MASK_TEXTURE", _maskTarget.Texture);
		_maskShader.SetParameter("shadowOpacity", 0.85f);
		((DrawingHandleBase)handle).SetTransform(ref inverseMatrix);
		((DrawingHandleBase)handle).UseShader(_maskShader);
		handle.DrawRect(ref worldBounds, Color.White, true);
		((DrawingHandleBase)handle).UseShader((ShaderInstance)null);
		DrawingHandleWorld obj = handle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj).SetTransform(ref identity);
	}

	private void CollectVisibleChunk(MapGridComponent grid, Vector2i chunkIndex, byte[] states)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (states.Length != 256)
		{
			return;
		}
		int num = 16;
		ushort tileSize = grid.TileSize;
		int originX = chunkIndex.X * num;
		int originY = chunkIndex.Y * num;
		int num2 = -1;
		int num3 = -1;
		for (int i = 0; i < num; i++)
		{
			num2 = -1;
			num3 = -1;
			for (int j = 0; j <= num; j++)
			{
				CivCommanderVisionTileState civCommanderVisionTileState;
				if (j < num)
				{
					int num4 = i * num + j;
					civCommanderVisionTileState = (CivCommanderVisionTileState)states[num4];
				}
				else
				{
					civCommanderVisionTileState = CivCommanderVisionTileState.Unseen;
				}
				switch (civCommanderVisionTileState)
				{
				case CivCommanderVisionTileState.Visible:
					if (num3 >= 0)
					{
						FlushRow(originX, originY, i, num3, j - 1, (int)tileSize, visible: false);
						num3 = -1;
					}
					if (num2 < 0)
					{
						num2 = j;
					}
					continue;
				case CivCommanderVisionTileState.Seen:
					if (num2 >= 0)
					{
						FlushRow(originX, originY, i, num2, j - 1, (int)tileSize, visible: true);
						num2 = -1;
					}
					if (num3 < 0)
					{
						num3 = j;
					}
					continue;
				}
				if (num2 >= 0)
				{
					FlushRow(originX, originY, i, num2, j - 1, (int)tileSize, visible: true);
					num2 = -1;
				}
				if (num3 >= 0)
				{
					FlushRow(originX, originY, i, num3, j - 1, (int)tileSize, visible: false);
					num3 = -1;
				}
			}
		}
	}

	private void FlushRow(int originX, int originY, int y, int startX, int endX, float tileSize, bool visible)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)(originX + startX) * tileSize;
		float num2 = (float)(originX + endX + 1) * tileSize;
		float num3 = (float)(originY + y) * tileSize;
		float num4 = num3 + tileSize;
		Color item = (Color)(visible ? Color.White : new Color(0.45f, 0.45f, 0.45f, 1f));
		_tileBatch.Add((new Box2(num, num3, num2, num4), item));
	}
}
