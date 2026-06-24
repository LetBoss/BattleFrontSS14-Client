// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.EntityWhitelistCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class EntityWhitelistCondition : 
  IConstructionCondition,
  ISerializationGenerated<EntityWhitelistCondition>,
  ISerializationGenerated
{
  [DataField("conditionString", false, 1, false, false, null)]
  public string ConditionString = "construction-step-condition-entity-whitelist";
  [DataField("conditionIcon", false, 1, false, false, null)]
  public SpriteSpecifier? ConditionIcon;
  [DataField("whitelist", false, 1, true, false, null)]
  public EntityWhitelist Whitelist = new EntityWhitelist();

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    return IoCManager.Resolve<IEntityManager>().System<EntityWhitelistSystem>().IsWhitelistPass(this.Whitelist, user);
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = this.ConditionString,
      Icon = this.ConditionIcon
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityWhitelistCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityWhitelistCondition>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.ConditionString == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ConditionString, ref str, hookCtx, false, context))
      str = this.ConditionString;
    target.ConditionString = str;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.ConditionIcon, ref spriteSpecifier, hookCtx, true, context))
      spriteSpecifier = serialization.CreateCopy<SpriteSpecifier>(this.ConditionIcon, hookCtx, context, false);
    target.ConditionIcon = spriteSpecifier;
    EntityWhitelist entityWhitelist = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist, hookCtx, context, true);
    }
    target.Whitelist = entityWhitelist;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityWhitelistCondition target,
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
    EntityWhitelistCondition target1 = (EntityWhitelistCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public EntityWhitelistCondition Instantiate() => new EntityWhitelistCondition();
}
