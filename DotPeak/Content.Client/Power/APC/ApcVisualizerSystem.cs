// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.APC.ApcVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.APC;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Power.APC;

public sealed class ApcVisualizerSystem : VisualizerSystem<ApcVisualsComponent>
{
  [Dependency]
  private SharedPointLightSystem _lights;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ApcVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    int num1 = this.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ApcVisualLayers.InterfaceLock);
    int num2 = this.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ApcVisualLayers.Equipment);
    ApcChargeState index1;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<ApcChargeState>(uid, (Enum) ApcVisuals.ChargeState, ref index1, args.Component))
      index1 = ApcChargeState.Lack;
    if (index1 >= ApcChargeState.Lack && index1 < ApcChargeState.NumStates)
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ApcVisualLayers.ChargeState, RSI.StateId.op_Implicit($"{comp.ScreenPrefix}-{comp.ScreenSuffixes[(int) index1]}"));
      byte num3;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<byte>(uid, (Enum) ApcVisuals.LockState, ref num3, args.Component))
      {
        for (int index2 = 0; index2 < (int) comp.LockIndicators; ++index2)
        {
          int num4 = (int) (byte) num1 + index2;
          sbyte index3 = (sbyte) ((int) num3 >> index2 & 1);
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, RSI.StateId.op_Implicit($"{comp.LockPrefix}{index2}-{comp.LockSuffixes[(int) index3]}"));
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, true);
        }
      }
      byte num5;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<byte>(uid, (Enum) ApcVisuals.ChannelState, ref num5, args.Component))
      {
        for (int index4 = 0; index4 < (int) comp.ChannelIndicators; ++index4)
        {
          int num6 = (int) (byte) num2 + index4;
          sbyte index5 = (sbyte) ((int) num5 >> (index4 << 1) & 3);
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num6, RSI.StateId.op_Implicit($"{comp.ChannelPrefix}{index4}-{comp.ChannelSuffixes[(int) index5]}"));
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num6, true);
        }
      }
      PointLightComponent pointLightComponent;
      if (!((EntitySystem) this).TryComp<PointLightComponent>(uid, ref pointLightComponent))
        return;
      this._lights.SetColor(uid, comp.ScreenColors[(int) index1], (SharedPointLightComponent) pointLightComponent);
    }
    else
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ApcVisualLayers.ChargeState, RSI.StateId.op_Implicit(comp.EmaggedScreenState));
      for (int index6 = 0; index6 < (int) comp.LockIndicators; ++index6)
      {
        int num7 = (int) (byte) num1 + index6;
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num7, false);
      }
      for (int index7 = 0; index7 < (int) comp.ChannelIndicators; ++index7)
      {
        int num8 = (int) (byte) num2 + index7;
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num8, false);
      }
      PointLightComponent pointLightComponent;
      if (!((EntitySystem) this).TryComp<PointLightComponent>(uid, ref pointLightComponent))
        return;
      this._lights.SetColor(uid, comp.EmaggedScreenColor, (SharedPointLightComponent) pointLightComponent);
    }
  }
}
