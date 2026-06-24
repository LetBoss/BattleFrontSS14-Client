using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Physics.Dynamics.Contacts;

namespace Robust.Shared.Physics.Systems;

public record struct ContactEnumerator
{
	public static readonly ContactEnumerator Empty = new ContactEnumerator(null);

	private Dictionary<string, Robust.Shared.Physics.Dynamics.Fixture>.ValueCollection.Enumerator _fixtureEnumerator;

	private Dictionary<Robust.Shared.Physics.Dynamics.Fixture, Contact>.ValueCollection.Enumerator _contactEnumerator;

	public bool IncludeDeleting;

	public ContactEnumerator(FixturesComponent? fixtures, bool includeDeleting = false)
	{
		IncludeDeleting = includeDeleting;
		if (fixtures == null || fixtures.Fixtures.Count == 0)
		{
			this = Empty;
			return;
		}
		_fixtureEnumerator = fixtures.Fixtures.Values.GetEnumerator();
		_fixtureEnumerator.MoveNext();
		_contactEnumerator = _fixtureEnumerator.Current.Contacts.Values.GetEnumerator();
	}

	public bool MoveNext([NotNullWhen(true)] out Contact? contact)
	{
		if (!_contactEnumerator.MoveNext())
		{
			if (!_fixtureEnumerator.MoveNext())
			{
				contact = null;
				return false;
			}
			_contactEnumerator = _fixtureEnumerator.Current.Contacts.Values.GetEnumerator();
			return MoveNext(out contact);
		}
		contact = _contactEnumerator.Current;
		if (!IncludeDeleting && contact.Deleting)
		{
			return MoveNext(out contact);
		}
		return true;
	}
}
