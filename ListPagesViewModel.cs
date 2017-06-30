using System.Collections;

namespace Episerver_Playground.Business.Plugins.Admin.ListPagesByPageTypes
{
    public class ListPagesViewModel : Pagination
    {
        public IEnumerable PagesList { get; set; }
        public IEnumerable PageTypesList { get; set; }
    }

    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int TotalPages { get; set; }
        public int NumberOfPages { get; set; }
        public int PageSize { get; set; }
    }
}