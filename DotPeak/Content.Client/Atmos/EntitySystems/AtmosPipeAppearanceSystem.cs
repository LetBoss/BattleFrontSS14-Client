// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.AtmosPipeAppearanceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.SubFloor;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosPipeAppearanceSystem : SharedAtmosPipeAppearanceSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PipeAppearanceComponent, ComponentInit>(new ComponentEventHandler<PipeAppearanceComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PipeAppearanceComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PipeAppearanceComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChanged)), (Type[]) null, new Type[1]
    {
      typeof (SubFloorHideSystem)
    });
  }

  private void OnInit(EntityUid uid, PipeAppearanceComponent component, ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    int numberOfPipeLayers = this.GetNumberOfPipeLayers(uid, out AtmosPipeLayersComponent _);
    foreach (AtmosPipeAppearanceSystem.PipeConnectionLayer layer in Enum.GetValues<AtmosPipeAppearanceSystem.PipeConnectionLayer>())
    {
      for (byte index = 0; (int) index < numberOfPipeLayers; ++index)
      {
        string str = layer.ToString() + index.ToString();
        int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), str);
        this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, component.Sprite[(int) index].RsiPath, new RSI.StateId?());
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, RSI.StateId.op_Implicit(component.Sprite[(int) index].RsiState));
        this._sprite.LayerSetDirOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, this.ToOffset(layer));
      }
    }
  }

  private void HideAllPipeConnection(
    Entity<SpriteComponent> entity,
    AtmosPipeLayersComponent? atmosPipeLayers,
    int numberOfPipeLayers)
  {
    SpriteComponent comp = entity.Comp;
    foreach (AtmosPipeAppearanceSystem.PipeConnectionLayer pipeConnectionLayer in Enum.GetValues<AtmosPipeAppearanceSystem.PipeConnectionLayer>())
    {
      for (byte index = 0; (int) index < numberOfPipeLayers; ++index)
      {
        string str = pipeConnectionLayer.ToString() + index.ToString();
        int num;
        if (this._sprite.LayerMapTryGet(entity.AsNullable(), str, ref num, false))
          comp[num].Visible = false;
      }
    }
  }

  private void OnAppearanceChanged(
    EntityUid uid,
    PipeAppearanceComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null || !args.Sprite.Visible)
      return;
    AtmosPipeLayersComponent atmosPipeLayers;
    int numberOfPipeLayers = this.GetNumberOfPipeLayers(uid, out atmosPipeLayers);
    int num1;
    if (!this._appearance.TryGetData<int>(uid, (Enum) PipeVisuals.VisualState, ref num1, args.Component))
    {
      this.HideAllPipeConnection(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), atmosPipeLayers, numberOfPipeLayers);
    }
    else
    {
      Color white;
      if (!this._appearance.TryGetData<Color>(uid, (Enum) PipeColorVisuals.Color, ref white, args.Component))
        white = Color.White;
      for (byte index = 0; (int) index < numberOfPipeLayers; ++index)
      {
        PipeDirection pipeDirection = ((PipeDirection) (15 & num1 >> 4 * (int) index)).RotatePipeDirection(Angle.op_Implicit(Angle.op_UnaryNegation(this.Transform(uid).LocalRotation)));
        foreach (AtmosPipeAppearanceSystem.PipeConnectionLayer pipeConnectionLayer in Enum.GetValues<AtmosPipeAppearanceSystem.PipeConnectionLayer>())
        {
          string str = pipeConnectionLayer.ToString() + index.ToString();
          int num2;
          if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), str, ref num2, false))
          {
            ISpriteLayer ispriteLayer = args.Sprite[num2];
            PipeDirection other = (PipeDirection) pipeConnectionLayer;
            bool flag = pipeDirection.HasDirection(other);
            ispriteLayer.Visible &= flag;
            if (flag)
              ispriteLayer.Color = white;
          }
        }
      }
    }
  }

  private SpriteComponent.DirectionOffset ToOffset(
    AtmosPipeAppearanceSystem.PipeConnectionLayer layer)
  {
    SpriteComponent.DirectionOffset offset;
    switch (layer)
    {
      case AtmosPipeAppearanceSystem.PipeConnectionLayer.NorthConnection:
        offset = (SpriteComponent.DirectionOffset) 3;
        break;
      case AtmosPipeAppearanceSystem.PipeConnectionLayer.WestConnection:
        offset = (SpriteComponent.DirectionOffset) 1;
        break;
      case AtmosPipeAppearanceSystem.PipeConnectionLayer.EastConnection:
        offset = (SpriteComponent.DirectionOffset) 2;
        break;
      default:
        offset = (SpriteComponent.DirectionOffset) 0;
        break;
    }
    return offset;
  }

  private enum PipeConnectionLayer : byte
  {
    NorthConnection = 1,
    SouthConnection = 2,
    WestConnection = 4,
    EastConnection = 8,
  }
}
