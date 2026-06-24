using System;
using Content.Client.Light.Components;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Light.EntitySystems;

public sealed class ExpendableLightVisualizerSystem : VisualizerSystem<ExpendableLightComponent>
{
	[Dependency]
	private PointLightSystem _pointLightSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private LightBehaviorSystem _lightBehavior;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExpendableLightComponent, ComponentShutdown>((ComponentEventHandler<ExpendableLightComponent, ComponentShutdown>)OnLightShutdown, (Type[])null, (Type[])null);
	}

	private void OnLightShutdown(EntityUid uid, ExpendableLightComponent component, ComponentShutdown args)
	{
		component.PlayingStream = _audioSystem.Stop(component.PlayingStream, (AudioComponent)null);
	}

	protected override void OnAppearanceChange(EntityUid uid, ExpendableLightComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		LightBehaviourComponent item = default(LightBehaviourComponent);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)ExpendableLightVisuals.Behavior, ref text, args.Component) && ((EntitySystem)this).TryComp<LightBehaviourComponent>(uid, ref item))
		{
			_lightBehavior.StopLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, item)));
			PointLightComponent val = default(PointLightComponent);
			if (!string.IsNullOrEmpty(text))
			{
				_lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, item)), text);
			}
			else if (((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val))
			{
				((SharedPointLightSystem)_pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
			}
		}
		ExpendableLightState expendableLightState = default(ExpendableLightState);
		if (args.Sprite == null || !base.SpriteSystem.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Overlay) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<ExpendableLightState>(uid, (Enum)ExpendableLightVisuals.State, ref expendableLightState, args.Component))
		{
			return;
		}
		int num = default(int);
		switch (expendableLightState)
		{
		case ExpendableLightState.Lit:
			_audioSystem.Stop(comp.PlayingStream, (AudioComponent)null);
			comp.PlayingStream = _audioSystem.PlayPvs(comp.LoopedSound, uid, (AudioParams?)null)?.Item1;
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Overlay, ref num, true))
			{
				if (!string.IsNullOrWhiteSpace(comp.IconStateLit))
				{
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(comp.IconStateLit));
				}
				if (!string.IsNullOrWhiteSpace(comp.SpriteShaderLit))
				{
					args.Sprite.LayerSetShader(num, comp.SpriteShaderLit);
				}
				else
				{
					args.Sprite.LayerSetShader(num, (ShaderInstance)null, (string)null);
				}
				if (comp.GlowColorLit.HasValue)
				{
					base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, comp.GlowColorLit.Value);
				}
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
			}
			if (comp.GlowColorLit.HasValue)
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Glow, comp.GlowColorLit.Value);
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Glow, true);
			break;
		case ExpendableLightState.Dead:
			comp.PlayingStream = _audioSystem.Stop(comp.PlayingStream, (AudioComponent)null);
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Overlay, ref num, true))
			{
				if (!string.IsNullOrWhiteSpace(comp.IconStateSpent))
				{
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(comp.IconStateSpent));
				}
				if (!string.IsNullOrWhiteSpace(comp.SpriteShaderSpent))
				{
					args.Sprite.LayerSetShader(num, comp.SpriteShaderSpent);
				}
				else
				{
					args.Sprite.LayerSetShader(num, (ShaderInstance)null, (string)null);
				}
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ExpendableLightVisualLayers.Glow, false);
			break;
		}
	}
}
