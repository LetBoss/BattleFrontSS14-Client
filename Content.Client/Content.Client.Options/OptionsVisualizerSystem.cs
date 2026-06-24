using System;
using System.Collections.Generic;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;

namespace Content.Client.Options;

public sealed class OptionsVisualizerSystem : EntitySystem
{
	private static readonly (OptionVisualizerOptions, CVarDef<bool>)[] OptionVars = new(OptionVisualizerOptions, CVarDef<bool>)[2]
	{
		(OptionVisualizerOptions.Test, CCVars.DebugOptionVisualizerTest),
		(OptionVisualizerOptions.ReducedMotion, CCVars.ReducedMotion)
	};

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IReflectionManager _reflection;

	[Dependency]
	private SpriteSystem _sprite;

	private OptionVisualizerOptions _currentOptions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		(OptionVisualizerOptions, CVarDef<bool>)[] optionVars = OptionVars;
		for (int i = 0; i < optionVars.Length; i++)
		{
			CVarDef<bool> item = optionVars[i].Item2;
			EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, item, (Action<bool>)delegate
			{
				CVarChanged();
			}, false);
		}
		UpdateActiveOptions();
		((EntitySystem)this).SubscribeLocalEvent<OptionsVisualizerComponent, ComponentStartup>((ComponentEventHandler<OptionsVisualizerComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
	}

	private void CVarChanged()
	{
		UpdateActiveOptions();
		UpdateAllComponents();
	}

	private void UpdateActiveOptions()
	{
		_currentOptions = OptionVisualizerOptions.Default;
		(OptionVisualizerOptions, CVarDef<bool>)[] optionVars = OptionVars;
		for (int i = 0; i < optionVars.Length; i++)
		{
			var (optionVisualizerOptions, val) = optionVars[i];
			if (_cfg.GetCVar<bool>(val))
			{
				_currentOptions |= optionVisualizerOptions;
			}
		}
	}

	private void UpdateAllComponents()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<OptionsVisualizerComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<OptionsVisualizerComponent, SpriteComponent>();
		EntityUid uid = default(EntityUid);
		OptionsVisualizerComponent component = default(OptionsVisualizerComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref uid, ref component, ref sprite))
		{
			UpdateComponent(uid, component, sprite);
		}
	}

	private void OnComponentStartup(EntityUid uid, OptionsVisualizerComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			UpdateComponent(uid, component, sprite);
		}
	}

	private void UpdateComponent(EntityUid uid, OptionsVisualizerComponent component, SpriteComponent sprite)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Enum obj = default(Enum);
		foreach (KeyValuePair<string, OptionsVisualizerComponent.LayerDatum[]> visual in component.Visuals)
		{
			visual.Deconstruct(out var key, out var value);
			string text = key;
			OptionsVisualizerComponent.LayerDatum[] array = value;
			OptionsVisualizerComponent.LayerDatum layerDatum = null;
			value = array;
			foreach (OptionsVisualizerComponent.LayerDatum layerDatum2 in value)
			{
				if ((layerDatum2.Options & _currentOptions) == layerDatum2.Options)
				{
					layerDatum = layerDatum2;
				}
			}
			if (layerDatum != null)
			{
				int num = (_reflection.TryParseEnumReference(text, ref obj, true) ? _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), obj) : _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text));
				_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, layerDatum.Data);
			}
		}
	}
}
