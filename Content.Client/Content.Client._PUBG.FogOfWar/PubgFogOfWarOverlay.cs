using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.FogOfWar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarOverlay : Overlay
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private readonly EntityLookupSystem _lookup;

	private readonly SharedTransformSystem _xform;

	private readonly PubgFogOfWarSystem _system;

	private readonly PubgFovModifierSystem _fovModifiers;

	private ShaderInstance _maskShader;

	private ShaderInstance _blurXShader;

	private ShaderInstance _blurYShader;

	private IRenderTexture? _blurPass;

	private IRenderTexture? _backBuffer;

	private IRenderTexture? _maskTarget;

	private IRenderTexture? _maskBlurTarget;

	private const float MaskBlurRadius = 6f;

	private const float SeenMaskValue = 0.5f;

	private const float ConeSoftness = 0.08f;

	private const float SafeZoneMeters = 0.65f;

	private const float SafeZoneSoftMeters = 0.2f;

	private const int RayCount = 256;

	private const float FacingOffset = 0f;

	private const float BlurScale = 0.7f;

	private static readonly TimeSpan BlurUpdateInterval = TimeSpan.FromSeconds(0.01666666753590107);

	private TimeSpan _nextBlurUpdate;

	private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();

	private readonly List<Box2> _occluderBoxes = new List<Box2>();

	private readonly List<Vector2> _fanVertices = new List<Vector2>();

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public PubgFogOfWarOverlay(PubgFogOfWarSystem system)
	{
		IoCManager.InjectDependencies<PubgFogOfWarOverlay>(this);
		_system = system;
		_xform = _entManager.System<SharedTransformSystem>();
		_lookup = _entManager.System<EntityLookupSystem>();
		_fovModifiers = _entManager.System<PubgFovModifierSystem>();
		InitializeShaders();
		((Overlay)this).ZIndex = 100;
	}

	private void InitializeShaders()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		ShaderInstance maskShader = _maskShader;
		if (maskShader != null)
		{
			maskShader.Dispose();
		}
		ShaderInstance blurXShader = _blurXShader;
		if (blurXShader != null)
		{
			blurXShader.Dispose();
		}
		ShaderInstance blurYShader = _blurYShader;
		if (blurYShader != null)
		{
			blurYShader.Dispose();
		}
		_maskShader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("PubgVisionMask")).InstanceUnique();
		_blurXShader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("BlurryVisionX")).InstanceUnique();
		_blurYShader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("BlurryVisionY")).InstanceUnique();
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		((IDisposable)_blurPass)?.Dispose();
		((IDisposable)_backBuffer)?.Dispose();
		((IDisposable)_maskTarget)?.Dispose();
		((IDisposable)_maskBlurTarget)?.Dispose();
		ShaderInstance maskShader = _maskShader;
		if (maskShader != null)
		{
			maskShader.Dispose();
		}
		ShaderInstance blurXShader = _blurXShader;
		if (blurXShader != null)
		{
			blurXShader.Dispose();
		}
		ShaderInstance blurYShader = _blurYShader;
		if (blurYShader != null)
		{
			blurYShader.Dispose();
		}
		_maskShader = null;
		_blurXShader = null;
		_blurYShader = null;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (_maskShader == null || _blurXShader == null || _blurYShader == null)
		{
			InitializeShaders();
		}
		if (!((ISharedPlayerManager)_player).LocalEntity.HasValue)
		{
			return false;
		}
		if (!_system.Active)
		{
			return false;
		}
		Vector2i size = args.Viewport.Size;
		Vector2i val = (Vector2i)(size * 0.7f);
		if (_backBuffer == null || ((IRenderTarget)_backBuffer).Size != val)
		{
			((IDisposable)_backBuffer)?.Dispose();
			_backBuffer = _clyde.CreateRenderTarget(val, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "pubg-fov-backbuffer");
			((IDisposable)_blurPass)?.Dispose();
			_blurPass = _clyde.CreateRenderTarget(val, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "pubg-fov-blurpass");
		}
		if (_maskTarget == null || ((IRenderTarget)_maskTarget).Size != size)
		{
			((IDisposable)_maskTarget)?.Dispose();
			((IDisposable)_maskBlurTarget)?.Dispose();
			_maskTarget = _clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "pubg-fov-mask");
			_maskBlurTarget = _clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "pubg-fov-mask-blur");
		}
		return true;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? playerEntity = ((ISharedPlayerManager)_player).LocalEntity;
		IEye eye = args.Viewport.Eye;
		if (!playerEntity.HasValue || args.MapId == MapId.Nullspace || base.ScreenTexture == null || _backBuffer == null || _blurPass == null || _maskTarget == null || _maskBlurTarget == null || eye == null)
		{
			return;
		}
		DrawingHandleWorld handle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2Rotated worldBounds = args.WorldBounds;
		Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
		if (_timing.RealTime >= _nextBlurUpdate)
		{
			Box2 blurBounds = new Box2(Vector2.Zero, Vector2i.op_Implicit(((IRenderTarget)_blurPass).Size));
			((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_blurPass, (Action)delegate
			{
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				_blurXShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
				((DrawingHandleBase)handle).UseShader(_blurXShader);
				handle.DrawRect(blurBounds, Color.White, true);
			}, (Color?)Color.Transparent);
			((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_backBuffer, (Action)delegate
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				_blurYShader.SetParameter("SCREEN_TEXTURE", _blurPass.Texture);
				((DrawingHandleBase)handle).UseShader(_blurYShader);
				handle.DrawRect(blurBounds, Color.White, true);
			}, (Color?)Color.Transparent);
			_nextBlurUpdate = _timing.RealTime + BlurUpdateInterval;
		}
		MapId mapId = args.MapId;
		TransformComponent playerXform = default(TransformComponent);
		_entManager.TryGetComponent<TransformComponent>(playerEntity.Value, ref playerXform);
		PubgFogOfWarComponent visionComponent = default(PubgFogOfWarComponent);
		_entManager.TryGetComponent<PubgFogOfWarComponent>(playerEntity.Value, ref visionComponent);
		((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)_maskTarget, (Action)delegate
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			((DrawingHandleBase)handle).SetTransform(ref invMatrix);
			handle.DrawRect(ref worldBounds, new Color(0.5f, 0.5f, 0.5f, 1f), true);
			if (playerXform != null && visionComponent != null)
			{
				Angle viewAngle = (visionComponent.DesiredViewAngle.HasValue ? visionComponent.CurrentAngle : _xform.GetWorldRotation(playerEntity.Value));
				BuildVisibleCone(playerEntity.Value, playerXform, visionComponent, viewAngle, mapId, _fanVertices);
				if (_fanVertices.Count >= 3)
				{
					((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)2, _fanVertices, Color.White);
				}
			}
		}, (Color?)Color.Transparent);
		_clyde.BlurRenderTarget(args.Viewport, (IRenderTarget)(object)_maskTarget, (IRenderTarget)(object)_maskBlurTarget, eye, 6f);
		_maskShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
		_maskShader.SetParameter("BLURRED_TEXTURE", _backBuffer.Texture);
		_maskShader.SetParameter("MASK_TEXTURE", _maskBlurTarget.Texture);
		float num = 0.85f;
		float num2 = 120f;
		Angle val = default(Angle);
		if (visionComponent != null)
		{
			num = visionComponent.ConeOpacity;
			num2 = _fovModifiers.GetEffectiveFov(playerEntity.Value, visionComponent);
			if (visionComponent.DesiredViewAngle.HasValue)
			{
				val = visionComponent.CurrentAngle;
			}
		}
		if (playerXform != null)
		{
			Vector2 worldPosition = _xform.GetWorldPosition(playerXform);
			Vector2 vector = args.Viewport.WorldToLocal(worldPosition);
			vector.Y = (float)args.Viewport.Size.Y - vector.Y;
			Angle val2 = val;
			Angle val3 = default(Angle);
			if (val2 == val3)
			{
				val = _xform.GetWorldRotation(playerEntity.Value);
			}
			val3 = new Angle(val.Theta + 0.0);
			Vector2 vector2 = ((Angle)(ref val3)).ToWorldVec();
			Vector2 vector3 = args.Viewport.WorldToLocal(worldPosition + vector2);
			vector3.Y = (float)args.Viewport.Size.Y - vector3.Y;
			Vector2 value = vector3 - vector;
			value = ((!(value.LengthSquared() > 0.001f)) ? Vector2.UnitY : Vector2.Normalize(value));
			Vector2 vector4 = args.Viewport.WorldToLocal(worldPosition + new Vector2(0.65f, 0f));
			vector4.Y = (float)args.Viewport.Size.Y - vector4.Y;
			float num3 = (vector4 - vector).Length();
			Vector2 vector5 = args.Viewport.WorldToLocal(worldPosition + new Vector2(0.84999996f, 0f));
			vector5.Y = (float)args.Viewport.Size.Y - vector5.Y;
			float num4 = MathF.Max(1f, (vector5 - vector).Length() - num3);
			_maskShader.SetParameter("shadowOpacity", num);
			_maskShader.SetParameter("playerScreen", vector);
			_maskShader.SetParameter("viewDir", value);
			_maskShader.SetParameter("fovCos", MathF.Cos(MathHelper.DegreesToRadians(num2 * 0.5f)));
			_maskShader.SetParameter("coneSoftness", 0.08f);
			_maskShader.SetParameter("safeRadius", num3);
			_maskShader.SetParameter("safeSoftness", num4);
			((DrawingHandleBase)handle).UseShader(_maskShader);
			handle.DrawRect(ref worldBounds, Color.White, true);
			((DrawingHandleBase)handle).UseShader((ShaderInstance)null);
		}
	}

	private void BuildVisibleCone(EntityUid playerUid, TransformComponent playerXform, PubgFogOfWarComponent visionComponent, Angle viewAngle, MapId mapId, List<Vector2> vertices)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 worldPosition = _xform.GetWorldPosition(playerXform);
		float num = MathF.Max(0.1f, visionComponent.Range);
		float effectiveFov = _fovModifiers.GetEffectiveFov(playerUid, visionComponent);
		_occluders.Clear();
		_lookup.GetEntitiesInRange<OccluderComponent>(mapId, worldPosition, num, _occluders, (LookupFlags)5);
		_occluderBoxes.Clear();
		EntityUid val = default(EntityUid);
		OccluderComponent val2 = default(OccluderComponent);
		TransformComponent val5 = default(TransformComponent);
		foreach (Entity<OccluderComponent> occluder in _occluders)
		{
			occluder.Deconstruct(ref val, ref val2);
			EntityUid val3 = val;
			OccluderComponent val4 = val2;
			if (val4.Enabled && _entManager.TryGetComponent<TransformComponent>(val3, ref val5) && !(val5.MapID != mapId))
			{
				Box2 item = Matrix3Helpers.TransformBox(_xform.GetWorldMatrix(val3), ref val4.BoundingBox);
				_occluderBoxes.Add(item);
			}
		}
		vertices.Clear();
		vertices.Add(worldPosition);
		float num2 = MathHelper.DegreesToRadians(effectiveFov * 0.5f);
		float num3 = 0f - num2;
		float num4 = num2 * 2f / 256f;
		for (int i = 0; i <= 256; i++)
		{
			float num5 = num3 + num4 * (float)i;
			Angle val6 = new Angle(viewAngle.Theta + (double)num5);
			Vector2 vector = ((Angle)(ref val6)).ToWorldVec();
			float num6 = num;
			for (int j = 0; j < _occluderBoxes.Count; j++)
			{
				if (RayAabb(worldPosition, vector, _occluderBoxes[j], out var distance) && distance >= 0f && distance < num6)
				{
					num6 = distance;
				}
			}
			vertices.Add(worldPosition + vector * num6);
		}
	}

	private static bool RayAabb(Vector2 origin, Vector2 dir, Box2 box, out float distance)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		distance = 0f;
		float tmin = 0f;
		float tmax = float.PositiveInfinity;
		if (!RaySlab(origin.X, dir.X, box.Left, box.Right, ref tmin, ref tmax))
		{
			return false;
		}
		if (!RaySlab(origin.Y, dir.Y, box.Bottom, box.Top, ref tmin, ref tmax))
		{
			return false;
		}
		if (tmax < 0f)
		{
			return false;
		}
		distance = ((tmin >= 0f) ? tmin : tmax);
		return distance >= 0f;
	}

	private static bool RaySlab(float origin, float dir, float min, float max, ref float tmin, ref float tmax)
	{
		if (MathF.Abs(dir) < 0.0001f)
		{
			if (origin < min || origin > max)
			{
				return false;
			}
			return true;
		}
		float num = 1f / dir;
		float num2 = (min - origin) * num;
		float num3 = (max - origin) * num;
		if (num2 > num3)
		{
			float num4 = num2;
			num2 = num3;
			num3 = num4;
		}
		if (num2 > tmin)
		{
			tmin = num2;
		}
		if (num3 < tmax)
		{
			tmax = num3;
		}
		return tmax >= tmin;
	}
}
