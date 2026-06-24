namespace Content.Client.Guidebook.Controls;

public interface ISearchableControl
{
	bool CheckMatchesSearch(string query);

	void SetHiddenState(bool state, string query);
}
