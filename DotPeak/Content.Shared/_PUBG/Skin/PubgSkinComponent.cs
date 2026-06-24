// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.PubgSkinComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgSkinComponent : 
  Component,
  ISerializationGenerated<PubgSkinComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, List<string>> AllItems = new Dictionary<string, List<string>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, List<string>> UnlockedItems = new Dictionary<string, List<string>>()
  {
    {
      "jumpsuit",
      new List<string>()
      {
        "PubgSkinJumpsuitBaseForestCamouflage"
      }
    },
    {
      "outerClothing",
      new List<string>()
    },
    {
      "shoes",
      new List<string>() { "PubgSkinShoesBlack" }
    },
    {
      "neck",
      new List<string>()
    },
    {
      "head",
      new List<string>()
    },
    {
      "ghost",
      new List<string>() { "PubgSkinGhostDefault" }
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, int> RecipePrices = new Dictionary<string, int>()
  {
    {
      "PubgSkinJumpsuitSeniorOfficer",
      1000
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, string> CurrentOutfit = new Dictionary<string, string>()
  {
    {
      "jumpsuit",
      "PubgSkinJumpsuitBaseForestCamouflage"
    },
    {
      "outerClothing",
      string.Empty
    },
    {
      "shoes",
      "PubgSkinShoesBlack"
    },
    {
      "neck",
      ""
    },
    {
      "head",
      ""
    },
    {
      "ghost",
      "PubgSkinGhostDefault"
    },
    {
      "firstKillBanner",
      "PubgFirstKillBannerTest"
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> UnlockedEmotes = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> EquippedEmotes = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, TimeSpan> EmoteCooldowns = new Dictionary<string, TimeSpan>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgSkinComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgSkinComponent) target1;
    if (serialization.TryCustomCopy<PubgSkinComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, List<string>> target2 = (Dictionary<string, List<string>>) null;
    if (this.AllItems == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(this.AllItems, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, List<string>>>(this.AllItems, hookCtx, context);
    target.AllItems = target2;
    Dictionary<string, List<string>> target3 = (Dictionary<string, List<string>>) null;
    if (this.UnlockedItems == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(this.UnlockedItems, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, List<string>>>(this.UnlockedItems, hookCtx, context);
    target.UnlockedItems = target3;
    Dictionary<string, int> target4 = (Dictionary<string, int>) null;
    if (this.RecipePrices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.RecipePrices, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, int>>(this.RecipePrices, hookCtx, context);
    target.RecipePrices = target4;
    Dictionary<string, string> target5 = (Dictionary<string, string>) null;
    if (this.CurrentOutfit == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.CurrentOutfit, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<string, string>>(this.CurrentOutfit, hookCtx, context);
    target.CurrentOutfit = target5;
    List<string> target6 = (List<string>) null;
    if (this.UnlockedEmotes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.UnlockedEmotes, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<string>>(this.UnlockedEmotes, hookCtx, context);
    target.UnlockedEmotes = target6;
    List<string> target7 = (List<string>) null;
    if (this.EquippedEmotes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.EquippedEmotes, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<string>>(this.EquippedEmotes, hookCtx, context);
    target.EquippedEmotes = target7;
    Dictionary<string, TimeSpan> target8 = (Dictionary<string, TimeSpan>) null;
    if (this.EmoteCooldowns == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, TimeSpan>>(this.EmoteCooldowns, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<string, TimeSpan>>(this.EmoteCooldowns, hookCtx, context);
    target.EmoteCooldowns = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgSkinComponent target,
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
    PubgSkinComponent target1 = (PubgSkinComponent) target;
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
    PubgSkinComponent target1 = (PubgSkinComponent) target;
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
    PubgSkinComponent target1 = (PubgSkinComponent) target;
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
  virtual PubgSkinComponent Component.Instantiate() => new PubgSkinComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgSkinComponent_AutoState : IComponentState
  {
    public Dictionary<string, List<string>> AllItems;
    public Dictionary<string, List<string>> UnlockedItems;
    public Dictionary<string, int> RecipePrices;
    public Dictionary<string, string> CurrentOutfit;
    public List<string> UnlockedEmotes;
    public List<string> EquippedEmotes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgSkinComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgSkinComponent, ComponentGetState>(new ComponentEventRefHandler<PubgSkinComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgSkinComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgSkinComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PubgSkinComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgSkinComponent.PubgSkinComponent_AutoState()
      {
        AllItems = component.AllItems,
        UnlockedItems = component.UnlockedItems,
        RecipePrices = component.RecipePrices,
        CurrentOutfit = component.CurrentOutfit,
        UnlockedEmotes = component.UnlockedEmotes,
        EquippedEmotes = component.EquippedEmotes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgSkinComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgSkinComponent.PubgSkinComponent_AutoState current))
        return;
      component.AllItems = current.AllItems == null ? (Dictionary<string, List<string>>) null : new Dictionary<string, List<string>>((IDictionary<string, List<string>>) current.AllItems);
      component.UnlockedItems = current.UnlockedItems == null ? (Dictionary<string, List<string>>) null : new Dictionary<string, List<string>>((IDictionary<string, List<string>>) current.UnlockedItems);
      component.RecipePrices = current.RecipePrices == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.RecipePrices);
      component.CurrentOutfit = current.CurrentOutfit == null ? (Dictionary<string, string>) null : new Dictionary<string, string>((IDictionary<string, string>) current.CurrentOutfit);
      component.UnlockedEmotes = current.UnlockedEmotes == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.UnlockedEmotes);
      component.EquippedEmotes = current.EquippedEmotes == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.EquippedEmotes);
    }
  }
}
