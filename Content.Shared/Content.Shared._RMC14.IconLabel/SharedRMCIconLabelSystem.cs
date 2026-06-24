using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared._RMC14.IconLabel;

public abstract class SharedRMCIconLabelSystem : EntitySystem
{
	public void Label(Entity<IconLabelComponent?> ent, LocId newLocId, List<(string, object)> newParams)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp = ((EntitySystem)this).EnsureComp<IconLabelComponent>(Entity<IconLabelComponent>.op_Implicit(ent));
		ent.Comp.LabelTextLocId = newLocId;
		ent.Comp.LabelTextParams = new List<(string, object)>(newParams);
		((EntitySystem)this).Dirty<IconLabelComponent>(ent, (MetaDataComponent)null);
	}

	public void Label(Entity<IconLabelComponent?> ent, LocId newLocId, params (string, object)[] newParams)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Label(ent, newLocId, newParams.ToList());
	}
}
