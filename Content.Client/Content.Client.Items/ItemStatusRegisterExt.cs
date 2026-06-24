using System;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Items;

public static class ItemStatusRegisterExt
{
	public static void ItemStatus<TComp>(this Subscriptions subs, Func<Entity<TComp>, Control?> createControl) where TComp : IComponent
	{
		subs.SubscribeLocalEvent<TComp, ItemStatusCollectMessage>((EntityEventRefHandler<TComp, ItemStatusCollectMessage>)delegate(Entity<TComp> entity, ref ItemStatusCollectMessage args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Control val = createControl(entity);
			if (val != null)
			{
				args.Controls.Add(val);
			}
		}, (Type[])null, (Type[])null);
	}
}
