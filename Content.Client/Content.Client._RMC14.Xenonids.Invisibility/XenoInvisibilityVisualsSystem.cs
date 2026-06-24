using Content.Shared._RMC14.Xenonids.Invisibility;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Xenonids.Invisibility;

public sealed class XenoInvisibilityVisualsSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<XenoActiveInvisibleComponent> _activeInvisibleQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_activeInvisibleQuery = ((EntitySystem)this).GetEntityQuery<XenoActiveInvisibleComponent>();
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoTurnInvisibleComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoTurnInvisibleComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		XenoTurnInvisibleComponent xenoTurnInvisibleComponent = default(XenoTurnInvisibleComponent);
		SpriteComponent item = default(SpriteComponent);
		while (val.MoveNext(ref val2, ref xenoTurnInvisibleComponent, ref item))
		{
			float num = (_activeInvisibleQuery.HasComp(val2) ? xenoTurnInvisibleComponent.Opacity : 1f);
			SpriteSystem sprite = _sprite;
			Entity<SpriteComponent> val3 = Entity<SpriteComponent>.op_Implicit((val2, item));
			Color transparent = Color.Transparent;
			sprite.SetColor(val3, ((Color)(ref transparent)).WithAlpha(num));
		}
	}
}
