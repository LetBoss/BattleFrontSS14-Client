// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.RoleCodeword.CodewordsData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles.RoleCodeword;

[DataDefinition]
[NetSerializable]
[Serializable]
public struct CodewordsData : ISerializationGenerated<CodewordsData>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Color Color;
  [DataField(null, false, 1, false, false, null)]
  public List<string> Codewords;

  public CodewordsData(Color color, List<string> codewords)
  {
    this.Color = color;
    this.Codewords = codewords;
  }

  public CodewordsData()
  {
    this.Color = new Color();
    this.Codewords = (List<string>) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CodewordsData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CodewordsData>(this, ref target, hookCtx, false, context))
      return;
    Color target1 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    List<string> target2 = (List<string>) null;
    if (this.Codewords == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Codewords, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<string>>(this.Codewords, hookCtx, context);
    target = target with
    {
      Color = target1,
      Codewords = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CodewordsData target,
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
    CodewordsData target1 = (CodewordsData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CodewordsData Instantiate() => new CodewordsData();
}
