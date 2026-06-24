// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Damage.RMCXenoDamageVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Damage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Damage;

public sealed class RMCXenoDamageVisualsSystem : VisualizerSystem<RMCXenoDamageVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RMCXenoDamageVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num1;
    int num2;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) RMCDamageVisuals.State, ref num1, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) RMCDamageVisualLayers.Base, ref num2, false))
      return;
    if (num1 == 0)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
    }
    else
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
      int num3 = component.States - num1 + 1;
      bool flag1;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag1, (AppearanceComponent) null) & flag1)
      {
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit($"{component.Prefix}_downed_{num3}"));
      }
      else
      {
        bool flag2;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Fortified, ref flag2, (AppearanceComponent) null) & flag2)
        {
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit($"{component.Prefix}_fortify_{num3}"));
        }
        else
        {
          bool flag3;
          if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent) null) & flag3)
            this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit($"{component.Prefix}_rest_{num3}"));
          else
            this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit($"{component.Prefix}_walk_{num3}"));
        }
      }
    }
  }
}
