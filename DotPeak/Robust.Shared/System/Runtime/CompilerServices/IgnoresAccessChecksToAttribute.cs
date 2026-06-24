// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class IgnoresAccessChecksToAttribute : Attribute
{
  private readonly string assemblyName;

  public string AssemblyName => this.assemblyName;

  public IgnoresAccessChecksToAttribute(string assemblyName) => this.assemblyName = assemblyName;
}
