using System;
using Content.Client.Items;
using Content.Client.Tools.Components;
using Content.Client.Tools.UI;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Tools;

public sealed class ToolSystem : SharedToolSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<WelderComponent>((Func<Entity<WelderComponent>, Control?>)((Entity<WelderComponent> ent) => (Control?)(object)new WelderStatusControl(ent, (IEntityManager)(object)((EntitySystem)this).EntityManager, this)));
		((EntitySystem)this).Subs.ItemStatus<MultipleToolComponent>((Func<Entity<MultipleToolComponent>, Control?>)((Entity<MultipleToolComponent> ent) => (Control?)(object)new MultipleToolStatusControl(Entity<MultipleToolComponent>.op_Implicit(ent))));
	}

	public override void SetMultipleTool(EntityUid uid, MultipleToolComponent? multiple = null, ToolComponent? tool = null, bool playSound = false, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipleToolComponent>(uid, ref multiple, true))
		{
			return;
		}
		base.SetMultipleTool(uid, multiple, tool, playSound, user);
		multiple.UiUpdateNeeded = true;
		SpriteComponent item = default(SpriteComponent);
		if (multiple.Entries.Length > multiple.CurrentEntry && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			MultipleToolComponent.ToolEntry toolEntry = multiple.Entries[multiple.CurrentEntry];
			if (toolEntry.Sprite != null)
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, item)), 0, toolEntry.Sprite);
			}
		}
	}
}
