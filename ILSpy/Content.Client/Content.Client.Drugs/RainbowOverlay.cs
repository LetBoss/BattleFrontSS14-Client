using System;
using Content.Shared.CCVar;
using Content.Shared.Drugs;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drugs;

public sealed class RainbowOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Rainbow");

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntitySystemManager _sysMan;

	[Dependency]
	private IGameTiming _timing;

	private readonly SharedStatusEffectsSystem _statusEffects;

	private readonly ShaderInstance _rainbowShader;

	public float Intoxication;

	public float TimeTicker;

	public float Phase;

	private const float VisualThreshold = 10f;

	private const float PowerDivisor = 250f;

	private float _timeScale;

	private float _warpScale;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	private float EffectScale => Math.Clamp((Intoxication - 10f) / 250f, 0f, 1f);

	public RainbowOverlay()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RainbowOverlay>(this);
		_statusEffects = _sysMan.GetEntitySystem<SharedStatusEffectsSystem>();
		_rainbowShader = _prototypeManager.Index<ShaderPrototype>(Shader).InstanceUnique();
		_config.OnValueChanged<bool>(CCVars.ReducedMotion, (Action<bool>)OnReducedMotionChanged, true);
	}

	private void OnReducedMotionChanged(bool reducedMotion)
	{
		_timeScale = (reducedMotion ? 0f : 1f);
		_warpScale = (reducedMotion ? 0f : 1f);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && _statusEffects.TryGetEffectsEndTimeWithComp<SeeingRainbowsStatusEffectComponent>(localEntity, out TimeSpan? endTime))
		{
			TimeSpan valueOrDefault = endTime.GetValueOrDefault();
			if (!endTime.HasValue)
			{
				valueOrDefault = TimeSpan.MaxValue;
				endTime = valueOrDefault;
			}
			float num = (float)(endTime - _timing.CurTime).Value.TotalSeconds;
			TimeTicker += ((FrameEventArgs)(ref args)).DeltaSeconds;
			if (num - TimeTicker > num / 16f)
			{
				Intoxication += (num - Intoxication) * ((FrameEventArgs)(ref args)).DeltaSeconds / 16f;
			}
			else
			{
				Intoxication -= Intoxication / (num - TimeTicker) * ((FrameEventArgs)(ref args)).DeltaSeconds;
			}
		}
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		EyeComponent val = default(EyeComponent);
		if (!_entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager)_playerManager).LocalEntity, ref val))
		{
			return false;
		}
		if ((object)args.Viewport.Eye != val.Eye)
		{
			return false;
		}
		return EffectScale > 0f;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_rainbowShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			_rainbowShader.SetParameter("colorScale", EffectScale);
			_rainbowShader.SetParameter("timeScale", _timeScale);
			_rainbowShader.SetParameter("warpScale", _warpScale * EffectScale);
			_rainbowShader.SetParameter("phase", Phase);
			((DrawingHandleBase)worldHandle).UseShader(_rainbowShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
