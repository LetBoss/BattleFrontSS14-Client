using System;
using Content.Shared.Movement.Components;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Silicons.Borgs;

public sealed class BorgSwitchableTypeSystem : SharedBorgSwitchableTypeSystem
{
	[Dependency]
	private BorgSystem _borgSystem;

	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BorgSwitchableTypeComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<BorgSwitchableTypeComponent, AfterAutoHandleStateEvent>)AfterStateHandler, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentStartup>((EntityEventRefHandler<BorgSwitchableTypeComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
	}

	private void OnComponentStartup(Entity<BorgSwitchableTypeComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateEntityAppearance(ent);
	}

	private void AfterStateHandler(Entity<BorgSwitchableTypeComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateEntityAppearance(ent);
	}

	protected override void UpdateEntityAppearance(Entity<BorgSwitchableTypeComponent> entity, BorgTypePrototype prototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref item))
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), item)), (Enum)BorgVisualLayers.Body, StateId.op_Implicit(prototype.SpriteBodyState));
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), item)), (Enum)BorgVisualLayers.LightStatus, StateId.op_Implicit(prototype.SpriteToggleLightState));
		}
		BorgChassisComponent item2 = default(BorgChassisComponent);
		if (((EntitySystem)this).TryComp<BorgChassisComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref item2))
		{
			_borgSystem.SetMindStates(Entity<BorgChassisComponent>.op_Implicit((entity.Owner, item2)), prototype.SpriteHasMindState, prototype.SpriteNoMindState);
			AppearanceComponent val = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref val))
			{
				((SharedAppearanceSystem)_appearance).QueueUpdate(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), val);
			}
		}
		string spriteBodyMovementState = prototype.SpriteBodyMovementState;
		if (spriteBodyMovementState != null)
		{
			SpriteMovementComponent spriteMovementComponent = ((EntitySystem)this).EnsureComp<SpriteMovementComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity));
			spriteMovementComponent.NoMovementLayers.Clear();
			spriteMovementComponent.NoMovementLayers["movement"] = new PrototypeLayerData
			{
				State = prototype.SpriteBodyState
			};
			spriteMovementComponent.MovementLayers.Clear();
			spriteMovementComponent.MovementLayers["movement"] = new PrototypeLayerData
			{
				State = spriteBodyMovementState
			};
		}
		else
		{
			((EntitySystem)this).RemComp<SpriteMovementComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity));
		}
		base.UpdateEntityAppearance(entity, prototype);
	}
}
