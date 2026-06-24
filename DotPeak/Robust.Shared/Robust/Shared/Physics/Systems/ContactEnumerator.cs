// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.ContactEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public record struct ContactEnumerator
{
  public static readonly ContactEnumerator Empty = new ContactEnumerator((FixturesComponent) null);
  private Dictionary<string, Fixture>.ValueCollection.Enumerator _fixtureEnumerator;
  private Dictionary<Fixture, Contact>.ValueCollection.Enumerator _contactEnumerator;
  public bool IncludeDeleting;

  public ContactEnumerator(FixturesComponent? fixtures, bool includeDeleting = false)
  {
    this.IncludeDeleting = includeDeleting;
    if (fixtures == null || fixtures.Fixtures.Count == 0)
    {
      this = ContactEnumerator.Empty;
    }
    else
    {
      this._fixtureEnumerator = fixtures.Fixtures.Values.GetEnumerator();
      this._fixtureEnumerator.MoveNext();
      this._contactEnumerator = this._fixtureEnumerator.Current.Contacts.Values.GetEnumerator();
    }
  }

  public bool MoveNext([NotNullWhen(true)] out Contact? contact)
  {
    if (!this._contactEnumerator.MoveNext())
    {
      if (!this._fixtureEnumerator.MoveNext())
      {
        contact = (Contact) null;
        return false;
      }
      this._contactEnumerator = this._fixtureEnumerator.Current.Contacts.Values.GetEnumerator();
      return this.MoveNext(out contact);
    }
    contact = this._contactEnumerator.Current;
    return this.IncludeDeleting || !contact.Deleting || this.MoveNext(out contact);
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<Dictionary<string, Fixture>.ValueCollection.Enumerator>.Default.GetHashCode(this._fixtureEnumerator) * -1521134295 + EqualityComparer<Dictionary<Fixture, Contact>.ValueCollection.Enumerator>.Default.GetHashCode(this._contactEnumerator)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.IncludeDeleting);
  }

  [CompilerGenerated]
  public readonly bool Equals(ContactEnumerator other)
  {
    return EqualityComparer<Dictionary<string, Fixture>.ValueCollection.Enumerator>.Default.Equals(this._fixtureEnumerator, other._fixtureEnumerator) && EqualityComparer<Dictionary<Fixture, Contact>.ValueCollection.Enumerator>.Default.Equals(this._contactEnumerator, other._contactEnumerator) && EqualityComparer<bool>.Default.Equals(this.IncludeDeleting, other.IncludeDeleting);
  }
}
