// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.Verb
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Admin;
using Content.Shared.Database;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Virtual]
[Serializable]
public class Verb : IComparable
{
  public static string DefaultTextStyleClass = nameof (Verb);
  public string TextStyleClass = Verb.DefaultTextStyleClass;
  [NonSerialized]
  public Action? Act;
  [NonSerialized]
  public object? ExecutionEventArgs;
  [NonSerialized]
  public EntityUid EventTarget = EntityUid.Invalid;
  [NonSerialized]
  public bool ClientExclusive;
  public string Text = string.Empty;
  public SpriteSpecifier? Icon;
  public VerbCategory? Category;
  public bool Disabled;
  public string? Message;
  public int Priority;
  public NetEntity? IconEntity;
  public bool? CloseMenu;
  public LogImpact Impact = LogImpact.Low;
  public bool ConfirmationPopup;
  public bool? DoContactInteraction;
  public static List<Type> VerbTypes = new List<Type>()
  {
    typeof (Verb),
    typeof (VvVerb),
    typeof (InteractionVerb),
    typeof (UtilityVerb),
    typeof (InnateVerb),
    typeof (AlternativeVerb),
    typeof (ActivationVerb),
    typeof (ExamineVerb),
    typeof (EquipmentVerb),
    typeof (RMCAdminVerb)
  };

  public virtual int TypePriority => 0;

  public virtual bool CloseMenuDefault => true;

  public virtual bool DefaultDoContactInteraction => false;

  public int CompareTo(object? obj)
  {
    if (!(obj is Verb verb))
      return -1;
    if (this.TypePriority != verb.TypePriority)
      return verb.TypePriority - this.TypePriority;
    if (this.Priority != verb.Priority)
      return verb.Priority - this.Priority;
    if (this.Category?.Text != verb.Category?.Text)
      return string.Compare(this.Category?.Text, verb.Category?.Text, StringComparison.CurrentCulture);
    if (this.Text != verb.Text)
      return string.Compare(this.Text, verb.Text, StringComparison.CurrentCulture);
    NetEntity? iconEntity1 = this.IconEntity;
    NetEntity? iconEntity2 = verb.IconEntity;
    if ((iconEntity1.HasValue == iconEntity2.HasValue ? (iconEntity1.HasValue ? (iconEntity1.GetValueOrDefault() != iconEntity2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
      return string.Compare(this.Icon?.ToString(), verb.Icon?.ToString(), StringComparison.CurrentCulture);
    if (!this.IconEntity.HasValue)
      return -1;
    return !verb.IconEntity.HasValue ? 1 : this.IconEntity.Value.CompareTo(verb.IconEntity.Value);
  }
}
