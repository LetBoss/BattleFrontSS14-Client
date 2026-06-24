using System;
using System.Numerics;
using Content.Shared.Singularity.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Singularity;

public sealed class SingularityOverlay : Overlay, IEntityEventSubscriber
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Singularity");

	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private SharedTransformSystem? _xformSystem;

	public const int MaxCount = 5;

	private const float MaxDistance = 20f;

	private readonly ShaderInstance _shader;

	private readonly Vector2[] _positions = new Vector2[5];

	private readonly float[] _intensities = new float[5];

	private readonly float[] _falloffPowers = new float[5];

	private int _count;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public SingularityOverlay()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SingularityOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(Shader).Instance().Duplicate();
		_shader.SetParameter("maxDistance", 640f);
		((IBroadcastEventBus)_entMan.EventBus).SubscribeEvent<PixelToMapEvent>((EventSource)1, (IEntityEventSubscriber)(object)this, (EntityEventRefHandler<PixelToMapEvent>)OnProjectFromScreenToMap);
		((Overlay)this).ZIndex = 101;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null)
		{
			return false;
		}
		if (_xformSystem == null && !_entMan.TrySystem<SharedTransformSystem>(ref _xformSystem))
		{
			return false;
		}
		_count = 0;
		EntityQueryEnumerator<SingularityDistortionComponent, TransformComponent> val = _entMan.EntityQueryEnumerator<SingularityDistortionComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		SingularityDistortionComponent singularityDistortionComponent = default(SingularityDistortionComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref singularityDistortionComponent, ref val3))
		{
			if (val3.MapID != args.MapId)
			{
				continue;
			}
			Vector2 worldPosition = _xformSystem.GetWorldPosition(val2);
			if (!((worldPosition - ((Box2)(ref args.WorldAABB)).ClosestPoint(ref worldPosition)).LengthSquared() > 400f))
			{
				Vector2 vector = args.Viewport.WorldToLocal(worldPosition);
				vector.Y = (float)args.Viewport.Size.Y - vector.Y;
				_positions[_count] = vector;
				_intensities[_count] = singularityDistortionComponent.Intensity;
				_falloffPowers[_count] = singularityDistortionComponent.FalloffPower;
				_count++;
				if (_count == 5)
				{
					break;
				}
			}
		}
		return _count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null && args.Viewport.Eye != null)
		{
			ShaderInstance shader = _shader;
			if (shader != null)
			{
				shader.SetParameter("renderScale", args.Viewport.RenderScale * args.Viewport.Eye.Scale);
			}
			ShaderInstance shader2 = _shader;
			if (shader2 != null)
			{
				shader2.SetParameter("count", _count);
			}
			ShaderInstance shader3 = _shader;
			if (shader3 != null)
			{
				shader3.SetParameter("position", _positions);
			}
			ShaderInstance shader4 = _shader;
			if (shader4 != null)
			{
				shader4.SetParameter("intensity", _intensities);
			}
			ShaderInstance shader5 = _shader;
			if (shader5 != null)
			{
				shader5.SetParameter("falloffPower", _falloffPowers);
			}
			ShaderInstance shader6 = _shader;
			if (shader6 != null)
			{
				shader6.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			}
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawRect(args.WorldAABB, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}

	private void OnProjectFromScreenToMap(ref PixelToMapEvent args)
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null)
		{
			return;
		}
		float num = 640f;
		Vector2 visiblePosition = args.VisiblePosition;
		for (int i = 0; i < 5 && i < _count; i++)
		{
			Vector2 vector = _positions[i];
			vector.Y = (float)args.Viewport.Size.Y - vector.Y;
			Vector2 vector2 = args.VisiblePosition - vector;
			float num2 = (vector2 / (args.Viewport.RenderScale * args.Viewport.Eye.Scale)).Length();
			float num3 = _intensities[i] / MathF.Pow(num2, _falloffPowers[i]);
			num3 = ((!(num2 >= num)) ? (num3 * (1f - MathF.Pow(num2 / num, 4f))) : 0f);
			if ((double)num3 > 0.8)
			{
				num3 = MathF.Pow(num3, 0.3f);
			}
			visiblePosition -= vector2 * num3;
		}
		visiblePosition.X -= MathF.Floor(visiblePosition.X / (float)(args.Viewport.Size.X * 2)) * (float)args.Viewport.Size.X * 2f;
		visiblePosition.Y -= MathF.Floor(visiblePosition.Y / (float)(args.Viewport.Size.Y * 2)) * (float)args.Viewport.Size.Y * 2f;
		visiblePosition.X = ((visiblePosition.X >= (float)args.Viewport.Size.X) ? ((float)(args.Viewport.Size.X * 2) - visiblePosition.X) : visiblePosition.X);
		visiblePosition.Y = ((visiblePosition.Y >= (float)args.Viewport.Size.Y) ? ((float)(args.Viewport.Size.Y * 2) - visiblePosition.Y) : visiblePosition.Y);
		args.VisiblePosition = visiblePosition;
	}
}
