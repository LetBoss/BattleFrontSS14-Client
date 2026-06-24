using Content.Shared._RMC14.NightVision;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionFilterOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototypes;

	private static readonly ProtoId<ShaderPrototype> ShaderId = ProtoId<ShaderPrototype>.op_Implicit("RMCNightVision");

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public NightVisionFilterOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<NightVisionFilterOverlay>(this);
		_shader = _prototypes.Index<ShaderPrototype>(ShaderId).InstanceUnique();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		NightVisionComponent nightVisionComponent = default(NightVisionComponent);
		if (_entity.TryGetComponent<NightVisionComponent>(((ISharedPlayerManager)_players).LocalEntity, ref nightVisionComponent) && nightVisionComponent.State != NightVisionState.Off)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			if (nightVisionComponent.Green && base.ScreenTexture != null)
			{
				_shader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
				((DrawingHandleBase)worldHandle).UseShader(_shader);
				worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
				((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
			}
		}
	}
}
