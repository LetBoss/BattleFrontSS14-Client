using System;
using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Construction;

public sealed class FlatpackSystem : SharedFlatpackSystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlatpackComponent, AppearanceChangeEvent>((EntityEventRefHandler<FlatpackComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(Entity<FlatpackComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Entity<FlatpackComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		FlatpackComponent flatpackComponent = default(FlatpackComponent);
		val.Deconstruct(ref val2, ref flatpackComponent);
		FlatpackComponent flatpackComponent2 = flatpackComponent;
		string text = default(string);
		EntityPrototype val3 = default(EntityPrototype);
		SpriteComponent val4 = default(SpriteComponent);
		if (!((SharedAppearanceSystem)_appearance).TryGetData<string>(Entity<FlatpackComponent>.op_Implicit(ent), (Enum)FlatpackVisuals.Machine, ref text, (AppearanceComponent)null) || args.Sprite == null || !PrototypeManager.TryIndex<EntityPrototype>(text, ref val3) || !val3.TryGetComponent<SpriteComponent>(ref val4, ((EntitySystem)this).EntityManager.ComponentFactory))
		{
			return;
		}
		Color? val5 = null;
		foreach (ISpriteLayer allLayer in val4.AllLayers)
		{
			string name = allLayer.RsiState.Name;
			if (name != null && flatpackComponent2.BoardColors.TryGetValue(name, out var value))
			{
				val5 = value;
				break;
			}
		}
		if (val5.HasValue)
		{
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, args.Sprite)), (Enum)FlatpackVisualLayers.Overlay, val5.Value);
		}
	}
}
