// Decompiled with JetBrains decompiler
// Type: Content.Client.Botany.PlantHolderVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Botany;

public sealed class PlantHolderVisualizerSystem : VisualizerSystem<PlantHolderVisualsComponent>
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<PlantHolderVisualsComponent, ComponentInit>(new ComponentEventHandler<PlantHolderVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(
    EntityUid uid,
    PlantHolderVisualsComponent component,
    ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) PlantHolderLayers.Plant);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) PlantHolderLayers.Plant, false);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PlantHolderVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    string str1;
    string str2;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) PlantHolderVisuals.PlantRsi, ref str1, args.Component) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) PlantHolderVisuals.PlantState, ref str2, args.Component))
      return;
    bool flag = !string.IsNullOrWhiteSpace(str2);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PlantHolderLayers.Plant, flag);
    if (!flag)
      return;
    this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PlantHolderLayers.Plant, new ResPath(str1), new RSI.StateId?());
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PlantHolderLayers.Plant, RSI.StateId.op_Implicit(str2));
  }
}
