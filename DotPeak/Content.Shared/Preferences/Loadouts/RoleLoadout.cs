// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.RoleLoadout
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Preferences.Loadouts.Effects;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Preferences.Loadouts;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class RoleLoadout : 
  IEquatable<RoleLoadout>,
  ISerializationGenerated<RoleLoadout>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RoleLoadoutPrototype> Role;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>> SelectedLoadouts = new Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>();
  public string? EntityName;
  public int? Points;

  public RoleLoadout(ProtoId<RoleLoadoutPrototype> role) => this.Role = role;

  public RoleLoadout Clone()
  {
    RoleLoadout roleLoadout = new RoleLoadout(this.Role);
    foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> selectedLoadout in this.SelectedLoadouts)
      roleLoadout.SelectedLoadouts.Add(selectedLoadout.Key, new List<Loadout>((IEnumerable<Loadout>) selectedLoadout.Value));
    roleLoadout.EntityName = this.EntityName;
    roleLoadout.Points = this.Points;
    return roleLoadout;
  }

  public void EnsureValid(
    HumanoidCharacterProfile profile,
    ICommonSession session,
    IDependencyCollection collection)
  {
    ValueList<string> valueList = new ValueList<string>();
    IPrototypeManager prototypeManager = collection.Resolve<IPrototypeManager>();
    IConfigurationManager configurationManager = collection.Resolve<IConfigurationManager>();
    RoleLoadoutPrototype prototype1;
    if (!prototypeManager.TryIndex<RoleLoadoutPrototype>(this.Role, out prototype1))
    {
      this.EntityName = (string) null;
      this.SelectedLoadouts.Clear();
    }
    else
    {
      if (!prototype1.CanCustomizeName)
        this.EntityName = (string) null;
      if (this.EntityName != null)
      {
        string str = this.EntityName.Trim();
        int cvar = configurationManager.GetCVar<int>(CCVars.MaxNameLength);
        if (str.Length > cvar)
          this.EntityName = str.Substring(0, cvar);
        if (str.Length == 0)
          this.EntityName = (string) null;
      }
      foreach (ProtoId<LoadoutGroupPrototype> group in prototype1.Groups)
      {
        if (!this.SelectedLoadouts.ContainsKey(group))
          this.SelectedLoadouts[group] = new List<Loadout>();
      }
      this.Points = prototype1.Points;
      foreach ((ProtoId<LoadoutGroupPrototype> protoId, List<Loadout> loadoutList1) in this.SelectedLoadouts)
      {
        if (!prototype1.Groups.Contains(protoId))
        {
          valueList.Add((string) protoId);
        }
        else
        {
          LoadoutGroupPrototype prototype2;
          if (!prototypeManager.TryIndex<LoadoutGroupPrototype>(protoId, out prototype2))
          {
            valueList.Add((string) protoId);
          }
          else
          {
            List<Loadout> loadoutList2 = loadoutList1.Slice(0, Math.Min(loadoutList1.Count, prototype2.MaxLimit));
            FormattedMessage reason;
            for (int index = loadoutList2.Count - 1; index >= 0; --index)
            {
              Loadout loadout = loadoutList2[index];
              LoadoutPrototype prototype3;
              if (!prototypeManager.TryIndex<LoadoutPrototype>(loadout.Prototype, out prototype3))
                loadoutList2.RemoveAt(index);
              else if (!prototype2.Loadouts.Contains(loadout.Prototype))
                loadoutList2.RemoveAt(index);
              else if (!this.IsValid(profile, session, loadout.Prototype, collection, out reason))
                loadoutList2.RemoveAt(index);
              else
                this.Apply(prototype3);
            }
            if (loadoutList2.Count < prototype2.MinLimit)
            {
              foreach (ProtoId<LoadoutPrototype> loadout1 in prototype2.Loadouts)
              {
                if (loadoutList2.Count < prototype2.MinLimit)
                {
                  LoadoutPrototype prototype4;
                  if (prototypeManager.TryIndex<LoadoutPrototype>(loadout1, out prototype4))
                  {
                    Loadout loadout2 = new Loadout()
                    {
                      Prototype = (ProtoId<LoadoutPrototype>) prototype4.ID
                    };
                    if (!loadoutList2.Contains(loadout2) && this.IsValid(profile, session, loadout2.Prototype, collection, out reason))
                    {
                      loadoutList2.Add(loadout2);
                      this.Apply(prototype4);
                    }
                  }
                }
                else
                  break;
              }
            }
            this.SelectedLoadouts[protoId] = loadoutList2;
          }
        }
      }
      foreach (string key in valueList)
        this.SelectedLoadouts.Remove((ProtoId<LoadoutGroupPrototype>) key);
    }
  }

  private void Apply(LoadoutPrototype loadoutProto)
  {
    foreach (LoadoutEffect effect in loadoutProto.Effects)
      effect.Apply(this);
    if (!loadoutProto.Cost.HasValue || !this.Points.HasValue)
      return;
    int? points = this.Points;
    int? cost = loadoutProto.Cost;
    this.Points = points.HasValue & cost.HasValue ? new int?(points.GetValueOrDefault() - cost.GetValueOrDefault()) : new int?();
  }

  public void SetDefault(
    HumanoidCharacterProfile? profile,
    ICommonSession? session,
    IPrototypeManager protoManager,
    bool force = false)
  {
    if (profile == null)
      return;
    if (force)
      this.SelectedLoadouts.Clear();
    IDependencyCollection instance = IoCManager.Instance;
    RoleLoadoutPrototype loadoutPrototype = protoManager.Index<RoleLoadoutPrototype>(this.Role);
    for (int index = loadoutPrototype.Groups.Count - 1; index >= 0; --index)
    {
      ProtoId<LoadoutGroupPrototype> group = loadoutPrototype.Groups[index];
      LoadoutGroupPrototype prototype1;
      if (protoManager.TryIndex<LoadoutGroupPrototype>(group, out prototype1) && !this.SelectedLoadouts.ContainsKey(group))
      {
        List<Loadout> loadoutList = new List<Loadout>();
        this.SelectedLoadouts[group] = loadoutList;
        this.Points = loadoutPrototype.Points;
        if (prototype1.MinLimit > 0)
        {
          foreach (ProtoId<LoadoutPrototype> loadout1 in prototype1.Loadouts)
          {
            if (loadoutList.Count < prototype1.MinLimit)
            {
              LoadoutPrototype prototype2;
              if (protoManager.TryIndex<LoadoutPrototype>(loadout1, out prototype2))
              {
                Loadout loadout2 = new Loadout()
                {
                  Prototype = (ProtoId<LoadoutPrototype>) prototype2.ID
                };
                if (this.IsValid(profile, session, loadout2.Prototype, instance, out FormattedMessage _))
                {
                  loadoutList.Add(loadout2);
                  this.Apply(prototype2);
                }
              }
            }
            else
              break;
          }
        }
      }
    }
  }

  public bool IsValid(
    HumanoidCharacterProfile profile,
    ICommonSession? session,
    ProtoId<LoadoutPrototype> loadout,
    IDependencyCollection collection,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    IPrototypeManager prototypeManager = collection.Resolve<IPrototypeManager>();
    LoadoutPrototype prototype;
    if (!prototypeManager.TryIndex<LoadoutPrototype>(loadout, out prototype))
    {
      reason = FormattedMessage.FromMarkupOrThrow("");
      return false;
    }
    if (!prototypeManager.HasIndex<RoleLoadoutPrototype>(this.Role))
    {
      reason = FormattedMessage.FromUnformatted("loadouts-prototype-missing");
      return false;
    }
    if (prototype.Cost.HasValue && this.Points.HasValue)
    {
      int? points = this.Points;
      int? cost = prototype.Cost;
      if (points.GetValueOrDefault() < cost.GetValueOrDefault() & points.HasValue & cost.HasValue)
      {
        reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-points-insufficient"));
        return false;
      }
    }
    bool flag = true;
    foreach (LoadoutEffect effect in prototype.Effects)
      flag = flag && effect.Validate(profile, this, session, collection, out reason);
    return flag;
  }

  public bool AddLoadout(
    ProtoId<LoadoutGroupPrototype> selectedGroup,
    ProtoId<LoadoutPrototype> selectedLoadout,
    IPrototypeManager protoManager)
  {
    List<Loadout> selectedLoadout1 = this.SelectedLoadouts[selectedGroup];
    int num = Math.Max(0, selectedLoadout1.Count + 1 - protoManager.Index<LoadoutGroupPrototype>(selectedGroup).MaxLimit);
    for (int index = 0; index < selectedLoadout1.Count; ++index)
    {
      if (!(selectedLoadout1[index].Prototype != selectedLoadout))
        return false;
      if (num > 0)
      {
        --num;
        selectedLoadout1.RemoveAt(index);
        --index;
      }
    }
    selectedLoadout1.Add(new Loadout()
    {
      Prototype = selectedLoadout
    });
    return true;
  }

  public bool RemoveLoadout(
    ProtoId<LoadoutGroupPrototype> selectedGroup,
    ProtoId<LoadoutPrototype> selectedLoadout,
    IPrototypeManager protoManager)
  {
    List<Loadout> selectedLoadout1 = this.SelectedLoadouts[selectedGroup];
    for (int index = 0; index < selectedLoadout1.Count; ++index)
    {
      if (!(selectedLoadout1[index].Prototype != selectedLoadout))
      {
        selectedLoadout1.RemoveAt(index);
        return true;
      }
    }
    return false;
  }

  public bool Equals(RoleLoadout? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    if (this.Role.Equals(other.Role) && this.SelectedLoadouts.Count == other.SelectedLoadouts.Count)
    {
      int? points1 = this.Points;
      int? points2 = other.Points;
      if (points1.GetValueOrDefault() == points2.GetValueOrDefault() & points1.HasValue == points2.HasValue && !(this.EntityName != other.EntityName))
      {
        foreach ((ProtoId<LoadoutGroupPrototype> key, List<Loadout> second) in this.SelectedLoadouts)
        {
          List<Loadout> first;
          if (!other.SelectedLoadouts.TryGetValue(key, out first) || !first.SequenceEqual<Loadout>((IEnumerable<Loadout>) second))
            return false;
        }
        return true;
      }
    }
    return false;
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is RoleLoadout other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<ProtoId<RoleLoadoutPrototype>, Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>, int?>(this.Role, this.SelectedLoadouts, this.Points);
  }

  public RoleLoadout()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RoleLoadout target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RoleLoadout>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<RoleLoadoutPrototype> target1 = new ProtoId<RoleLoadoutPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RoleLoadoutPrototype>>(this.Role, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<RoleLoadoutPrototype>>(this.Role, hookCtx, context);
    target.Role = target1;
    Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>> target2 = (Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>) null;
    if (this.SelectedLoadouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>>(this.SelectedLoadouts, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>>(this.SelectedLoadouts, hookCtx, context);
    target.SelectedLoadouts = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RoleLoadout target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RoleLoadout target1 = (RoleLoadout) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RoleLoadout Instantiate() => new RoleLoadout();
}
