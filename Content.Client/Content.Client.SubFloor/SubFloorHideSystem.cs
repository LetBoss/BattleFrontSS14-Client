using System;
using Content.Client.UserInterface.Systems.Sandbox;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.ViewVariables;

namespace Content.Client.SubFloor;

public sealed class SubFloorHideSystem : SharedSubFloorHideSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IUserInterfaceManager _ui;

	private bool _showAll;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool ShowAll
	{
		get
		{
			return _showAll;
		}
		set
		{
			if (_showAll != value)
			{
				_showAll = value;
				_ui.GetUIController<SandboxUIController>().SetToggleSubfloors(value);
				ShowSubfloorRequestEvent showSubfloorRequestEvent = new ShowSubfloorRequestEvent
				{
					Value = value
				};
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)showSubfloorRequestEvent);
			}
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, AppearanceChangeEvent>((ComponentEventRefHandler<SubFloorHideComponent, AppearanceChangeEvent>)OnAppearanceChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<ShowSubfloorRequestEvent>((EntityEventHandler<ShowSubfloorRequestEvent>)OnRequestReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
	}

	private void OnPlayerDetached(LocalPlayerDetachedEvent ev)
	{
		ShowAll = false;
	}

	private void OnRequestReceived(ShowSubfloorRequestEvent ev)
	{
		UpdateAll();
	}

	private void OnAppearanceChanged(EntityUid uid, SubFloorHideComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		bool flag = default(bool);
		_appearance.TryGetData<bool>(uid, (Enum)SubFloorVisuals.Covered, ref flag, args.Component);
		bool flag2 = default(bool);
		_appearance.TryGetData<bool>(uid, (Enum)SubFloorVisuals.ScannerRevealed, ref flag2, args.Component);
		flag2 &= !ShowAll;
		bool flag3 = !flag || ShowAll || flag2;
		foreach (ISpriteLayer allLayer in args.Sprite.AllLayers)
		{
			allLayer.Visible = flag3;
		}
		bool flag4 = false;
		int num = default(int);
		foreach (Enum visibleLayer in component.VisibleLayers)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), visibleLayer, ref num, false))
			{
				ISpriteLayer obj = args.Sprite[num];
				obj.Visible = true;
				Color color = obj.Color;
				obj.Color = ((Color)(ref color)).WithAlpha(1f);
				flag4 = true;
			}
		}
		_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), flag4 || flag3);
		if (ShowAll)
		{
			int valueOrDefault = component.OriginalDrawDepth.GetValueOrDefault();
			if (!component.OriginalDrawDepth.HasValue)
			{
				valueOrDefault = args.Sprite.DrawDepth;
				component.OriginalDrawDepth = valueOrDefault;
			}
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 10);
		}
		else if (flag2)
		{
			int? originalDrawDepth = component.OriginalDrawDepth;
			if (!originalDrawDepth.HasValue)
			{
				component.OriginalDrawDepth = args.Sprite.DrawDepth;
				int num2 = -9;
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), args.Sprite.DrawDepth - (num2 - 1));
			}
		}
		else if (component.OriginalDrawDepth.HasValue)
		{
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.OriginalDrawDepth.Value);
			component.OriginalDrawDepth = null;
		}
	}

	private void UpdateAll()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<SubFloorHideComponent, AppearanceComponent> val = ((EntitySystem)this).AllEntityQuery<SubFloorHideComponent, AppearanceComponent>();
		EntityUid val2 = default(EntityUid);
		SubFloorHideComponent subFloorHideComponent = default(SubFloorHideComponent);
		AppearanceComponent val3 = default(AppearanceComponent);
		while (val.MoveNext(ref val2, ref subFloorHideComponent, ref val3))
		{
			_appearance.QueueUpdate(val2, val3);
		}
	}
}
