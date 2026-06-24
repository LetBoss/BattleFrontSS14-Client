// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Entrenching.EntrenchingToolVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Entrenching;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Entrenching;

public sealed class EntrenchingToolVisualsSystem : EntitySystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntrenchingToolComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<EntrenchingToolComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntrenchingToolComponent, AppearanceChangeEvent>(new EntityEventRefHandler<EntrenchingToolComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    Entity<EntrenchingToolComponent> tool,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateVisuals(tool);
  }

  private void OnAppearanceChange(
    Entity<EntrenchingToolComponent> tool,
    ref AppearanceChangeEvent args)
  {
    this.UpdateVisuals(tool);
  }

  private void UpdateVisuals(Entity<EntrenchingToolComponent> tool)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref spriteComponent))
      return;
    bool flag;
    if (((SharedAppearanceSystem) this._appearance).TryGetData<bool>(Entity<EntrenchingToolComponent>.op_Implicit(tool), (Enum) ToggleableVisuals.Enabled, ref flag, (AppearanceComponent) null) & flag)
    {
      int num1;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Base, ref num1, false))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num1, true);
      int num2;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Folded, ref num2, false))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num2, false);
      if (tool.Comp.TotalLayers > 0)
      {
        int num3;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Dirt, ref num3, false))
        {
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num3, true);
          this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num3, Color.FromHex((ReadOnlySpan<char>) "#C04000", new Color?()));
        }
        else
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num3, false);
      }
      else
      {
        int num4;
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Dirt, ref num4, false))
          return;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num4, false);
      }
    }
    else
    {
      int num5;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Base, ref num5, false))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num5, false);
      int num6;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Folded, ref num6, false))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num6, true);
      int num7;
      if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), (Enum) EntrenchingToolComponentVisualLayers.Dirt, ref num7, false))
        return;
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, spriteComponent)), num7, false);
    }
  }
}
