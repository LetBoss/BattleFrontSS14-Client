using System;
using Content.Shared.APC;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Power.APC;

public sealed class ApcVisualizerSystem : VisualizerSystem<ApcVisualsComponent>
{
	[Dependency]
	private SharedPointLightSystem _lights;

	protected override void OnAppearanceChange(EntityUid uid, ApcVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		int num = base.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ApcVisualLayers.InterfaceLock);
		int num2 = base.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ApcVisualLayers.Equipment);
		ApcChargeState apcChargeState = default(ApcChargeState);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<ApcChargeState>(uid, (Enum)ApcVisuals.ChargeState, ref apcChargeState, args.Component))
		{
			apcChargeState = ApcChargeState.Lack;
		}
		if (apcChargeState >= ApcChargeState.Lack && apcChargeState < ApcChargeState.NumStates)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ApcVisualLayers.ChargeState, StateId.op_Implicit(comp.ScreenPrefix + "-" + comp.ScreenSuffixes[(int)apcChargeState]));
			byte b = default(byte);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<byte>(uid, (Enum)ApcVisuals.LockState, ref b, args.Component))
			{
				for (int i = 0; i < comp.LockIndicators; i++)
				{
					int num3 = (byte)num + i;
					sbyte b2 = (sbyte)((b >> i) & 1);
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit($"{comp.LockPrefix}{i}-{comp.LockSuffixes[b2]}"));
					base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
				}
			}
			byte b3 = default(byte);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<byte>(uid, (Enum)ApcVisuals.ChannelState, ref b3, args.Component))
			{
				for (int j = 0; j < comp.ChannelIndicators; j++)
				{
					int num4 = (byte)num2 + j;
					sbyte b4 = (sbyte)((b3 >> (j << 1)) & 3);
					base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, StateId.op_Implicit($"{comp.ChannelPrefix}{j}-{comp.ChannelSuffixes[b4]}"));
					base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, true);
				}
			}
			PointLightComponent val = default(PointLightComponent);
			if (((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val))
			{
				_lights.SetColor(uid, comp.ScreenColors[(int)apcChargeState], (SharedPointLightComponent)(object)val);
			}
		}
		else
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ApcVisualLayers.ChargeState, StateId.op_Implicit(comp.EmaggedScreenState));
			for (int k = 0; k < comp.LockIndicators; k++)
			{
				int num5 = (byte)num + k;
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num5, false);
			}
			for (int l = 0; l < comp.ChannelIndicators; l++)
			{
				int num6 = (byte)num2 + l;
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num6, false);
			}
			PointLightComponent val2 = default(PointLightComponent);
			if (((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val2))
			{
				_lights.SetColor(uid, comp.EmaggedScreenColor, (SharedPointLightComponent)(object)val2);
			}
		}
	}
}
