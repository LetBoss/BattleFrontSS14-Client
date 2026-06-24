using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Rotation;

public abstract class SharedRotationVisualsSystem : EntitySystem
{
	public void SetHorizontalAngle(Entity<RotationVisualsComponent?> ent, Angle angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RotationVisualsComponent>(Entity<RotationVisualsComponent>.op_Implicit(ent), ref ent.Comp, false) && !((Angle)(ref ent.Comp.HorizontalRotation)).Equals(angle))
		{
			ent.Comp.HorizontalRotation = angle;
			((EntitySystem)this).Dirty<RotationVisualsComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void ResetHorizontalAngle(Entity<RotationVisualsComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RotationVisualsComponent>(Entity<RotationVisualsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			SetHorizontalAngle(ent, ent.Comp.DefaultRotation);
		}
	}
}
