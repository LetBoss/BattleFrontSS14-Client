using System.Numerics;
using Content.Shared._RMC14.Xenonids.Screech;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Xenonids.Screech;

public sealed class RMCXenoScreechShockWaveOverlay : Overlay, IEntityEventSubscriber
{
	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private SharedTransformSystem? _xformSystem;

	private readonly ShaderInstance _shader;

	private Vector2 _position;

	private float _waveStrength;

	private float _waveSpeed;

	private float _downScale;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public RMCXenoScreechShockWaveOverlay()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RMCXenoScreechShockWaveOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCXenoScreechShockWave")).Instance().Duplicate();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null || (_xformSystem == null && !_entMan.TrySystem<SharedTransformSystem>(ref _xformSystem)))
		{
			return false;
		}
		EntityUid val = default(EntityUid);
		RMCXenoScreechShockWaveComponent rMCXenoScreechShockWaveComponent = default(RMCXenoScreechShockWaveComponent);
		TransformComponent val2 = default(TransformComponent);
		if (_entMan.EntityQueryEnumerator<RMCXenoScreechShockWaveComponent, TransformComponent>().MoveNext(ref val, ref rMCXenoScreechShockWaveComponent, ref val2))
		{
			if (val2.MapID != args.MapId)
			{
				return false;
			}
			Vector2 worldPosition = _xformSystem.GetWorldPosition(val);
			Vector2 position = args.Viewport.WorldToLocal(worldPosition);
			position.Y = 1f - position.Y / (float)args.Viewport.Size.Y;
			position.X /= args.Viewport.Size.X;
			_position = position;
			_waveStrength = rMCXenoScreechShockWaveComponent.WaveStrength;
			_waveSpeed = rMCXenoScreechShockWaveComponent.WaveSpeed;
			_downScale = rMCXenoScreechShockWaveComponent.DownScale;
			return true;
		}
		return false;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null && args.Viewport.Eye != null)
		{
			ShaderInstance shader = _shader;
			if (shader != null)
			{
				shader.SetParameter("position", _position);
			}
			ShaderInstance shader2 = _shader;
			if (shader2 != null)
			{
				shader2.SetParameter("waveSpeed", _waveSpeed);
			}
			ShaderInstance shader3 = _shader;
			if (shader3 != null)
			{
				shader3.SetParameter("downScale", _downScale);
			}
			ShaderInstance shader4 = _shader;
			if (shader4 != null)
			{
				shader4.SetParameter("waveStrength", _waveStrength);
			}
			ShaderInstance shader5 = _shader;
			if (shader5 != null)
			{
				shader5.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			}
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
