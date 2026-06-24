using System;
using Content.Shared.DeviceLinking.Components;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.DeviceLinking.Systems;

public sealed class TwoWayLeverSystem : EntitySystem
{
	[Dependency]
	private SharedDeviceLinkSystem _signalSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	private const string _leftToggleImage = "rotate_ccw.svg.192dpi.png";

	private const string _rightToggleImage = "rotate_cw.svg.192dpi.png";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TwoWayLeverComponent, ComponentInit>((ComponentEventHandler<TwoWayLeverComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TwoWayLeverComponent, ActivateInWorldEvent>((ComponentEventHandler<TwoWayLeverComponent, ActivateInWorldEvent>)OnActivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractionVerbs, (Type[])null, (Type[])null);
	}

	private void OnInit(EntityUid uid, TwoWayLeverComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_signalSystem.EnsureSourcePorts(uid, component.LeftPort, component.RightPort, component.MiddlePort);
	}

	private void OnActivated(EntityUid uid, TwoWayLeverComponent component, ActivateInWorldEvent args)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			component.State = component.State switch
			{
				TwoWayLeverState.Middle => (!component.NextSignalLeft) ? TwoWayLeverState.Right : TwoWayLeverState.Left, 
				TwoWayLeverState.Right => TwoWayLeverState.Middle, 
				TwoWayLeverState.Left => TwoWayLeverState.Middle, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			StateChanged(uid, component);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGetInteractionVerbs(EntityUid uid, TwoWayLeverComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && args.Hands != null)
		{
			bool disabled = component.State == TwoWayLeverState.Left;
			InteractionVerb verbLeft = new InteractionVerb
			{
				Act = delegate
				{
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					TwoWayLeverComponent twoWayLeverComponent = component;
					twoWayLeverComponent.State = component.State switch
					{
						TwoWayLeverState.Middle => TwoWayLeverState.Left, 
						TwoWayLeverState.Right => TwoWayLeverState.Middle, 
						_ => throw new ArgumentOutOfRangeException(), 
					};
					StateChanged(uid, component);
				},
				Category = VerbCategory.Lever,
				Message = (disabled ? base.Loc.GetString("two-way-lever-cant") : null),
				Disabled = disabled,
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/rotate_ccw.svg.192dpi.png")),
				Text = base.Loc.GetString("two-way-lever-left")
			};
			args.Verbs.Add(verbLeft);
			disabled = component.State == TwoWayLeverState.Right;
			InteractionVerb verbRight = new InteractionVerb
			{
				Act = delegate
				{
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					TwoWayLeverComponent twoWayLeverComponent = component;
					twoWayLeverComponent.State = component.State switch
					{
						TwoWayLeverState.Left => TwoWayLeverState.Middle, 
						TwoWayLeverState.Middle => TwoWayLeverState.Right, 
						_ => throw new ArgumentOutOfRangeException(), 
					};
					StateChanged(uid, component);
				},
				Category = VerbCategory.Lever,
				Message = (disabled ? base.Loc.GetString("two-way-lever-cant") : null),
				Disabled = disabled,
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/rotate_cw.svg.192dpi.png")),
				Text = base.Loc.GetString("two-way-lever-right")
			};
			args.Verbs.Add(verbRight);
		}
	}

	private void StateChanged(EntityUid uid, TwoWayLeverComponent component)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (component.State == TwoWayLeverState.Middle)
		{
			component.NextSignalLeft = !component.NextSignalLeft;
		}
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			_appearance.SetData(uid, (Enum)TwoWayLeverVisuals.State, (object)component.State, appearance);
		}
		ProtoId<SourcePortPrototype> port = (ProtoId<SourcePortPrototype>)(component.State switch
		{
			TwoWayLeverState.Left => component.LeftPort, 
			TwoWayLeverState.Right => component.RightPort, 
			TwoWayLeverState.Middle => component.MiddlePort, 
			_ => throw new ArgumentOutOfRangeException(), 
		});
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		_signalSystem.InvokePort(uid, ProtoId<SourcePortPrototype>.op_Implicit(port));
	}
}
