using System;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Singularity.Systems;

public sealed class EmitterSystem : SharedEmitterSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EmitterComponent, AppearanceChangeEvent>((ComponentEventRefHandler<EmitterComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, EmitterComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		EmitterVisualState emitterVisualState = default(EmitterVisualState);
		if (!_appearance.TryGetData<EmitterVisualState>(uid, (Enum)EmitterVisuals.VisualState, ref emitterVisualState, args.Component))
		{
			emitterVisualState = EmitterVisualState.Off;
		}
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)EmitterVisualLayers.Lights, ref num, false))
		{
			return;
		}
		switch (emitterVisualState)
		{
		case EmitterVisualState.On:
			if (component.OnState != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(component.OnState));
			}
			break;
		case EmitterVisualState.Underpowered:
			if (component.UnderpoweredState != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(component.UnderpoweredState));
			}
			break;
		case EmitterVisualState.Off:
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
