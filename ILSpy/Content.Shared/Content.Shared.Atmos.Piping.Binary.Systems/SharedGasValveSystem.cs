using System;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.Piping.Binary.Systems;

public abstract class SharedGasValveSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasValveComponent, ComponentStartup>((EntityEventRefHandler<GasValveComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasValveComponent, ActivateInWorldEvent>((EntityEventRefHandler<GasValveComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasValveComponent, ExaminedEvent>((EntityEventRefHandler<GasValveComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<GasValveComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Set(ent.Owner, ent.Comp, ent.Comp.Open);
	}

	public virtual void Set(EntityUid uid, GasValveComponent component, bool value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		component.Open = value;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			_appearance.SetData(uid, (Enum)FilterVisuals.Enabled, (object)component.Open, appearance);
		}
	}

	public void Toggle(EntityUid uid, GasValveComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Set(uid, component, !component.Open);
	}

	private void OnActivate(Entity<GasValveComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			Toggle(ent.Owner, ent.Comp);
			_audio.PlayPredicted(ent.Comp.ValveSound, ent.Owner, (EntityUid?)args.User, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.25f));
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnExamined(Entity<GasValveComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GasValveComponent valve = ent.Comp;
		string str = default(string);
		if (((EntitySystem)this).Transform(Entity<GasValveComponent>.op_Implicit(ent)).Anchored && base.Loc.TryGetString("gas-valve-system-examined", ref str, (ValueTuple<string, object>)("statusColor", valve.Open ? "green" : "orange"), (ValueTuple<string, object>)("open", valve.Open)))
		{
			args.PushMarkup(str);
		}
	}
}
