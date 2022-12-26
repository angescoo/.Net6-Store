namespace API.Helpers;

public class Params
{
    private int _pageSize = 5;
    private const int MAX_PAGE_SIZE = 50;
    private int _pageIndex = 1;
    private string _search;


    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MAX_PAGE_SIZE)? MAX_PAGE_SIZE : value;
    }

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = (value <= 0)? 1 : value;
    }

    public string Search
    {
        get => _search;
        set => _search = (!string.IsNullOrEmpty(value)) ? value.ToLower() : string.Empty;
    }
}
