// Decompiled with JetBrains decompiler
// Type: Content.Client.Smoking.BurnStateVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Smoking;

public sealed class BurnStateVisualizerSystem : VisualizerSystem<BurnStateVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    BurnStateVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    object obj;
    if (args.Sprite == null || !args.AppearanceData.TryGetValue((Enum) SmokingVisuals.Smoking, out obj))
      return;
    string str1;
    if (obj is SmokableState smokableState)
    {
      switch (smokableState)
      {
        case SmokableState.Lit:
          str1 = component.LitIcon;
          goto label_7;
        case SmokableState.Burnt:
          str1 = component.BurntIcon;
          goto label_7;
      }
    }
    str1 = component.UnlitIcon;
label_7:
    string str2 = str1;
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(str2));
  }
}
