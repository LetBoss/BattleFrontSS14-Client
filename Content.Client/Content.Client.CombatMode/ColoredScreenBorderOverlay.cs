using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.CombatMode;

public sealed class ColoredScreenBorderOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("ColoredScreenBorder");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public ColoredScreenBorderOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<ColoredScreenBorderOverlay>(this);
		_shader = _prototypeManager.Index<ShaderPrototype>(Shader).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		Box2 worldAABB = args.WorldAABB;
		worldHandle.DrawRect(worldAABB, Color.White, true);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
