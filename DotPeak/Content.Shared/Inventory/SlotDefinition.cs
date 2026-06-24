// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.SlotDefinition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Inventory;

[DataDefinition]
public sealed class SlotDefinition : ISerializationGenerated<SlotDefinition>, ISerializationGenerated
{
  [DataField("whitelist", false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField("blacklist", false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;

  [DataField("name", false, 1, true, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField("slotTexture", false, 1, false, false, null)]
  public string TextureName { get; private set; } = "pocket";

  [DataField(null, false, 1, false, false, null)]
  public string FullTextureName { get; private set; } = "SlotBackground";

  [DataField("slotFlags", false, 1, false, false, null)]
  public SlotFlags SlotFlags { get; private set; } = SlotFlags.PREVENTEQUIP;

  [DataField("showInWindow", false, 1, false, false, null)]
  public bool ShowInWindow { get; private set; } = true;

  [DataField("slotGroup", false, 1, false, false, null)]
  public string SlotGroup { get; private set; } = "Default";

  [DataField("stripTime", false, 1, false, false, null)]
  public TimeSpan StripTime { get; private set; } = TimeSpan.FromSeconds(4.0);

  [DataField("uiWindowPos", false, 1, true, false, null)]
  public Vector2i UIWindowPosition { get; private set; }

  [DataField("strippingWindowPos", false, 1, true, false, null)]
  public Vector2i StrippingWindowPos { get; private set; }

  [DataField("dependsOn", false, 1, false, false, null)]
  public string? DependsOn { get; private set; }

  [DataField("dependsOnComponents", false, 1, false, false, null)]
  public ComponentRegistry? DependsOnComponents { get; private set; }

  [DataField("displayName", false, 1, true, false, null)]
  public string DisplayName { get; private set; } = string.Empty;

  [DataField("stripHidden", false, 1, false, false, null)]
  public bool StripHidden { get; private set; }

  [DataField("offset", false, 1, false, false, null)]
  public Vector2 Offset { get; private set; } = Vector2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SlotDefinition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SlotDefinition>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    string target2 = (string) null;
    if (this.TextureName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TextureName, ref target2, hookCtx, false, context))
      target2 = this.TextureName;
    target.TextureName = target2;
    string target3 = (string) null;
    if (this.FullTextureName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FullTextureName, ref target3, hookCtx, false, context))
      target3 = this.FullTextureName;
    target.FullTextureName = target3;
    SlotFlags target4 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.SlotFlags, ref target4, hookCtx, false, context))
      target4 = this.SlotFlags;
    target.SlotFlags = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowInWindow, ref target5, hookCtx, false, context))
      target5 = this.ShowInWindow;
    target.ShowInWindow = target5;
    string target6 = (string) null;
    if (this.SlotGroup == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotGroup, ref target6, hookCtx, false, context))
      target6 = this.SlotGroup;
    target.SlotGroup = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StripTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.StripTime, hookCtx, context);
    target.StripTime = target7;
    Vector2i target8 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.UIWindowPosition, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2i>(this.UIWindowPosition, hookCtx, context);
    target.UIWindowPosition = target8;
    Vector2i target9 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.StrippingWindowPos, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2i>(this.StrippingWindowPos, hookCtx, context);
    target.StrippingWindowPos = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DependsOn, ref target10, hookCtx, false, context))
      target10 = this.DependsOn;
    target.DependsOn = target10;
    ComponentRegistry target11 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.DependsOnComponents, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ComponentRegistry>(this.DependsOnComponents, hookCtx, context);
    target.DependsOnComponents = target11;
    string target12 = (string) null;
    if (this.DisplayName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DisplayName, ref target12, hookCtx, false, context))
      target12 = this.DisplayName;
    target.DisplayName = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.StripHidden, ref target13, hookCtx, false, context))
      target13 = this.StripHidden;
    target.StripHidden = target13;
    Vector2 target14 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target14;
    EntityWhitelist target15 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target15, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target15 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target15, hookCtx, context);
    }
    target.Whitelist = target15;
    EntityWhitelist target16 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target16, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target16 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target16, hookCtx, context);
    }
    target.Blacklist = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SlotDefinition target,
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
    SlotDefinition target1 = (SlotDefinition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SlotDefinition Instantiate() => new SlotDefinition();
}
