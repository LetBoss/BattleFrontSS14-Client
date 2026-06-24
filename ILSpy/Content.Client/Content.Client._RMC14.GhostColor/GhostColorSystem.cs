using System;
using Content.Shared._RMC14.GhostColor;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.GhostColor;

public sealed class GhostColorSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.FromHex((ReadOnlySpan<char>)"#FFFFFF88", (Color?)null);
		EntityQueryEnumerator<GhostColorComponent, SpriteComponent> val2 = ((EntitySystem)this).EntityQueryEnumerator<GhostColorComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		GhostColorComponent ghostColorComponent = default(GhostColorComponent);
		SpriteComponent item2 = default(SpriteComponent);
		while (val2.MoveNext(ref item, ref ghostColorComponent, ref item2))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((item, item2)), ghostColorComponent.Color ?? val);
		}
	}
}
