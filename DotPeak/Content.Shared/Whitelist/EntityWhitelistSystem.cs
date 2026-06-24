// Decompiled with JetBrains decompiler
// Type: Content.Shared.Whitelist.EntityWhitelistSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Whitelist;

public sealed class EntityWhitelistSystem : EntitySystem
{
  [Dependency]
  private TagSystem _tag;
  private Robust.Shared.GameObjects.EntityQuery<ItemComponent> _itemQuery;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private RMCSizeStunSystem _rmcSizeStun;

  public override void Initialize()
  {
    base.Initialize();
    this._itemQuery = this.GetEntityQuery<ItemComponent>();
  }

  public bool IsValid(EntityWhitelist list, [NotNullWhen(true)] EntityUid? uid)
  {
    return uid.HasValue && this.IsValid(list, uid.Value);
  }

  public bool CheckBoth([NotNullWhen(true)] EntityUid? uid, EntityWhitelist? blacklist = null, EntityWhitelist? whitelist = null)
  {
    if (!uid.HasValue || blacklist != null && this.IsValid(blacklist, uid))
      return false;
    return whitelist == null || this.IsValid(whitelist, uid);
  }

  public bool IsValid(EntityWhitelist list, EntityUid uid)
  {
    if (list.Components != null && list.Registrations == null)
    {
      List<ComponentRegistration> regs = this.StringsToRegs(list.Components);
      list.Registrations = new List<ComponentRegistration>();
      list.Registrations.AddRange((IEnumerable<ComponentRegistration>) regs);
    }
    if (list.Registrations != null && list.Registrations.Count > 0)
    {
      foreach (ComponentRegistration registration in list.Registrations)
      {
        if (this.HasComp(uid, registration.Type))
        {
          if (!list.RequireAll)
            return true;
        }
        else if (list.RequireAll)
          return false;
      }
    }
    ItemComponent component;
    if (list.Sizes != null && this._itemQuery.TryComp(uid, out component) && list.Sizes.Contains(component.Size))
      return true;
    return list.Tags != null ? (!list.RequireAll ? this._tag.HasAnyTag(uid, list.Tags) : this._tag.HasAllTags(uid, list.Tags)) : (list.Skills != null ? (!list.RequireAll ? this._skills.HasAnySkills((Entity<SkillsComponent>) uid, list.Skills) : this._skills.HasAllSkills((Entity<SkillsComponent>) uid, list.Skills)) : (list.MinMobSize.HasValue ? this._rmcSizeStun.IsXenoSized((Entity<RMCSizeComponent>) uid) : list.RequireAll));
  }

  public bool IsWhitelistPass(EntityWhitelist? whitelist, EntityUid uid)
  {
    return whitelist != null && this.IsValid(whitelist, uid);
  }

  public bool IsWhitelistFail(EntityWhitelist? whitelist, EntityUid uid)
  {
    return whitelist != null && !this.IsValid(whitelist, uid);
  }

  public bool IsWhitelistPassOrNull(EntityWhitelist? whitelist, EntityUid uid)
  {
    return whitelist == null || this.IsValid(whitelist, uid);
  }

  public bool IsWhitelistFailOrNull(EntityWhitelist? whitelist, EntityUid uid)
  {
    return whitelist == null || !this.IsValid(whitelist, uid);
  }

  public bool IsBlacklistPass(EntityWhitelist? blacklist, EntityUid uid)
  {
    return this.IsWhitelistPass(blacklist, uid);
  }

  public bool IsBlacklistFail(EntityWhitelist? blacklist, EntityUid uid)
  {
    return this.IsWhitelistFail(blacklist, uid);
  }

  public bool IsBlacklistPassOrNull(EntityWhitelist? blacklist, EntityUid uid)
  {
    return this.IsWhitelistPassOrNull(blacklist, uid);
  }

  public bool IsBlacklistFailOrNull(EntityWhitelist? blacklist, EntityUid uid)
  {
    return this.IsWhitelistFailOrNull(blacklist, uid);
  }

  private List<ComponentRegistration> StringsToRegs(string[]? input)
  {
    List<ComponentRegistration> regs = new List<ComponentRegistration>();
    if (input == null || input.Length == 0)
      return regs;
    foreach (string componentName in input)
    {
      ComponentAvailability componentAvailability = this.Factory.GetComponentAvailability(componentName);
      ComponentRegistration registration;
      if (this.Factory.TryGetRegistration(componentName, out registration) && componentAvailability == ComponentAvailability.Available)
        regs.Add(registration);
      else if (componentAvailability == ComponentAvailability.Unknown)
        this.Log.Error($"StringsToRegs failed: Unknown component name {componentName} passed to EntityWhitelist!");
    }
    return regs;
  }
}
