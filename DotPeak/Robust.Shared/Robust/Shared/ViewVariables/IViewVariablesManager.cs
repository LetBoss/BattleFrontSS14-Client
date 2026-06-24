// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.IViewVariablesManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Player;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.ViewVariables;

[NotContentImplementable]
public interface IViewVariablesManager
{
  void RegisterDomain(string domain, DomainResolveObject resolveObject, DomainListPaths list);

  bool UnregisterDomain(string domain);

  ViewVariablesTypeHandler<T> GetTypeHandler<T>();

  ViewVariablesPath? ResolvePath(string path);

  object? ReadPath(string path);

  string? ReadPathSerialized(string path);

  void WritePath(string path, string value);

  object? InvokePath(string path, string arguments);

  IEnumerable<string> ListPath(string path, VVListPathOptions options);

  Task<string?> ReadRemotePath(string path, ICommonSession? session = null);

  Task WriteRemotePath(string path, string value, ICommonSession? session = null);

  Task<string?> InvokeRemotePath(string path, string arguments, ICommonSession? session = null);

  Task<IEnumerable<string>> ListRemotePath(
    string path,
    VVListPathOptions options,
    ICommonSession? session = null);
}
