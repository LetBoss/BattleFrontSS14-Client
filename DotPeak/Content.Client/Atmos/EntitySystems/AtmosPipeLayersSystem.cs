// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.AtmosPipeLayersSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosPipeLayersSystem : SharedAtmosPipeLayersSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IReflectionManager _reflection;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosPipeLayersComponent, AppearanceChangeEvent>(new EntityEventRefHandler<AtmosPipeLayersComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    Entity<AtmosPipeLayersComponent> ent,
    ref AppearanceChangeEvent ev)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    string str1;
    RSIResource rsiResource;
    if (this._appearance.TryGetData<string>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum) AtmosPipeLayerVisuals.Sprite, ref str1, (AppearanceComponent) null) && this._resourceCache.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, str1), ref rsiResource))
      this._sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), spriteComponent)), rsiResource.RSI);
    Dictionary<string, string> dictionary;
    if (!this._appearance.TryGetData<Dictionary<string, string>>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum) AtmosPipeLayerVisuals.SpriteLayers, ref dictionary, (AppearanceComponent) null))
      return;
    foreach ((string str2, string str3) in dictionary)
    {
      Enum @enum;
      if (this.TryParseKey(str2, out @enum))
        this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), spriteComponent)), @enum, new ResPath(str3), new RSI.StateId?());
      else
        this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), spriteComponent)), str2, new ResPath(str3), new RSI.StateId?());
    }
  }

  private bool TryParseKey(string keyString, [NotNullWhen(true)] out Enum? @enum)
  {
    return this._reflection.TryParseEnumReference(keyString, ref @enum, true);
  }
}
