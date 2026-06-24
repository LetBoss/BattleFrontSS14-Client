// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.PrettyPrint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

public static class PrettyPrint
{
  private static readonly FrozenDictionary<Type, string> TypeShortHand = new Dictionary<Type, string>()
  {
    {
      typeof (void),
      "void"
    },
    {
      typeof (object),
      "object"
    },
    {
      typeof (bool),
      "bool"
    },
    {
      typeof (byte),
      "byte"
    },
    {
      typeof (char),
      "char"
    },
    {
      typeof (Decimal),
      "decimal"
    },
    {
      typeof (double),
      "double"
    },
    {
      typeof (float),
      "float"
    },
    {
      typeof (int),
      "int"
    },
    {
      typeof (long),
      "long"
    },
    {
      typeof (sbyte),
      "sbyte"
    },
    {
      typeof (short),
      "short"
    },
    {
      typeof (string),
      "string"
    },
    {
      typeof (uint),
      "uint"
    },
    {
      typeof (ulong),
      "ulong"
    },
    {
      typeof (ushort),
      "ushort"
    }
  }.ToFrozenDictionary<Type, string>();

  public static string? PrintUserFacing(object? value)
  {
    return PrettyPrint.PrintUserFacingWithType(value, out string _);
  }

  public static string PrintUserFacingWithType(object? value, out string typeRep)
  {
    if (value == null)
    {
      typeRep = string.Empty;
      return "null";
    }
    string str;
    if (value.GetType().GetMethod("ToString", new Type[0], new ParameterModifier[0]).DeclaringType == typeof (object))
    {
      str = TypeAbbreviation.Abbreviate(value.GetType());
      typeRep = string.Empty;
    }
    else if (value is EntityUid uid)
    {
      str = (string) IoCManager.Resolve<IEntityManager>().ToPrettyString((Entity<MetaDataComponent>) uid);
      typeRep = TypeAbbreviation.Abbreviate(value.GetType());
    }
    else
    {
      str = value.ToString();
      typeRep = TypeAbbreviation.Abbreviate(value.GetType());
    }
    return str;
  }

  public static string PrintUserFacingTypeShort(Type type, int maxSections)
  {
    string str = TypeAbbreviation.Abbreviate(type);
    List<string> list = ((IEnumerable<string>) str.Split('.')).ToList<string>();
    int count = list.Count - maxSections - 1;
    if (count <= 0)
      return str;
    list.RemoveRange(maxSections, count);
    return string.Join<string>('.', (IEnumerable<string>) list);
  }

  public static string PrintTypeSignature(this Type type)
  {
    string str;
    if (PrettyPrint.TypeShortHand.TryGetValue(type, out str))
      return str;
    bool flag = false;
    Type underlyingType;
    if (type.IsNullable(out underlyingType))
    {
      type = underlyingType;
      flag = true;
    }
    if (!type.IsGenericType)
      return type.Name;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return $"{type.Name.Split('`')[0]}<{string.Join(", ", ((IEnumerable<Type>) type.GetGenericArguments()).Select<Type, string>(PrettyPrint.\u003C\u003EO.\u003C0\u003E__PrintTypeSignature ?? (PrettyPrint.\u003C\u003EO.\u003C0\u003E__PrintTypeSignature = new Func<Type, string>(PrettyPrint.PrintTypeSignature))))}>{(flag ? "?" : string.Empty)}";
  }

  public static string PrintParameterSignature(this ParameterInfo parameter)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (parameter.IsIn)
      stringBuilder1.Append("in ");
    if (parameter.IsOut)
      stringBuilder1.Append("out ");
    else if (parameter.ParameterType.IsByRef)
      stringBuilder1.Append("ref ");
    stringBuilder1.Append(parameter.ParameterType.PrintTypeSignature());
    string name = parameter.Name;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
    if (name != null)
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
      interpolatedStringHandler.AppendLiteral(" ");
      interpolatedStringHandler.AppendFormatted(name);
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local);
    }
    if (parameter.HasDefaultValue)
    {
      StringBuilder stringBuilder4 = stringBuilder1;
      StringBuilder stringBuilder5 = stringBuilder4;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder4);
      interpolatedStringHandler.AppendLiteral(" = ");
      interpolatedStringHandler.AppendFormatted(PrettyPrint.PrintUserFacing(parameter.DefaultValue));
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder5.Append(ref local);
    }
    return stringBuilder1.ToString();
  }

  public static string PrintMethodSignature(
    this MethodInfo method,
    bool modifiers = false,
    bool arguments = true,
    bool returnType = true,
    bool name = true)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (modifiers)
    {
      if (method.IsPublic)
        stringBuilder1.Append("public ");
      if (method.IsPrivate)
        stringBuilder1.Append("private ");
      if (method.IsFamilyAndAssembly)
        stringBuilder1.Append("private protected ");
      if (method.IsFamily)
        stringBuilder1.Append("protected ");
      if (method.IsFamilyOrAssembly)
        stringBuilder1.Append("protected internal ");
      if (method.IsAssembly)
        stringBuilder1.Append("internal ");
      if (method.IsStatic)
        stringBuilder1.Append("static ");
      if (method.IsAbstract)
      {
        Type declaringType = method.DeclaringType;
        if ((object) declaringType != null && declaringType.IsAbstract && !declaringType.IsInterface)
        {
          stringBuilder1.Append("abstract ");
          goto label_23;
        }
      }
      Type declaringType1 = method.DeclaringType;
      if ((object) declaringType1 != null && !declaringType1.IsInterface)
      {
        if (method.IsFinal)
          stringBuilder1.Append("sealed override ");
        else if (method.IsVirtual)
          stringBuilder1.Append(method.Equals((object) method.GetBaseDefinition()) ? "virtual " : "override ");
      }
    }
label_23:
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
    if (returnType && !method.IsConstructor)
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
      interpolatedStringHandler.AppendFormatted(method.ReturnType.PrintTypeSignature());
      interpolatedStringHandler.AppendLiteral(" ");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local);
    }
    if (name)
      stringBuilder1.Append(method.Name);
    if (!arguments)
      return stringBuilder1.ToString();
    if (method.IsGenericMethod)
    {
      StringBuilder stringBuilder4 = stringBuilder1;
      StringBuilder stringBuilder5 = stringBuilder4;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder4);
      interpolatedStringHandler.AppendLiteral("<");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      interpolatedStringHandler.AppendFormatted(string.Join(", ", ((IEnumerable<Type>) method.GetGenericArguments()).Select<Type, string>(PrettyPrint.\u003C\u003EO.\u003C0\u003E__PrintTypeSignature ?? (PrettyPrint.\u003C\u003EO.\u003C0\u003E__PrintTypeSignature = new Func<Type, string>(PrettyPrint.PrintTypeSignature)))));
      interpolatedStringHandler.AppendLiteral(">");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder5.Append(ref local);
    }
    StringBuilder stringBuilder6 = stringBuilder1;
    StringBuilder stringBuilder7 = stringBuilder6;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder6);
    interpolatedStringHandler.AppendLiteral("(");
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    interpolatedStringHandler.AppendFormatted(string.Join(", ", ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, string>(PrettyPrint.\u003C\u003EO.\u003C1\u003E__PrintParameterSignature ?? (PrettyPrint.\u003C\u003EO.\u003C1\u003E__PrintParameterSignature = new Func<ParameterInfo, string>(PrettyPrint.PrintParameterSignature)))));
    interpolatedStringHandler.AppendLiteral(")");
    ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
    stringBuilder7.Append(ref local1);
    return stringBuilder1.ToString();
  }

  public static string PrintPropertySignature(
    this PropertyInfo property,
    bool modifiers = false,
    bool accessors = false)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder stringBuilder2 = stringBuilder1;
    StringBuilder stringBuilder3 = stringBuilder2;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
    interpolatedStringHandler.AppendFormatted(property.PropertyType.PrintTypeSignature());
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(property.Name);
    ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
    stringBuilder3.Append(ref local1);
    if (accessors)
    {
      stringBuilder1.Append(" { ");
      if (property.CanRead)
      {
        StringBuilder stringBuilder4 = stringBuilder1;
        StringBuilder stringBuilder5 = stringBuilder4;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder4);
        interpolatedStringHandler.AppendFormatted(property.GetMethod.PrintMethodSignature(modifiers, false, false, false));
        interpolatedStringHandler.AppendLiteral("get; ");
        ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
        stringBuilder5.Append(ref local2);
      }
      if (property.CanWrite)
      {
        StringBuilder stringBuilder6 = stringBuilder1;
        StringBuilder stringBuilder7 = stringBuilder6;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder6);
        interpolatedStringHandler.AppendFormatted(property.SetMethod.PrintMethodSignature(modifiers, false, false, false));
        interpolatedStringHandler.AppendLiteral("set; ");
        ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
        stringBuilder7.Append(ref local3);
      }
      stringBuilder1.Append('}');
    }
    return stringBuilder1.ToString();
  }

  public static string PrintFieldSignature(this FieldInfo field, bool modifiers = false)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (modifiers)
    {
      if (field.IsPublic)
        stringBuilder1.Append("public ");
      if (field.IsPrivate)
        stringBuilder1.Append("private ");
      if (field.IsFamilyAndAssembly)
        stringBuilder1.Append("private protected ");
      if (field.IsFamily)
        stringBuilder1.Append("protected ");
      if (field.IsFamilyOrAssembly)
        stringBuilder1.Append("protected internal ");
      if (field.IsAssembly)
        stringBuilder1.Append("internal ");
      if (field.IsStatic)
        stringBuilder1.Append("static ");
    }
    StringBuilder stringBuilder2 = stringBuilder1;
    StringBuilder stringBuilder3 = stringBuilder2;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
    interpolatedStringHandler.AppendFormatted(field.FieldType.PrintTypeSignature());
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(field.Name);
    ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
    stringBuilder3.Append(ref local);
    return stringBuilder1.ToString();
  }
}
