using System.Collections.Generic;
using System.Linq;
using Content.Shared.SprayPainter;
using Content.Shared.SprayPainter.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Client.SprayPainter;

public sealed class SprayPainterSystem : SharedSprayPainterSystem
{
	[Dependency]
	private IResourceCache _resourceCache;

	public List<SprayPainterEntry> Entries { get; private set; } = new List<SprayPainterEntry>();

	protected override void CacheStyles()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		base.CacheStyles();
		Entries.Clear();
		State val = default(State);
		foreach (AirlockStyle style in base.Styles)
		{
			string name = style.Name;
			string text = base.Groups.FindAll((AirlockGroupPrototype x) => x.StylePaths.ContainsKey(name))?.MaxBy((AirlockGroupPrototype x) => x.IconPriority)?.StylePaths[name];
			if (text == null)
			{
				Entries.Add(new SprayPainterEntry(name, null));
			}
			else if (!_resourceCache.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / new ResPath(text), true).RSI.TryGetState(StateId.op_Implicit("closed"), ref val))
			{
				Entries.Add(new SprayPainterEntry(name, null));
			}
			else
			{
				Entries.Add(new SprayPainterEntry(name, val.Frame0));
			}
		}
	}
}
