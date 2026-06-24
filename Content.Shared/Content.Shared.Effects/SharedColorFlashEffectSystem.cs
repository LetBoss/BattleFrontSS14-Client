using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Shared.Effects;

public abstract class SharedColorFlashEffectSystem : EntitySystem
{
	public abstract void RaiseEffect(Color color, List<EntityUid> entities, Filter filter);
}
