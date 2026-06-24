using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class MonoTag : IMarkupTag, IMarkupTagHandler
{
	public static readonly ProtoId<FontPrototype> MonoFont = ProtoId<FontPrototype>.op_Implicit("Monospace");

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	public string Name => "mono";

	public void PushDrawContext(MarkupNode node, MarkupDrawingContext context)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Font item = FontTag.CreateFont(context.Font, node, _resourceCache, _prototypeManager, ProtoId<FontPrototype>.op_Implicit(MonoFont));
		context.Font.Push(item);
	}

	public void PopDrawContext(MarkupNode node, MarkupDrawingContext context)
	{
		context.Font.Pop();
	}
}
