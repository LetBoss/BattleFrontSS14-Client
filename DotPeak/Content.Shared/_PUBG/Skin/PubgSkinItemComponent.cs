// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.PubgSkinItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ghost;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgSkinItemComponent : 
  Component,
  ISerializationGenerated<PubgSkinItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool IsBase;
  [DataField(null, false, 1, false, false, null)]
  public SkinRarity Rarity;
  [DataField(null, false, 1, false, false, null)]
  public int CraftPrice;
  [DataField(null, false, 1, false, false, null)]
  public int SellPrice = 100;
  [DataField(null, false, 1, false, false, null)]
  public int PremiumPrice;
  [DataField(null, false, 1, false, false, null)]
  public int CollectibleLimit;
  [DataField(null, false, 1, false, false, null)]
  public List<PubgSkinShopOfferPrototype> ShopOffers = new List<PubgSkinShopOfferPrototype>();
  [DataField(null, false, 1, false, false, null)]
  public string Category = "";
  [DataField(null, false, 1, false, false, null)]
  public bool Disabled;
  [DataField(null, false, 1, false, false, null)]
  public bool CanDropFromCase;
  [DataField(null, false, 1, false, false, null)]
  public string Description = "";
  [DataField(null, false, 1, false, false, null)]
  public List<string> Tags = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public int? ExpiresIn;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<GhostComponent>? GhostPrototype;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<GhostComponent>? AdminGhostPrototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgSkinItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgSkinItemComponent) target1;
    if (serialization.TryCustomCopy<PubgSkinItemComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBase, ref target2, hookCtx, false, context))
      target2 = this.IsBase;
    target.IsBase = target2;
    SkinRarity target3 = SkinRarity.Common;
    if (!serialization.TryCustomCopy<SkinRarity>(this.Rarity, ref target3, hookCtx, false, context))
      target3 = this.Rarity;
    target.Rarity = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.CraftPrice, ref target4, hookCtx, false, context))
      target4 = this.CraftPrice;
    target.CraftPrice = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.SellPrice, ref target5, hookCtx, false, context))
      target5 = this.SellPrice;
    target.SellPrice = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.PremiumPrice, ref target6, hookCtx, false, context))
      target6 = this.PremiumPrice;
    target.PremiumPrice = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.CollectibleLimit, ref target7, hookCtx, false, context))
      target7 = this.CollectibleLimit;
    target.CollectibleLimit = target7;
    List<PubgSkinShopOfferPrototype> target8 = (List<PubgSkinShopOfferPrototype>) null;
    if (this.ShopOffers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<PubgSkinShopOfferPrototype>>(this.ShopOffers, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<PubgSkinShopOfferPrototype>>(this.ShopOffers, hookCtx, context);
    target.ShopOffers = target8;
    string target9 = (string) null;
    if (this.Category == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Category, ref target9, hookCtx, false, context))
      target9 = this.Category;
    target.Category = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Disabled, ref target10, hookCtx, false, context))
      target10 = this.Disabled;
    target.Disabled = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDropFromCase, ref target11, hookCtx, false, context))
      target11 = this.CanDropFromCase;
    target.CanDropFromCase = target11;
    string target12 = (string) null;
    if (this.Description == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Description, ref target12, hookCtx, false, context))
      target12 = this.Description;
    target.Description = target12;
    List<string> target13 = (List<string>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Tags, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<string>>(this.Tags, hookCtx, context);
    target.Tags = target13;
    int? target14 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.ExpiresIn, ref target14, hookCtx, false, context))
      target14 = this.ExpiresIn;
    target.ExpiresIn = target14;
    EntProtoId<GhostComponent>? target15 = new EntProtoId<GhostComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<GhostComponent>?>(this.GhostPrototype, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId<GhostComponent>?>(this.GhostPrototype, hookCtx, context);
    target.GhostPrototype = target15;
    EntProtoId<GhostComponent>? target16 = new EntProtoId<GhostComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<GhostComponent>?>(this.AdminGhostPrototype, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId<GhostComponent>?>(this.AdminGhostPrototype, hookCtx, context);
    target.AdminGhostPrototype = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgSkinItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgSkinItemComponent target1 = (PubgSkinItemComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgSkinItemComponent target1 = (PubgSkinItemComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgSkinItemComponent target1 = (PubgSkinItemComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PubgSkinItemComponent Component.Instantiate() => new PubgSkinItemComponent();
}
