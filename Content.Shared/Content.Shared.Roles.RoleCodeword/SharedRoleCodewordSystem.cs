using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Roles.RoleCodeword;

public abstract class SharedRoleCodewordSystem : EntitySystem
{
	public void SetRoleCodewords(Entity<RoleCodewordComponent> ent, string key, List<string> codewords, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		CodewordsData data = new CodewordsData(color, codewords);
		ent.Comp.RoleCodewords[key] = data;
		((EntitySystem)this).Dirty<RoleCodewordComponent>(ent, (MetaDataComponent)null);
	}
}
