using System;
using Content.Shared.Flash;
using Content.Shared.Flash.Components;
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

namespace Content.Client.Flash;

public sealed class FlashOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> FlashedEffectShader = ProtoId<ShaderPrototype>.op_Implicit("FlashedEffect");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IGameTiming _timing;

	private readonly SharedFlashSystem _flash;

	private readonly StatusEffectsSystem _statusSys;

	private readonly ShaderInstance _shader;

	public float PercentComplete;

	public Texture? ScreenshotTexture;

	public override OverlaySpace Space => (OverlaySpace)4;

	public FlashOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<FlashOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(FlashedEffectShader).InstanceUnique();
		_flash = _entityManager.System<SharedFlashSystem>();
		_statusSys = _entityManager.System<StatusEffectsSystem>();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		StatusEffectsComponent status = default(StatusEffectsComponent);
		if (localEntity.HasValue && _entityManager.HasComponent<FlashedComponent>(localEntity) && _entityManager.TryGetComponent<StatusEffectsComponent>(localEntity, ref status) && _statusSys.TryGetTime(localEntity.Value, ProtoId<StatusEffectPrototype>.op_Implicit(_flash.FlashedKey), out var time, status))
		{
			TimeSpan curTime = _timing.CurTime;
			float num = (float)(time.Value.Item2 - time.Value.Item1).TotalSeconds;
			float num2 = (float)(curTime - time.Value.Item1).TotalSeconds;
			PercentComplete = num2 / num;
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
		return PercentComplete < 1f;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		if (((Overlay)this).RequestScreenTexture && base.ScreenTexture != null)
		{
			ScreenshotTexture = base.ScreenTexture;
			((Overlay)this).RequestScreenTexture = false;
		}
		if (ScreenshotTexture != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_shader.SetParameter("percentComplete", PercentComplete);
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawTextureRectRegion(ScreenshotTexture, ref args.WorldBounds, (Color?)null, (UIBox2?)null);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		ScreenshotTexture = null;
	}
}
