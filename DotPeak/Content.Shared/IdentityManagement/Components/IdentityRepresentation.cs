// Decompiled with JetBrains decompiler
// Type: Content.Shared.IdentityManagement.Components.IdentityRepresentation
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared.IdentityManagement.Components;

public sealed class IdentityRepresentation
{
  public string TrueName;
  public Gender TrueGender;
  public string AgeString;
  public string? PresumedName;
  public string? PresumedJob;

  public IdentityRepresentation(
    string trueName,
    Gender trueGender,
    string ageString,
    string? presumedName = null,
    string? presumedJob = null)
  {
    this.TrueName = trueName;
    this.TrueGender = trueGender;
    this.AgeString = ageString;
    this.PresumedJob = presumedJob;
    this.PresumedName = presumedName;
  }

  public string ToStringKnown(bool trueName)
  {
    return !trueName ? this.PresumedName ?? this.ToStringUnknown() : this.TrueName;
  }

  public string ToStringUnknown()
  {
    string str1;
    switch (this.TrueGender)
    {
      case Gender.Female:
        str1 = Loc.GetString("identity-gender-feminine");
        break;
      case Gender.Male:
        str1 = Loc.GetString("identity-gender-masculine");
        break;
      default:
        str1 = Loc.GetString("identity-gender-person");
        break;
    }
    string str2 = str1;
    if (this.PresumedJob == null)
      return $"{this.AgeString} {str2}";
    return $"{this.AgeString} {this.PresumedJob} {str2}";
  }
}
