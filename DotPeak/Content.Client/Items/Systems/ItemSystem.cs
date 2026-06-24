// Decompiled with JetBrains decompiler
// Type: Content.Client.Items.Systems.ItemSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Items.Systems;

public sealed class ItemSystem : SharedItemSystem
{
  [Dependency]
  private IResourceCache _resCache;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemComponent, GetInhandVisualsEvent>(new ComponentEventHandler<ItemComponent, GetInhandVisualsEvent>((object) this, __methodptr(OnGetVisuals)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpriteComponent, GotEquippedEvent>(new ComponentEventHandler<SpriteComponent, GotEquippedEvent>((object) this, __methodptr(OnEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpriteComponent, GotUnequippedEvent>(new ComponentEventHandler<SpriteComponent, GotUnequippedEvent>((object) this, __methodptr(OnUnequipped)), (Type[]) null, (Type[]) null);
  }

  private void OnUnequipped(EntityUid uid, SpriteComponent component, GotUnequippedEvent args)
  {
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, component)), true);
  }

  private void OnEquipped(EntityUid uid, SpriteComponent component, GotEquippedEvent args)
  {
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, component)), false);
  }

  public override void VisualsChanged(EntityUid uid)
  {
    BaseContainer baseContainer;
    if (!this.Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
      return;
    this.RaiseLocalEvent<VisualsChangedEvent>(baseContainer.Owner, new VisualsChangedEvent(this.GetNetEntity(uid, (MetaDataComponent) null), baseContainer.ID), false);
  }

  private void OnGetVisuals(EntityUid uid, ItemComponent item, GetInhandVisualsEvent args)
  {
    string defaultKey = "inhand-" + args.Location.ToString().ToLowerInvariant();
    List<PrototypeLayerData> result;
    if (!item.InhandVisuals.TryGetValue(args.Location, out result) && !this.TryGetDefaultVisuals(uid, item, defaultKey, out result))
      return;
    int num = 0;
    foreach (PrototypeLayerData prototypeLayerData in result)
    {
      HashSet<string> mapKeys = prototypeLayerData.MapKeys;
      string str1 = mapKeys != null ? mapKeys.FirstOrDefault<string>() : (string) null;
      if (str1 == null)
      {
        string str2;
        if (num != 0)
          str2 = $"{defaultKey}-{num}";
        else
          str2 = defaultKey;
        str1 = str2;
        ++num;
      }
      args.Layers.Add((str1, prototypeLayerData));
    }
  }

  private bool TryGetDefaultVisuals(
    EntityUid uid,
    ItemComponent item,
    string defaultKey,
    [NotNullWhen(true)] out List<PrototypeLayerData>? result)
  {
    result = (List<PrototypeLayerData>) null;
    RSI rsi = (RSI) null;
    if (item.RsiPath != null)
    {
      rsi = this._resCache.GetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, item.RsiPath), true).RSI;
    }
    else
    {
      SpriteComponent spriteComponent;
      if (this.TryComp<SpriteComponent>(uid, ref spriteComponent))
        rsi = spriteComponent.BaseRSI;
    }
    if (rsi == null)
      return false;
    string str = item.HeldPrefix == null ? defaultKey : $"{item.HeldPrefix}-{defaultKey}";
    RSI.State state;
    if (!rsi.TryGetState(RSI.StateId.op_Implicit(str), ref state))
      return false;
    result = new List<PrototypeLayerData>()
    {
      new PrototypeLayerData()
      {
        RsiPath = rsi.Path.ToString(),
        State = str,
        MapKeys = new HashSet<string>() { str }
      }
    };
    return true;
  }
}
