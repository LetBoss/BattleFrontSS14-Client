using Content.Shared._RMC14.Areas;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Areas;

public sealed class AreasCommandSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public bool Enabled;

	public bool ShowCAS;

	public override void Update(float frameTime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (Enabled)
		{
			AllEntityQueryEnumerator<AreaComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<AreaComponent, SpriteComponent>();
			EntityUid item = default(EntityUid);
			AreaComponent areaComponent = default(AreaComponent);
			SpriteComponent item2 = default(SpriteComponent);
			while (val.MoveNext(ref item, ref areaComponent, ref item2))
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((item, item2)), areaComponent.CAS == ShowCAS);
			}
		}
	}
}
