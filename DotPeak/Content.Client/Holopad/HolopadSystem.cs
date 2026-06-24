// Decompiled with JetBrains decompiler
// Type: Content.Client.Holopad.HolopadSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Holopad;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Holopad;

public sealed class HolopadSystem : SharedHolopadSystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HolopadHologramComponent, ComponentStartup>(new EntityEventRefHandler<HolopadHologramComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HolopadHologramComponent, BeforePostShaderRenderEvent>(new EntityEventRefHandler<HolopadHologramComponent, BeforePostShaderRenderEvent>((object) this, __methodptr(OnShaderRender)), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<TypingChangedEvent>(new EntitySessionEventHandler<TypingChangedEvent>(this.OnTypingChanged), (Type[]) null, (Type[]) null);
  }

  private void OnComponentStartup(Entity<HolopadHologramComponent> entity, ref ComponentStartup ev)
  {
    this.UpdateHologramSprite(Entity<HolopadHologramComponent>.op_Implicit(entity), entity.Comp.LinkedEntity);
  }

  private void OnShaderRender(
    Entity<HolopadHologramComponent> entity,
    ref BeforePostShaderRenderEvent ev)
  {
    if (ev.Sprite.PostShader == null)
      return;
    this.UpdateHologramSprite(Entity<HolopadHologramComponent>.op_Implicit(entity), entity.Comp.LinkedEntity);
  }

  private void OnTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = ((EntitySessionEventArgs) ref args).SenderSession.AttachedEntity;
    if (!this.Exists(attachedEntity) || !this.HasComp<HolopadUserComponent>(attachedEntity))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new HolopadUserTypingChangedEvent(this.GetNetEntity(attachedEntity.Value, (MetaDataComponent) null), ev.State));
  }

  private void UpdateHologramSprite(EntityUid hologram, EntityUid? target)
  {
    SpriteComponent sprite;
    HolopadHologramComponent holopadHologram;
    if (!this.TryComp<SpriteComponent>(hologram, ref sprite) || !this.TryComp<HolopadHologramComponent>(hologram, ref holopadHologram))
      return;
    for (int index = sprite.AllLayers.Count<ISpriteLayer>() - 1; index >= 0; --index)
      this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), index, true);
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(target, ref spriteComponent))
    {
      HolographicAvatarComponent holographicAvatarComponent;
      if (this.TryComp<HolographicAvatarComponent>(target, ref holographicAvatarComponent) && holographicAvatarComponent.LayerData != null)
      {
        for (int index = 0; index < holographicAvatarComponent.LayerData.Length; ++index)
          this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), holographicAvatarComponent.LayerData[index], new int?(index));
      }
      else
        this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((target.Value, spriteComponent)), Entity<SpriteComponent>.op_Implicit((hologram, sprite)));
    }
    else
    {
      if (string.IsNullOrEmpty(holopadHologram.RsiPath) || string.IsNullOrEmpty(holopadHologram.RsiState))
        return;
      PrototypeLayerData prototypeLayerData = new PrototypeLayerData()
      {
        RsiPath = holopadHologram.RsiPath,
        State = holopadHologram.RsiState
      };
      this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), prototypeLayerData, new int?());
    }
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), Color.White);
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), holopadHologram.Offset);
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), 6);
    sprite.NoRotation = true;
    sprite.DirectionOverride = (Direction) 0;
    sprite.EnableDirectionOverride = true;
    for (int index = 0; index < sprite.AllLayers.Count<ISpriteLayer>(); ++index)
    {
      SpriteComponent.Layer layer;
      if (this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((hologram, sprite)), index, ref layer, false))
      {
        ProtoId<ShaderPrototype>? shaderPrototype = layer.ShaderPrototype;
        ProtoId<ShaderPrototype>? nullable = ProtoId<ShaderPrototype>.op_Implicit("DisplacedDraw");
        if ((shaderPrototype.HasValue == nullable.HasValue ? (shaderPrototype.HasValue ? (ProtoId<ShaderPrototype>.op_Inequality(shaderPrototype.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
          sprite.LayerSetShader(index, "unshaded");
      }
    }
    this.UpdateHologramShader(hologram, sprite, holopadHologram);
  }

  private void UpdateHologramShader(
    EntityUid uid,
    SpriteComponent sprite,
    HolopadHologramComponent holopadHologram)
  {
    float num = (float) sprite.AllLayers.Max<ISpriteLayer>((Func<ISpriteLayer, int>) (x => x.PixelSize.Y));
    ShaderInstance shaderInstance = this._prototypeManager.Index<ShaderPrototype>(holopadHologram.ShaderName).InstanceUnique();
    shaderInstance.SetParameter("color1", new Vector3(holopadHologram.Color1.R, holopadHologram.Color1.G, holopadHologram.Color1.B));
    shaderInstance.SetParameter("color2", new Vector3(holopadHologram.Color2.R, holopadHologram.Color2.G, holopadHologram.Color2.B));
    shaderInstance.SetParameter("alpha", holopadHologram.Alpha);
    shaderInstance.SetParameter("intensity", holopadHologram.Intensity);
    shaderInstance.SetParameter("texHeight", num);
    shaderInstance.SetParameter("t", (float) this._timing.CurTime.TotalSeconds * holopadHologram.ScrollRate);
    sprite.PostShader = shaderInstance;
    sprite.RaiseShaderEvent = true;
  }
}
