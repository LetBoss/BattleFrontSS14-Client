using System;
using Content.Shared.Dragon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Dragon;

public sealed class DragonSystem : EntitySystem
{
	[Dependency]
	private SharedPointLightSystem _lights;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DragonRiftComponent, ComponentHandleState>((ComponentEventRefHandler<DragonRiftComponent, ComponentHandleState>)OnRiftHandleState, (Type[])null, (Type[])null);
	}

	private void OnRiftHandleState(EntityUid uid, DragonRiftComponent component, ref ComponentHandleState args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is DragonRiftComponentState dragonRiftComponentState) || component.State == dragonRiftComponentState.State)
		{
			return;
		}
		component.State = dragonRiftComponentState.State;
		SpriteComponent val = default(SpriteComponent);
		((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val);
		PointLightComponent val2 = default(PointLightComponent);
		((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val2);
		if (val == null && val2 == null)
		{
			return;
		}
		switch (dragonRiftComponentState.State)
		{
		case DragonRiftState.Charging:
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, val)), 0, Color.FromHex((ReadOnlySpan<char>)"#569fff", (Color?)null));
			if (val2 != null)
			{
				_lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>)"#366db5", (Color?)null), (SharedPointLightComponent)(object)val2);
			}
			break;
		case DragonRiftState.AlmostFinished:
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, val)), 0, Color.FromHex((ReadOnlySpan<char>)"#cf4cff", (Color?)null));
			if (val2 != null)
			{
				_lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>)"#9e2fc1", (Color?)null), (SharedPointLightComponent)(object)val2);
			}
			break;
		case DragonRiftState.Finished:
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, val)), 0, Color.FromHex((ReadOnlySpan<char>)"#edbc36", (Color?)null));
			if (val2 != null)
			{
				_lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>)"#cbaf20", (Color?)null), (SharedPointLightComponent)(object)val2);
			}
			break;
		}
	}
}
