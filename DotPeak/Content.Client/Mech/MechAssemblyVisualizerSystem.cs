// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.MechAssemblyVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mech;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Mech;

public sealed class MechAssemblyVisualizerSystem : VisualizerSystem<MechAssemblyVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    MechAssemblyVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) MechAssemblyVisuals.State, ref num, args.Component))
      return;
    string str = component.StatePrefix + num.ToString();
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(str));
  }
}
