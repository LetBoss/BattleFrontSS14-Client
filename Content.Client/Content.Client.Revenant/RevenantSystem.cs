using System;
using Content.Shared.Alert;
using Content.Shared.Alert.Components;
using Content.Shared.Revenant;
using Content.Shared.Revenant.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Revenant;

public sealed class RevenantSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RevenantComponent, AppearanceChangeEvent>((ComponentEventRefHandler<RevenantComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevenantComponent, GetGenericAlertCounterAmountEvent>((EntityEventRefHandler<RevenantComponent, GetGenericAlertCounterAmountEvent>)OnGetCounterAmount, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, RevenantComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		bool flag = default(bool);
		bool flag2 = default(bool);
		bool flag3 = default(bool);
		if (_appearance.TryGetData<bool>(uid, (Enum)RevenantVisuals.Harvesting, ref flag, args.Component) && flag)
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(component.HarvestingState));
		}
		else if (_appearance.TryGetData<bool>(uid, (Enum)RevenantVisuals.Stunned, ref flag2, args.Component) && flag2)
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(component.StunnedState));
		}
		else if (_appearance.TryGetData<bool>(uid, (Enum)RevenantVisuals.Corporeal, ref flag3, args.Component))
		{
			if (flag3)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(component.CorporealState));
			}
			else
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(component.State));
			}
		}
	}

	private void OnGetCounterAmount(Entity<RevenantComponent> ent, ref GetGenericAlertCounterAmountEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && !(ent.Comp.EssenceAlert != ProtoId<AlertPrototype>.op_Implicit(args.Alert)))
		{
			args.Amount = ent.Comp.Essence.Int();
		}
	}
}
