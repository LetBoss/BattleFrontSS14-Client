// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.EmplaceCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class EmplaceCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (EmplaceCommand.EmplaceBlockOutputParser)
  };

  public override Type[] TypeParameterParsers => EmplaceCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  private TOut Emplace<TOut, TIn>(IInvocationContext ctx, [PipedArgument] TIn value, [CommandArgument(typeof (EmplaceCommand.EmplaceBlockParser), false)] Block block)
  {
    return (TOut) block.Invoke((object) null, (IInvocationContext) new EmplaceCommand.EmplaceContext<TIn>(ctx, this.EntityManager)
    {
      Value = value
    });
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  private IEnumerable<TOut> Emplace<TOut, TIn>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<TIn> value,
    [CommandArgument(typeof (EmplaceCommand.EmplaceBlockParser), false)] Block block)
  {
    EmplaceCommand emplaceCommand = this;
    EmplaceCommand.EmplaceContext<TIn> emplaceCtx = new EmplaceCommand.EmplaceContext<TIn>(ctx, emplaceCommand.EntityManager);
    foreach (TIn @in in value)
    {
      if (ctx.HasErrors)
        break;
      emplaceCtx.Value = @in;
      yield return (TOut) block.Invoke((object) null, (IInvocationContext) emplaceCtx);
    }
  }

  private record EmplaceContext<T> : IInvocationContext
  {
    public T? Value;
    private readonly IInvocationContext _inner;
    private readonly IEntityManager _entMan;
    private readonly HashSet<string> _localVars = new HashSet<string>();

    public EmplaceContext(IInvocationContext inner, IEntityManager entMan, T? value = null)
    {
      this._inner = inner;
      this._entMan = entMan;
      this.Value = value;
      this._localVars.Add(nameof (value));
      if (typeof (T) == typeof (EntityUid))
      {
        this._localVars.Add("wx");
        this._localVars.Add("wy");
        this._localVars.Add("proto");
        this._localVars.Add("name");
        this._localVars.Add("desc");
        this._localVars.Add("paused");
      }
      else
      {
        if (!typeof (T).IsAssignableTo(typeof (ICommonSession)))
          return;
        this._localVars.Add("ent");
        this._localVars.Add("name");
        this._localVars.Add("userid");
      }
    }

    public bool CheckInvokable(CommandSpec command, out IConError? error)
    {
      return this._inner.CheckInvokable(command, out error);
    }

    public ICommonSession? Session => this._inner.Session;

    public ToolshedManager Toolshed => this._inner.Toolshed;

    public NetUserId? User => this._inner.User;

    public ToolshedEnvironment Environment => this._inner.Environment;

    public void WriteLine(string line) => this._inner.WriteLine(line);

    public void ReportError(IConError err) => this._inner.ReportError(err);

    public IEnumerable<IConError> GetErrors() => this._inner.GetErrors();

    public bool HasErrors => this._inner.HasErrors;

    public void ClearErrors() => this._inner.ClearErrors();

    public IEnumerable<string> GetVars()
    {
      foreach (string localVar in this._localVars)
        yield return localVar;
      foreach (string var in this._inner.GetVars())
      {
        if (!this._localVars.Contains(var))
          yield return var;
      }
    }

    public object? ReadVar(string name)
    {
      if (name == "value")
        return (object) this.Value;
      object obj1;
      switch (this.Value)
      {
        case EntityUid uid:
          object obj2;
          switch (name)
          {
            case "wx":
              obj2 = (object) this._entMan.System<SharedTransformSystem>().GetWorldPosition(uid).X;
              break;
            case "wy":
              obj2 = (object) this._entMan.System<SharedTransformSystem>().GetWorldPosition(uid).Y;
              break;
            case "proto":
              obj2 = (object) (this._entMan.GetComponent<MetaDataComponent>(uid).EntityPrototype?.ID ?? "");
              break;
            case "desc":
              obj2 = (object) this._entMan.GetComponent<MetaDataComponent>(uid).EntityDescription;
              break;
            case nameof (name):
              obj2 = (object) this._entMan.GetComponent<MetaDataComponent>(uid).EntityName;
              break;
            case "paused":
              obj2 = (object) this._entMan.GetComponent<MetaDataComponent>(uid).EntityPaused;
              break;
            default:
              obj2 = this._inner.ReadVar(name);
              break;
          }
          obj1 = obj2;
          break;
        case ICommonSession commonSession:
          object obj3;
          switch (name)
          {
            case "ent":
              obj3 = (object) commonSession.AttachedEntity;
              break;
            case nameof (name):
              obj3 = (object) commonSession.Name;
              break;
            case "userid":
              obj3 = (object) commonSession.UserId;
              break;
            default:
              obj3 = this._inner.ReadVar(name);
              break;
          }
          obj1 = obj3;
          break;
        default:
          obj1 = this._inner.ReadVar(name);
          break;
      }
      return obj1;
    }

    public void WriteVar(string name, object? value)
    {
      if (this._localVars.Contains(name))
        this.ReportError((IConError) new ReadonlyVariableError(name));
      else
        this._inner.WriteVar(name, value);
    }

    public bool IsReadonlyVar(string name) => this._localVars.Contains(name);

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Value = ");
      builder.Append((object) this.Value);
      builder.Append(", Session = ");
      builder.Append((object) this.Session);
      builder.Append(", Toolshed = ");
      builder.Append((object) this.Toolshed);
      builder.Append(", User = ");
      builder.Append(this.User.ToString());
      builder.Append(", Environment = ");
      builder.Append((object) this.Environment);
      builder.Append(", HasErrors = ");
      builder.Append(this.HasErrors.ToString());
      return true;
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.Value)) * -1521134295 + EqualityComparer<IInvocationContext>.Default.GetHashCode(this._inner)) * -1521134295 + EqualityComparer<IEntityManager>.Default.GetHashCode(this._entMan)) * -1521134295 + EqualityComparer<HashSet<string>>.Default.GetHashCode(this._localVars);
    }

    [CompilerGenerated]
    public virtual bool Equals(EmplaceCommand.EmplaceContext<
    #nullable disable
    T>
    #nullable enable
    ? other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<T>.Default.Equals(this.Value, other.Value) && EqualityComparer<IInvocationContext>.Default.Equals(this._inner, other._inner) && EqualityComparer<IEntityManager>.Default.Equals(this._entMan, other._entMan) && EqualityComparer<HashSet<string>>.Default.Equals(this._localVars, other._localVars);
    }

    [CompilerGenerated]
    protected EmplaceContext(EmplaceCommand.EmplaceContext<
    #nullable disable
    T> original)
    {
      this.Value = original.Value;
      this._inner = original._inner;
      this._entMan = original._entMan;
      this._localVars = original._localVars;
    }
  }

  private sealed class EmplaceBlockParser : CustomTypeParser<
  #nullable enable
  Block>
  {
    public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun? result)
    {
      if (ctx.Bundle.PipedType == (Type) null)
      {
        result = (CommandRun) null;
        return false;
      }
      Type type = ctx.Bundle.PipedType;
      if (type.IsGenericType(typeof (IEnumerable<>)))
        type = type.GetGenericArguments()[0];
      LocalVarParser localVarParser = EmplaceCommand.EmplaceBlockParser.SetupVarParser(ctx, type);
      int num = Block.TryParseBlock(ctx, (Type) null, (Type) null, out result) ? 1 : 0;
      ctx.VariableParser = localVarParser.Inner;
      return num != 0;
    }

    private static LocalVarParser SetupVarParser(ParserContext ctx, Type input)
    {
      LocalVarParser localVarParser = new LocalVarParser(ctx.VariableParser);
      ctx.VariableParser = (IVariableParser) localVarParser;
      localVarParser.SetLocalType("value", input, true);
      if (input == typeof (EntityUid))
      {
        localVarParser.SetLocalType("wx", typeof (float), true);
        localVarParser.SetLocalType("wy", typeof (float), true);
        localVarParser.SetLocalType("proto", typeof (string), true);
        localVarParser.SetLocalType("desc", typeof (string), true);
        localVarParser.SetLocalType("name", typeof (string), true);
        localVarParser.SetLocalType("paused", typeof (bool), true);
      }
      else if (input.IsAssignableTo(typeof (ICommonSession)))
      {
        localVarParser.SetLocalType("ent", typeof (EntityUid), true);
        localVarParser.SetLocalType("name", typeof (string), true);
        localVarParser.SetLocalType("userid", typeof (NetUserId), true);
      }
      return localVarParser;
    }

    public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? result)
    {
      result = (Block) null;
      CommandRun result1;
      if (!EmplaceCommand.EmplaceBlockParser.TryParse(ctx, out result1))
        return false;
      result = new Block(result1);
      return true;
    }

    public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
    {
      EmplaceCommand.EmplaceBlockParser.TryParse(ctx, out CommandRun _);
      return ctx.Completions;
    }
  }

  private sealed class EmplaceBlockOutputParser : CustomTypeParser<Type>
  {
    public override bool ShowTypeArgSignature => false;

    public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
    {
      result = (Type) null;
      ParserRestorePoint point = ctx.Save();
      CommandRun result1;
      if (!EmplaceCommand.EmplaceBlockParser.TryParse(ctx, out result1) || result1.ReturnType == (Type) null)
        return false;
      ctx.Restore(point);
      result = result1.ReturnType;
      return true;
    }

    public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
    {
      EmplaceCommand.EmplaceBlockParser.TryParse(ctx, out CommandRun _);
      return ctx.Completions;
    }
  }
}
