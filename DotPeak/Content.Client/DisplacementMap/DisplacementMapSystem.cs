// Decompiled with JetBrains decompiler
// Type: Content.Client.DisplacementMap.DisplacementMapSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.DisplacementMap;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.DisplacementMap;

public sealed class DisplacementMapSystem : EntitySystem
{
  [Dependency]
  private ISerializationManager _serialization;
  [Dependency]
  private SpriteSystem _sprite;

  public bool TryAddDisplacement(
    DisplacementData data,
    Entity<SpriteComponent> sprite,
    int index,
    object key,
    out string displacementKey)
  {
    displacementKey = $"{key}-displacement";
    if (key.ToString() == null)
      return false;
    if (data.ShaderOverride != null)
      sprite.Comp.LayerSetShader(index, data.ShaderOverride);
    this._sprite.RemoveLayer(sprite.AsNullable(), displacementKey, false);
    foreach (KeyValuePair<int, PrototypeLayerData> sizeMap in data.SizeMaps)
    {
      PrototypeLayerData prototypeLayerData = sizeMap.Value;
      if (prototypeLayerData.CopyToShaderParameters == null)
        prototypeLayerData.CopyToShaderParameters = new PrototypeCopyToShaderParameters()
        {
          LayerKey = "dummy",
          ParameterTexture = "displacementMap",
          ParameterUV = "displacementUV"
        };
    }
    if (!data.SizeMaps.ContainsKey(32 /*0x20*/))
    {
      this.Log.Error($"DISPLACEMENT: {displacementKey} don't have 32x32 default displacement map");
      return false;
    }
    PrototypeLayerData prototypeLayerData1 = data.SizeMaps[32 /*0x20*/];
    RSI effectiveRsi = this._sprite.LayerGetEffectiveRsi(sprite.AsNullable(), index);
    if (effectiveRsi != null)
    {
      if (effectiveRsi.Size.X != effectiveRsi.Size.Y)
        this.Log.Warning($"DISPLACEMENT: {displacementKey} has a resolution that is not 1:1, things can look crooked");
      int x = effectiveRsi.Size.X;
      PrototypeLayerData prototypeLayerData2;
      if (data.SizeMaps.TryGetValue(x, out prototypeLayerData2))
        prototypeLayerData1 = prototypeLayerData2;
    }
    PrototypeLayerData copy = this._serialization.CreateCopy<PrototypeLayerData>(prototypeLayerData1, (ISerializationContext) null, false, true);
    copy.CopyToShaderParameters.LayerKey = key.ToString() ?? "this is impossible";
    this._sprite.AddLayer(sprite.AsNullable(), copy, new int?(index));
    this._sprite.LayerMapSet(sprite.AsNullable(), displacementKey, index);
    return true;
  }

  [Obsolete("Use the Entity<SpriteComponent> overload")]
  public bool TryAddDisplacement(
    DisplacementData data,
    SpriteComponent sprite,
    int index,
    object key,
    out string displacementKey)
  {
    return this.TryAddDisplacement(data, Entity<SpriteComponent>.op_Implicit((((Component) sprite).Owner, sprite)), index, key, out displacementKey);
  }
}
