using Content.Shared._RMC14.Xenonids.Doom;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Xenonids.Doom;

public sealed class DoomOverlay : Overlay
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntityManager _entityManager;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public DoomOverlay()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DoomOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCDoomVision")).InstanceUnique();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		MobDoomedComponent mobDoomedComponent = default(MobDoomedComponent);
		if (_entityManager.TryGetComponent<MobDoomedComponent>(((ISharedPlayerManager)_playerManager).LocalEntity, ref mobDoomedComponent) && base.ScreenTexture != null && args.Viewport.Eye != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_shader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
