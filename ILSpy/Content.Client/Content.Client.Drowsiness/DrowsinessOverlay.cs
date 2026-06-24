using System;
using Content.Shared.Drowsiness;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drowsiness;

public sealed class DrowsinessOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Drowsiness");

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

	private readonly ShaderInstance _drowsinessShader;

	public float CurrentPower;

	private const float PowerDivisor = 250f;

	private const float Intensity = 0.2f;

	private float _visualScale;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public DrowsinessOverlay()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DrowsinessOverlay>(this);
		_statusEffects = _sysMan.GetEntitySystem<SharedStatusEffectsSystem>();
		_drowsinessShader = _prototypeManager.Index<ShaderPrototype>(Shader).InstanceUnique();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && _statusEffects.TryGetEffectsEndTimeWithComp<DrowsinessStatusEffectComponent>(localEntity, out TimeSpan? endTime))
		{
			TimeSpan valueOrDefault = endTime.GetValueOrDefault();
			if (!endTime.HasValue)
			{
				valueOrDefault = TimeSpan.MaxValue;
				endTime = valueOrDefault;
			}
			float num = (float)(endTime - _timing.CurTime).Value.TotalSeconds;
			CurrentPower += 8f * (0.5f * num - CurrentPower) * ((FrameEventArgs)(ref args)).DeltaSeconds / (num + 1f);
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
		_visualScale = Math.Clamp(CurrentPower / 250f, 0f, 1f);
		return _visualScale > 0f;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_drowsinessShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			_drowsinessShader.SetParameter("Strength", _visualScale * 0.2f);
			((DrawingHandleBase)worldHandle).UseShader(_drowsinessShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
