// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.GetVerbsEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Verbs;

public sealed class GetVerbsEvent<TVerb> : EntityEventArgs where TVerb : Verb
{
  public readonly SortedSet<TVerb> Verbs = new SortedSet<TVerb>();
  public readonly List<VerbCategory> ExtraCategories;
  public readonly bool CanAccess;
  public readonly EntityUid Target;
  public readonly EntityUid User;
  public readonly bool CanInteract;
  public readonly bool CanComplexInteract;
  public readonly HandsComponent? Hands;
  public readonly EntityUid? Using;

  public GetVerbsEvent(
    EntityUid user,
    EntityUid target,
    EntityUid? @using,
    HandsComponent? hands,
    bool canInteract,
    bool canComplexInteract,
    bool canAccess,
    List<VerbCategory> extraCategories)
  {
    this.User = user;
    this.Target = target;
    this.Using = @using;
    this.Hands = hands;
    this.CanAccess = canAccess;
    this.CanComplexInteract = canComplexInteract;
    this.CanInteract = canInteract;
    this.ExtraCategories = extraCategories;
  }
}
