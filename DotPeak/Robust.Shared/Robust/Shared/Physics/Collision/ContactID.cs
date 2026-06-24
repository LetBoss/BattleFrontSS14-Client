// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.ContactID
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Physics.Collision;

[StructLayout(LayoutKind.Explicit)]
public struct ContactID
{
  [FieldOffset(0)]
  public ContactFeature Features;
  [FieldOffset(0)]
  public uint Key;

  public static bool operator ==(ContactID id, ContactID other) => (int) id.Key == (int) other.Key;

  public static bool operator !=(ContactID id, ContactID other) => !(id == other);

  public override bool Equals(object? obj)
  {
    return obj is ContactID contactId && (int) this.Key == (int) contactId.Key;
  }

  public bool Equals(ContactID other) => (int) this.Key == (int) other.Key;

  public override int GetHashCode() => this.Key.GetHashCode();
}
