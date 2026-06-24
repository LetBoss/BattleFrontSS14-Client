using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Client.Graphics;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;

namespace Content.Client.Light;

public sealed class AmbientOcclusionOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	private static readonly ProtoId<ShaderPrototype> StencilMaskShader = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");

	private static readonly ProtoId<ShaderPrototype> StencilEqualDrawShader = ProtoId<ShaderPrototype>.op_Implicit("StencilEqualDraw");

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _proto;

	private IRenderTexture? _aoTarget;

	private IRenderTexture? _aoBlurBuffer;

	private IRenderTexture? _aoStencilTarget;

	public override OverlaySpace Space => (OverlaySpace)64;

	public AmbientOcclusionOverlay()
	{
		IoCManager.InjectDependencies<AmbientOcclusionOverlay>(this);
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		IClydeViewport viewport = args.Viewport;
		MapId mapId = args.MapId;
		Box2Rotated worldBounds = args.WorldBounds;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Color value = Color.FromHex((ReadOnlySpan<char>)_cfgManager.GetCVar<string>(CCVars.AmbientOcclusionColor), (Color?)null);
		float distance = _cfgManager.GetCVar<float>(CCVars.AmbientOcclusionDistance);
		IRenderTexture renderTarget = viewport.RenderTarget;
		Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)renderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
		Vector2 scale = viewport.RenderScale / (Vector2.One / vector);
		SharedMapSystem maps = _entManager.System<SharedMapSystem>();
		EntityLookupSystem lookups = _entManager.System<EntityLookupSystem>();
		OccluderSystem query = _entManager.System<OccluderSystem>();
		SharedTransformSystem xformSystem = _entManager.System<SharedTransformSystem>();
		TurfSystem turfSystem = _entManager.System<TurfSystem>();
		Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
		IRenderTexture? aoTarget = _aoTarget;
		Vector2i? val = ((aoTarget != null) ? new Vector2i?(aoTarget.Texture.Size) : ((Vector2i?)null));
		Vector2i size = ((IRenderTarget)renderTarget).Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_aoTarget)?.Dispose();
			_aoTarget = _clyde.CreateRenderTarget(((IRenderTarget)renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "ambient-occlusion-target");
		}
		IRenderTexture? aoBlurBuffer = _aoBlurBuffer;
		val = ((aoBlurBuffer != null) ? new Vector2i?(aoBlurBuffer.Texture.Size) : ((Vector2i?)null));
		size = ((IRenderTarget)renderTarget).Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_aoBlurBuffer)?.Dispose();
			_aoBlurBuffer = _clyde.CreateRenderTarget(((IRenderTarget)renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "ambient-occlusion-blur-target");
		}
		IRenderTexture? aoStencilTarget = _aoStencilTarget;
		val = ((aoStencilTarget != null) ? new Vector2i?(aoStencilTarget.Texture.Size) : ((Vector2i?)null));
		size = ((IRenderTarget)renderTarget).Size;
		if (!val.HasValue || val.GetValueOrDefault() != size)
		{
			((IDisposable)_aoStencilTarget)?.Dispose();
			_aoStencilTarget = _clyde.CreateRenderTarget(((IRenderTarget)renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "ambient-occlusion-stencil-target");
		}
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)_aoTarget, (Action)delegate
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(UnshadedShader).Instance());
			Matrix3x2 worldToLocalMatrix = ((IRenderTarget)_aoTarget).GetWorldToLocalMatrix(viewport.Eye, scale);
			foreach (ComponentTreeEntry<OccluderComponent> item in ((ComponentTreeSystem<OccluderTreeComponent, OccluderComponent>)(object)query).QueryAabb(mapId, worldBounds, true))
			{
				Matrix3x2 matrix3x = Matrix3x2.Multiply(xformSystem.GetWorldMatrix(item.Transform), worldToLocalMatrix);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				worldHandle.DrawRect(((Box2)(ref Box2.UnitCentered)).Enlarged(distance / 32f), Color.White, true);
			}
		}, (Color?)Color.Transparent);
		_clyde.BlurRenderTarget(viewport, (IRenderTarget)(object)_aoTarget, (IRenderTarget)(object)_aoBlurBuffer, viewport.Eye, 14f);
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)_aoStencilTarget, (Action)delegate
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(UnshadedShader).Instance());
			List<Entity<MapGridComponent>> list = new List<Entity<MapGridComponent>>();
			_mapManager.FindGridsIntersecting(mapId, worldBounds, ref list, false, true);
			TileRef val2 = default(TileRef);
			foreach (Entity<MapGridComponent> item2 in list)
			{
				Matrix3x2 matrix3x = Matrix3x2.Multiply(xformSystem.GetWorldMatrix(item2.Owner), invMatrix);
				TilesEnumerator tilesEnumerator = maps.GetTilesEnumerator(item2.Owner, item2.Comp, worldBounds, true, (Predicate<TileRef>)null);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				while (((TilesEnumerator)(ref tilesEnumerator)).MoveNext(ref val2))
				{
					if (!turfSystem.IsSpace(val2))
					{
						Box2 localBounds = lookups.GetLocalBounds(val2, item2.Comp.TileSize);
						worldHandle.DrawRect(localBounds, Color.White, true);
					}
				}
			}
		}, (Color?)Color.Transparent);
		((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(StencilMaskShader).Instance());
		worldHandle.DrawTextureRect(_aoStencilTarget.Texture, ref worldBounds, (Color?)null);
		((DrawingHandleBase)worldHandle).UseShader(_proto.Index<ShaderPrototype>(StencilEqualDrawShader).Instance());
		worldHandle.DrawTextureRect(_aoTarget.Texture, ref worldBounds, (Color?)value);
		DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle2).SetTransform(ref identity);
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).UseShader((ShaderInstance)null);
	}
}
