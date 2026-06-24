using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Light.Components;
using Robust.Client.Graphics;
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

public sealed class SunShadowOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> MixShader = ProtoId<ShaderPrototype>.op_Implicit("Mix");

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	private readonly EntityLookupSystem _lookup;

	private readonly SharedTransformSystem _xformSys;

	private readonly HashSet<Entity<SunShadowCastComponent>> _shadows = new HashSet<Entity<SunShadowCastComponent>>();

	private IRenderTexture? _blurTarget;

	private IRenderTexture? _target;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override OverlaySpace Space => (OverlaySpace)512;

	public SunShadowOverlay()
	{
		IoCManager.InjectDependencies<SunShadowOverlay>(this);
		_xformSys = _entManager.System<SharedTransformSystem>();
		_lookup = _entManager.System<EntityLookupSystem>();
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		IClydeViewport viewport = args.Viewport;
		IEye eye = viewport.Eye;
		if (eye == null)
		{
			return;
		}
		_grids.Clear();
		_mapManager.FindGridsIntersecting(args.MapId, ((Box2Rotated)(ref args.WorldBounds)).Enlarged(5f), ref _grids, false, true);
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		MapId mapId = args.MapId;
		Box2Rotated worldBounds = args.WorldBounds;
		Vector2i size = ((IRenderTarget)viewport.LightRenderTarget).Size;
		IRenderTexture? target = _target;
		if (target == null || ((IRenderTarget)target).Size != size)
		{
			_target = _clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "sun-shadow-target");
			IRenderTexture? blurTarget = _blurTarget;
			if (blurTarget == null || ((IRenderTarget)blurTarget).Size != size)
			{
				_blurTarget = _clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "sun-shadow-blur");
			}
		}
		Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
		Vector2 scale = viewport.RenderScale / (Vector2.One / vector);
		SunShadowComponent sunShadowComponent = default(SunShadowComponent);
		foreach (Entity<MapGridComponent> grid in _grids)
		{
			if (!_entManager.TryGetComponent<SunShadowComponent>(grid.Owner, ref sunShadowComponent))
			{
				continue;
			}
			Vector2 direction = sunShadowComponent.Direction;
			float alpha = Math.Clamp(sunShadowComponent.Alpha, 0f, 1f);
			if (direction.Equals(Vector2.Zero) || alpha == 0f)
			{
				continue;
			}
			Box2Rotated expandedBounds = ((Box2Rotated)(ref worldBounds)).Enlarged(direction.Length() + 0.01f);
			_shadows.Clear();
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)_target, (Action)delegate
			{
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				Matrix3x2 worldToLocalMatrix = ((IRenderTarget)_target).GetWorldToLocalMatrix(eye, scale);
				Vector2[] array = new Vector2[16];
				_lookup.GetEntitiesIntersecting<SunShadowCastComponent>(mapId, expandedBounds, _shadows, (LookupFlags)110);
				foreach (Entity<SunShadowCastComponent> shadow in _shadows)
				{
					TransformComponent component = _entManager.GetComponent<TransformComponent>(shadow.Owner);
					ValueTuple<Vector2, Angle> worldPositionRotation = _xformSys.GetWorldPositionRotation(component);
					Vector2 item = worldPositionRotation.Item1;
					Angle item2 = worldPositionRotation.Item2;
					Matrix3x2 matrix3x = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(item), worldToLocalMatrix);
					int num = shadow.Comp.Points.Length;
					Array.Copy(shadow.Comp.Points, array, num);
					for (int i = 0; i < num; i++)
					{
						array[i] = ((Angle)(ref item2)).RotateVec(ref array[i]);
						array[num + i] = array[i] + direction;
					}
					Span<Vector2> span = PhysicsHull.ComputePoints((ReadOnlySpan<Vector2>)array, num * 2);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					((DrawingHandleBase)worldHandle).DrawPrimitives((DrawPrimitiveTopology)2, (ReadOnlySpan<Vector2>)span, Color.White);
				}
			}, (Color?)Color.Transparent);
			_clyde.BlurRenderTarget(viewport, (IRenderTarget)(object)_target, (IRenderTarget)(object)_blurTarget, eye, 1f);
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)viewport.LightRenderTarget, (Action)delegate
			{
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				Matrix3x2 worldToLocalMatrix = ((IRenderTarget)viewport.LightRenderTarget).GetWorldToLocalMatrix(eye, scale);
				((DrawingHandleBase)worldHandle).SetTransform(ref worldToLocalMatrix);
				ShaderInstance val = _protoManager.Index<ShaderPrototype>(MixShader).Instance();
				((DrawingHandleBase)worldHandle).UseShader(val);
				DrawingHandleWorld obj = worldHandle;
				Texture texture = _target.Texture;
				ref Box2Rotated reference = ref worldBounds;
				Color black = Color.Black;
				obj.DrawTextureRect(texture, ref reference, (Color?)((Color)(ref black)).WithAlpha(alpha));
			}, (Color?)null);
		}
	}
}
