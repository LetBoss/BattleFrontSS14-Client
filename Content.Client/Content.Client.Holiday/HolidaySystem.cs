using System;
using Content.Shared.Holiday;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Client.Holiday;

public sealed class HolidaySystem : EntitySystem
{
	[Dependency]
	private IResourceCache _rescache;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HolidayRsiSwapComponent, AppearanceChangeEvent>((EntityEventRefHandler<HolidayRsiSwapComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(Entity<HolidayRsiSwapComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		string key = default(string);
		if (_appearance.TryGetData<string>(Entity<HolidayRsiSwapComponent>.op_Implicit(ent), (Enum)HolidayVisuals.Holiday, ref key, args.Component) && ent.Comp.Sprite.TryGetValue(key, out string value) && args.Sprite != null)
		{
			ResPath val = SpriteSpecifierSerializer.TextureRoot / value;
			RSIResource val2 = default(RSIResource);
			if (_rescache.TryGetResource<RSIResource>(val, ref val2))
			{
				_sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, args.Sprite)), val2.RSI);
			}
		}
	}
}
