// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Visualizers.EntityStorageVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Storage.Visualizers;

public sealed class EntityStorageVisualizerSystem : VisualizerSystem<EntityStorageVisualsComponent>
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<EntityStorageVisualsComponent, ComponentInit>(new ComponentEventHandler<EntityStorageVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(
    EntityUid uid,
    EntityStorageVisualsComponent comp,
    ComponentInit args)
  {
    if (comp.StateBaseClosed == null)
      return;
    EntityStorageVisualsComponent visualsComponent = comp;
    if (visualsComponent.StateBaseOpen == null)
      visualsComponent.StateBaseOpen = comp.StateBaseClosed;
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) StorageVisualLayers.Base, RSI.StateId.op_Implicit(comp.StateBaseClosed));
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    EntityStorageVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    int num;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) StorageVisuals.Open, ref flag, args.Component) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, ref num, false))
      return;
    if (flag)
    {
      if (comp.OpenDrawDepth.HasValue)
        this.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), comp.OpenDrawDepth.Value);
      if (comp.StateDoorOpen != null)
      {
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, RSI.StateId.op_Implicit(comp.StateDoorOpen));
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, true);
      }
      else
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, false);
      if (comp.StateBaseOpen == null)
        return;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Base, RSI.StateId.op_Implicit(comp.StateBaseOpen));
    }
    else
    {
      if (comp.ClosedDrawDepth.HasValue)
        this.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), comp.ClosedDrawDepth.Value);
      if (comp.StateDoorClosed != null)
      {
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, RSI.StateId.op_Implicit(comp.StateDoorClosed));
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, true);
      }
      else
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Door, false);
      if (comp.StateBaseClosed == null)
        return;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageVisualLayers.Base, RSI.StateId.op_Implicit(comp.StateBaseClosed));
    }
  }
}
