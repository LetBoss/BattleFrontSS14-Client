using System;
using Content.Shared.Drunk;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drunk;

public sealed class DrunkOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Drunk");

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

	private readonly ShaderInstance _drunkShader;

	public float CurrentBoozePower;

	private const float VisualThreshold = 10f;

	private const float PowerDivisor = 250f;

	private float _visualScale;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public DrunkOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DrunkOverlay>(this);
		_drunkShader = _prototypeManager.Index<ShaderPrototype>(Shader).InstanceUnique();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		StatusEffectsComponent status = default(StatusEffectsComponent);
		if (localEntity.HasValue && _entityManager.HasComponent<DrunkComponent>(localEntity) && _entityManager.TryGetComponent<StatusEffectsComponent>(localEntity, ref status) && _sysMan.GetEntitySystem<StatusEffectsSystem>().TryGetTime(localEntity.Value, ProtoId<StatusEffectPrototype>.op_Implicit(SharedDrunkSystem.DrunkKey), out var time, status))
		{
			TimeSpan curTime = _timing.CurTime;
			float num = (float)(time.Value.Item2 - curTime).TotalSeconds;
			CurrentBoozePower += 8f * (0.5f * num - CurrentBoozePower) * ((FrameEventArgs)(ref args)).DeltaSeconds / (num + 1f);
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
		_visualScale = BoozePowerToVisual(CurrentBoozePower);
		return _visualScale > 0f;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_drunkShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			_drunkShader.SetParameter("boozePower", _visualScale);
			((DrawingHandleBase)worldHandle).UseShader(_drunkShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}

	private float BoozePowerToVisual(float boozePower)
	{
		if (boozePower < 50f)
		{
			return 0f;
		}
		return Math.Clamp((boozePower - 10f) / 250f, 0f, 1f);
	}
}
