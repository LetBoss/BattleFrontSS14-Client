// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Monitor.AtmosAlarmableVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Monitor;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.Monitor;

public sealed class AtmosAlarmableVisualsSystem : VisualizerSystem<AtmosAlarmableVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    AtmosAlarmableVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    int num1;
    object obj1;
    if (args.Sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.LayerMap, ref num1, false) || !args.AppearanceData.TryGetValue((Enum) PowerDeviceVisuals.Powered, out obj1) || !(obj1 is bool flag))
      return;
    if (component.HideOnDepowered != null)
    {
      foreach (string str in component.HideOnDepowered)
      {
        int num2;
        if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), str, ref num2, false))
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag);
      }
    }
    if (component.SetOnDepowered != null && !flag)
    {
      foreach ((string key, string str) in component.SetOnDepowered)
      {
        int num3;
        if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), key, ref num3, false))
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, new RSI.StateId(str));
      }
    }
    object obj2;
    string str1;
    if (((!args.AppearanceData.TryGetValue((Enum) AtmosMonitorVisuals.AlarmType, out obj2) ? 0 : (!(obj2 is AtmosAlarmType key1) ? 0 : 1)) & (flag ? 1 : 0)) == 0 || !component.AlarmStates.TryGetValue(key1, out str1))
      return;
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, new RSI.StateId(str1));
  }
}
