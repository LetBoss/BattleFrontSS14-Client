using System;
using Content.Shared.Mobs;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Silicons.Borgs;

public sealed class BorgSystem : SharedBorgSystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, AppearanceChangeEvent>((ComponentEventRefHandler<BorgChassisComponent, AppearanceChangeEvent>)OnBorgAppearanceChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MMIComponent, AppearanceChangeEvent>((ComponentEventRefHandler<MMIComponent, AppearanceChangeEvent>)OnMMIAppearanceChanged, (Type[])null, (Type[])null);
	}

	private void OnBorgAppearanceChanged(EntityUid uid, BorgChassisComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateBorgAppearance(uid, component, args.Component, args.Sprite);
		}
	}

	protected override void OnInserted(EntityUid uid, BorgChassisComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)component).Initialized)
		{
			base.OnInserted(uid, component, args);
			UpdateBorgAppearance(uid, component);
		}
	}

	protected override void OnRemoved(EntityUid uid, BorgChassisComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)component).Initialized)
		{
			base.OnRemoved(uid, component, args);
			UpdateBorgAppearance(uid, component);
		}
	}

	private void UpdateBorgAppearance(EntityUid uid, BorgChassisComponent? component = null, AppearanceComponent? appearance = null, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BorgChassisComponent, AppearanceComponent, SpriteComponent>(uid, ref component, ref appearance, ref sprite, true))
		{
			return;
		}
		MobState mobState = default(MobState);
		if (((SharedAppearanceSystem)_appearance).TryGetData<MobState>(uid, (Enum)MobStateVisuals.State, ref mobState, appearance) && mobState != MobState.Alive)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BorgVisualLayers.Light, false);
			return;
		}
		bool flag = default(bool);
		if (!((SharedAppearanceSystem)_appearance).TryGetData<bool>(uid, (Enum)BorgVisuals.HasPlayer, ref flag, appearance))
		{
			flag = false;
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BorgVisualLayers.Light, component.BrainEntity.HasValue || flag);
		_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BorgVisualLayers.Light, StateId.op_Implicit(flag ? component.HasMindState : component.NoMindState));
	}

	private void OnMMIAppearanceChanged(EntityUid uid, MMIComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			SpriteComponent sprite = args.Sprite;
			bool flag = default(bool);
			if (!((SharedAppearanceSystem)_appearance).TryGetData<bool>(uid, (Enum)MMIVisuals.BrainPresent, ref flag, (AppearanceComponent)null))
			{
				flag = false;
			}
			bool flag2 = default(bool);
			if (!((SharedAppearanceSystem)_appearance).TryGetData<bool>(uid, (Enum)MMIVisuals.HasMind, ref flag2, (AppearanceComponent)null))
			{
				flag2 = false;
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)MMIVisualLayers.Brain, flag);
			if (!flag)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)MMIVisualLayers.Base, StateId.op_Implicit(component.NoBrainState));
				return;
			}
			string text = (flag2 ? component.HasMindState : component.NoMindState);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)MMIVisualLayers.Base, StateId.op_Implicit(text));
		}
	}

	public void SetMindStates(Entity<BorgChassisComponent> borg, string hasMindState, string noMindState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		borg.Comp.HasMindState = hasMindState;
		borg.Comp.NoMindState = noMindState;
	}
}
