// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.IconSmoothing.IconSmoothRandomSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.IconSmoothing;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.IconSmoothing;

public sealed class IconSmoothRandomSystem : EntitySystem
{
  [Dependency]
  private IReflectionManager _reflection;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<RandomSpriteComponent> _randomSpriteQuery;
  private EntityQuery<SpriteComponent> _spriteQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._randomSpriteQuery = this.GetEntityQuery<RandomSpriteComponent>();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IconSmoothRandomComponent, IconSmoothingUpdatedEvent>(new EntityEventRefHandler<IconSmoothRandomComponent, IconSmoothingUpdatedEvent>((object) this, __methodptr(OnOverrideIconSmoothingUpdated)), (Type[]) null, (Type[]) null);
  }

  private void OnOverrideIconSmoothingUpdated(
    Entity<IconSmoothRandomComponent> ent,
    ref IconSmoothingUpdatedEvent args)
  {
    RandomSpriteComponent randomSpriteComponent;
    SpriteComponent spriteComponent;
    if (!this._randomSpriteQuery.TryGetComponent(Entity<IconSmoothRandomComponent>.op_Implicit(ent), ref randomSpriteComponent) || !this._spriteQuery.TryGetComponent(Entity<IconSmoothRandomComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    foreach (KeyValuePair<string, (string State, Color? Color)> keyValuePair in randomSpriteComponent.Selected)
    {
      Enum @enum;
      int result;
      if (this._reflection.TryParseEnumReference(keyValuePair.Key, ref @enum, true))
      {
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), @enum, ref result, true))
          continue;
      }
      else if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), keyValuePair.Key, ref result, false))
      {
        string key = keyValuePair.Key;
        if (key == null || !int.TryParse(key, out result))
        {
          this.Log.Error($"Invalid key `{keyValuePair.Key}` for entity with random sprite {this.ToPrettyString(new EntityUid?(Entity<IconSmoothRandomComponent>.op_Implicit(ent)), (MetaDataComponent) null)}");
          continue;
        }
      }
      string name = this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), result).Name;
      if (name != null && ent.Comp.Overrides.Contains(name))
      {
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), result, RSI.StateId.op_Implicit(keyValuePair.Value.State));
        this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), result, keyValuePair.Value.Color ?? Color.White);
      }
    }
  }
}
