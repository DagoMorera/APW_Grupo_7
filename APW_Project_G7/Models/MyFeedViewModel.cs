namespace APW.Mvc.Models;

public class MyFeedViewModel
{
    public List<SourceViewModel> SubscribedSources { get; set; } = new();
    public List<FeedEntryViewModel> Entries { get; set; } = new();
}