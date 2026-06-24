// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ScriptingColorScheme
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public static class ScriptingColorScheme
{
  public const string ClassName = "class name";
  public const string Comment = "comment";
  public const string EnumName = "enum name";
  public const string FieldName = "field name";
  public const string InterfaceName = "interface name";
  public const string Keyword = "keyword";
  public const string MethodName = "method name";
  public const string NamespaceName = "namespace name";
  public const string NumericLiteral = "number";
  public const string PropertyName = "property name";
  public const string StaticSymbol = "static symbol";
  public const string StringLiteral = "string";
  public const string StructName = "struct name";
  public const string Default = "default";
  public static readonly FrozenDictionary<string, Color> ColorScheme = new Dictionary<string, Color>()
  {
    {
      "class name",
      Color.FromHex("#4EC9B0".AsSpan(), new Color?())
    },
    {
      "comment",
      Color.FromHex("#57A64A".AsSpan(), new Color?())
    },
    {
      "enum name",
      Color.FromHex("#B8D7A3".AsSpan(), new Color?())
    },
    {
      "field name",
      Color.FromHex("#C86E11".AsSpan(), new Color?())
    },
    {
      "interface name",
      Color.FromHex("#B8D7A3".AsSpan(), new Color?())
    },
    {
      "keyword",
      Color.FromHex("#569CD6".AsSpan(), new Color?())
    },
    {
      "method name",
      Color.FromHex("#11A3C8".AsSpan(), new Color?())
    },
    {
      "namespace name",
      Color.FromHex("#C8A611".AsSpan(), new Color?())
    },
    {
      "number",
      Color.FromHex("#b5cea8".AsSpan(), new Color?())
    },
    {
      "property name",
      Color.FromHex("#11C89D".AsSpan(), new Color?())
    },
    {
      "static symbol",
      Color.FromHex("#4EC9B0".AsSpan(), new Color?())
    },
    {
      "string",
      Color.FromHex("#D69D85".AsSpan(), new Color?())
    },
    {
      "struct name",
      Color.FromHex("#4EC9B0".AsSpan(), new Color?())
    },
    {
      "default",
      Color.FromHex("#D4D4D4".AsSpan(), new Color?())
    }
  }.ToFrozenDictionary<string, Color>();
}
