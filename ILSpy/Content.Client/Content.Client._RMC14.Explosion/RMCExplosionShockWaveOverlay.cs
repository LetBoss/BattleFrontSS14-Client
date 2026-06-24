using System.Numerics;
using Content.Shared._RMC14.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Explosion;

public sealed class RMCExplosionShockWaveOverlay : Overlay, IEntityEventSubscriber
{
	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private SharedTransformSystem? _xformSystem;

	private readonly ShaderInstance _shader;

	public const int MaxCount = 10;

	private readonly Vector2[] _positions = new Vector2[10];

	private readonly float[] _falloffPower = new float[10];

	private readonly float[] _sharpness = new float[10];

	private readonly float[] _width = new float[10];

	private int _count;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public RMCExplosionShockWaveOverlay()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RMCExplosionShockWaveOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCShockWave")).Instance().Duplicate();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null || (_xformSystem == null && !_entMan.TrySystem<SharedTransformSystem>(ref _xformSystem)))
		{
			return false;
		}
		EntityQueryEnumerator<RMCExplosionShockWaveComponent, TransformComponent> val = _entMan.EntityQueryEnumerator<RMCExplosionShockWaveComponent, TransformComponent>();
		_count = 0;
		EntityUid val2 = default(EntityUid);
		RMCExplosionShockWaveComponent rMCExplosionShockWaveComponent = default(RMCExplosionShockWaveComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref rMCExplosionShockWaveComponent, ref val3))
		{
			if (!(val3.MapID != args.MapId))
			{
				Vector2 worldPosition = _xformSystem.GetWorldPosition(val2);
				Vector2 vector = args.Viewport.WorldToLocal(worldPosition);
				vector.Y = 1f - vector.Y / (float)args.Viewport.Size.Y;
				vector.X /= args.Viewport.Size.X;
				_positions[_count] = vector;
				_falloffPower[_count] = rMCExplosionShockWaveComponent.FalloffPower;
				_sharpness[_count] = rMCExplosionShockWaveComponent.Sharpness;
				_width[_count] = rMCExplosionShockWaveComponent.Width;
				_count++;
				if (_count == 10)
				{
					break;
				}
			}
		}
		return _count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
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
				shader4.SetParameter("falloffPower", _falloffPower);
			}
			ShaderInstance shader5 = _shader;
			if (shader5 != null)
			{
				shader5.SetParameter("sharpness", _sharpness);
			}
			ShaderInstance shader6 = _shader;
			if (shader6 != null)
			{
				shader6.SetParameter("width", _width);
			}
			ShaderInstance shader7 = _shader;
			if (shader7 != null)
			{
				shader7.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			}
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
