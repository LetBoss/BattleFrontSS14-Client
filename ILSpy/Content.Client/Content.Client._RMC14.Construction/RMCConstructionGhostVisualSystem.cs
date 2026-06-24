using System;
using Content.Shared._RMC14.Construction;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Construction;

public sealed class RMCConstructionGhostVisualSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	private static readonly Color GhostColor = new Color(0.45f, 0.7f, 1f, 0.5f);

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionGhostComponent, ComponentInit>((EntityEventRefHandler<RMCConstructionGhostComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionGhostComponent, ComponentRemove>((EntityEventRefHandler<RMCConstructionGhostComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<RMCConstructionGhostComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCConstructionGhostComponent>.op_Implicit(ent), ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((Entity<RMCConstructionGhostComponent>.op_Implicit(ent), item)), GhostColor);
		}
	}

	private void OnRemove(Entity<RMCConstructionGhostComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCConstructionGhostComponent>.op_Implicit(ent), ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((Entity<RMCConstructionGhostComponent>.op_Implicit(ent), item)), Color.White);
		}
	}
}
