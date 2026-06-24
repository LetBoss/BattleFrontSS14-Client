using System;
using System.Linq;
using Content.Client.Items;
using Content.Client.Storage.Systems;
using Content.Shared.Stacks;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Stack;

public sealed class StackSystem : SharedStackSystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private ItemCounterSystem _counterSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, AppearanceChangeEvent>((ComponentEventRefHandler<StackComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.ItemStatus<StackComponent>((Func<Entity<StackComponent>, Control?>)((Entity<StackComponent> ent) => (Control?)(object)new StackStatusControl(Entity<StackComponent>.op_Implicit(ent))));
	}

	public override void SetCount(EntityUid uid, int amount, StackComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent>(uid, ref component, false))
		{
			return;
		}
		base.SetCount(uid, amount, component);
		SpriteComponent val = default(SpriteComponent);
		if (component.Lingering && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			Color val2;
			if (component.Count != 0 || !component.Lingering)
			{
				val2 = Color.White;
			}
			else
			{
				Color darkGray = Color.DarkGray;
				val2 = ((Color)(ref darkGray)).WithAlpha(0.65f);
			}
			Color val3 = val2;
			for (int i = 0; i < val.AllLayers.Count(); i++)
			{
				_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, val)), i, val3);
			}
		}
		if (component.Count <= 0 && !component.Lingering)
		{
			Xform.DetachEntity(uid, ((EntitySystem)this).Transform(uid));
		}
		else
		{
			component.UiUpdateNeeded = true;
		}
	}

	private void OnAppearanceChange(EntityUid uid, StackComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		int actual = default(int);
		if (args.Sprite != null && comp.LayerStates.Count >= 1 && ((SharedAppearanceSystem)_appearanceSystem).TryGetData<int>(uid, (Enum)StackVisuals.Actual, ref actual, args.Component))
		{
			int maxCount = default(int);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<int>(uid, (Enum)StackVisuals.MaxCount, ref maxCount, args.Component))
			{
				maxCount = comp.LayerStates.Count;
			}
			bool hide = default(bool);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)StackVisuals.Hide, ref hide, args.Component))
			{
				hide = false;
			}
			if (comp.LayerFunction != StackLayerFunction.None)
			{
				ApplyLayerFunction(Entity<StackComponent>.op_Implicit((uid, comp)), ref actual, ref maxCount);
			}
			if (comp.IsComposite)
			{
				_counterSystem.ProcessCompositeSprite(uid, actual, maxCount, comp.LayerStates, hide, args.Sprite);
			}
			else
			{
				_counterSystem.ProcessOpaqueSprite(uid, comp.BaseLayer, actual, maxCount, comp.LayerStates, hide, args.Sprite);
			}
		}
	}

	private bool ApplyLayerFunction(Entity<StackComponent> ent, ref int actual, ref int maxCount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StackLayerThresholdComponent comp = default(StackLayerThresholdComponent);
		if (ent.Comp.LayerFunction == StackLayerFunction.Threshold && ((EntitySystem)this).TryComp<StackLayerThresholdComponent>(Entity<StackComponent>.op_Implicit(ent), ref comp))
		{
			ApplyThreshold(comp, ref actual, ref maxCount);
			return true;
		}
		return false;
	}

	private static void ApplyThreshold(StackLayerThresholdComponent comp, ref int actual, ref int maxCount)
	{
		maxCount = Math.Min(comp.Thresholds.Count + 1, maxCount);
		int num = 0;
		foreach (int threshold in comp.Thresholds)
		{
			if (actual >= threshold && num < maxCount)
			{
				num++;
				continue;
			}
			break;
		}
		actual = num;
	}
}
