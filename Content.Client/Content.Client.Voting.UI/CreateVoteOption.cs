using System.Collections.Generic;

namespace Content.Client.Voting.UI;

public record struct CreateVoteOption
{
	public string Name;

	public List<Dictionary<string, string>> Dropdowns;

	public bool EnableVoteWarning;

	public int? FollowDropdownId;

	public CreateVoteOption(string name, List<Dictionary<string, string>> dropdowns, bool enableVoteWarning, int? followDropdownId)
	{
		Name = name;
		Dropdowns = dropdowns;
		EnableVoteWarning = enableVoteWarning;
		FollowDropdownId = followDropdownId;
	}
}
