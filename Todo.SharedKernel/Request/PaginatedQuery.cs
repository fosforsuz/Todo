namespace Todo.SharedKernel.Request;

public abstract class PaginatedQuery
{
    private const int MaxPageSize = 100;

    private int _page = 1;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? 10 : value;
    }

    public string? Search { get; set; }

    /// <summary>
    /// Alan adı (örn: "createdAt", "name") — mapping'de dikkatli ol!
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// ASC, DESC gibi bir sıralama yönü. Default ASC.
    /// </summary>
    public string SortDirection { get; set; } = "ASC";

    public bool IsDescending => SortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
}