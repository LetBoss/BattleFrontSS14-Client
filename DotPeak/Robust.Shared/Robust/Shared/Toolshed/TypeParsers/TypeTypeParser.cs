// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.TypeTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class TypeTypeParser : TypeParser<Type>
{
  [Dependency]
  private readonly IModLoader _modLoader;
  public Dictionary<string, Type> Types = new Dictionary<string, Type>()
  {
    {
      "object",
      typeof (object)
    },
    {
      "int",
      typeof (int)
    },
    {
      "uint",
      typeof (uint)
    },
    {
      "char",
      typeof (char)
    },
    {
      "byte",
      typeof (byte)
    },
    {
      "sbyte",
      typeof (sbyte)
    },
    {
      "short",
      typeof (short)
    },
    {
      "ushort",
      typeof (ushort)
    },
    {
      "long",
      typeof (ulong)
    },
    {
      "ulong",
      typeof (ulong)
    },
    {
      "string",
      typeof (string)
    },
    {
      "bool",
      typeof (bool)
    },
    {
      "nint",
      typeof (IntPtr)
    },
    {
      "nuint",
      typeof (UIntPtr)
    },
    {
      "float",
      typeof (float)
    },
    {
      "double",
      typeof (double)
    },
    {
      "decimal",
      typeof (Decimal)
    },
    {
      "Vector2",
      typeof (Vector2)
    },
    {
      "TimeSpan",
      typeof (TimeSpan)
    },
    {
      "DateTime",
      typeof (DateTime)
    },
    {
      "IEnumerable",
      typeof (IEnumerable<>)
    },
    {
      "List",
      typeof (List<>)
    },
    {
      "HashSet",
      typeof (HashSet<>)
    },
    {
      "Task",
      typeof (Task<>)
    },
    {
      "ValueTask",
      typeof (ValueTask<>)
    },
    {
      "Dictionary",
      typeof (Dictionary<,>)
    }
  };
  private readonly HashSet<string> _ambiguousTypes = new HashSet<string>();
  private CompletionResult? _optionsCache;

  public override void PostInject()
  {
    foreach (Assembly assembly in this._modLoader.LoadedModules.Append<Assembly>(Assembly.GetExecutingAssembly()).Append<Assembly>(Assembly.GetAssembly(typeof (Box2))))
    {
      foreach (Type exportedType in assembly.ExportedTypes)
      {
        string name = exportedType.Name;
        if (!this._ambiguousTypes.Contains(name) && !this.Types.TryAdd(name, exportedType))
        {
          this.Types.Remove(name);
          this._ambiguousTypes.Add(name);
        }
      }
    }
    this._optionsCache = CompletionResult.FromHintOptions(this.Types.Select<KeyValuePair<string, Type>, CompletionOption>((Func<KeyValuePair<string, Type>, CompletionOption>) (x => new CompletionOption(x.Key))), "C# level type");
  }

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(TypeTypeParser.\u003C\u003EO.\u003C0\u003E__IsLetterOrDigit ?? (TypeTypeParser.\u003C\u003EO.\u003C0\u003E__IsLetterOrDigit = new Func<Rune, bool>(Rune.IsLetterOrDigit)));
    if (word == null)
    {
      ctx.Error = (IConError) new OutOfInputError();
      result = (Type) null;
      return false;
    }
    Type type = this.ParseBase(word);
    if ((object) type == null)
    {
      ctx.Error = (IConError) new UnknownType(word);
      result = (Type) null;
      return false;
    }
    if (type.IsGenericTypeDefinition)
    {
      if (!ctx.EatMatch('<'))
      {
        ctx.Error = (IConError) new ExpectedGeneric();
        result = (Type) null;
        return false;
      }
      int length = type.GetGenericArguments().Length;
      Type[] typeArray = new Type[type.GetGenericArguments().Length];
      for (int index = 0; index < length; ++index)
      {
        Type result1;
        if (!this.TryParse(ctx, out result1))
        {
          result = (Type) null;
          return false;
        }
        typeArray[index] = result1;
        if (index != length - 1 && !ctx.EatMatch(','))
        {
          ctx.Error = (IConError) new ExpectedNextType();
          result = (Type) null;
          return false;
        }
      }
      if (!ctx.EatMatch('>'))
      {
        ctx.Error = (IConError) new ExpectedGeneric();
        result = (Type) null;
        return false;
      }
      type = type.MakeGenericType(typeArray);
    }
    if (ctx.EatMatch('['))
    {
      if (!ctx.EatMatch(']'))
      {
        ctx.Error = (IConError) new UnknownType(word);
        result = (Type) null;
        return false;
      }
      type = type.MakeArrayType();
    }
    if (ctx.EatMatch('?') && (type.IsValueType || type.IsPrimitive))
      type = typeof (Nullable<>).MakeGenericType(type);
    result = type;
    return true;
  }

  private Type? ParseBase(string word)
  {
    Type type;
    this.Types.TryGetValue(word, out type);
    return type;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    if (this._optionsCache != (CompletionResult) null)
      this._optionsCache.Hint = this.GetArgHint(arg);
    return this._optionsCache;
  }
}
