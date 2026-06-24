using System;
using System.Numerics;
using Content.Shared.Emag.Systems;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Medical.Cryogenics;

public sealed class CryoPodSystem : SharedCryoPodSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, ComponentInit>((ComponentEventHandler<CryoPodComponent, ComponentInit>)base.OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>)base.AddAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, GotEmaggedEvent>((ComponentEventRefHandler<CryoPodComponent, GotEmaggedEvent>)base.OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, CryoPodPryFinished>((ComponentEventHandler<CryoPodComponent, CryoPodPryFinished>)base.OnCryoPodPryFinished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, AppearanceChangeEvent>((ComponentEventRefHandler<CryoPodComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsideCryoPodComponent, ComponentStartup>((ComponentEventHandler<InsideCryoPodComponent, ComponentStartup>)OnCryoPodInsertion, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsideCryoPodComponent, ComponentRemove>((ComponentEventHandler<InsideCryoPodComponent, ComponentRemove>)OnCryoPodRemoval, (Type[])null, (Type[])null);
	}

	private void OnCryoPodInsertion(EntityUid uid, InsideCryoPodComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			component.PreviousOffset = val.Offset;
			_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, val)), new Vector2(0f, 1f));
		}
	}

	private void OnCryoPodRemoval(EntityUid uid, InsideCryoPodComponent component, ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), component.PreviousOffset);
		}
	}

	private void OnAppearanceChange(EntityUid uid, CryoPodComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		bool flag2 = default(bool);
		if (args.Sprite != null && _appearance.TryGetData<bool>(uid, (Enum)CryoPodComponent.CryoPodVisuals.ContainsEntity, ref flag, args.Component) && _appearance.TryGetData<bool>(uid, (Enum)CryoPodComponent.CryoPodVisuals.IsOn, ref flag2, args.Component))
		{
			if (flag)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)CryoPodVisualLayers.Base, StateId.op_Implicit("pod-open"));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)CryoPodVisualLayers.Cover, false);
			}
			else
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)CryoPodVisualLayers.Base, StateId.op_Implicit(flag2 ? "pod-on" : "pod-off"));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)CryoPodVisualLayers.Cover, StateId.op_Implicit(flag2 ? "cover-on" : "cover-off"));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)CryoPodVisualLayers.Cover, true);
			}
		}
	}
}
