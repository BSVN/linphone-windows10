namespace BSN.Resa.Vns.Commons.Responses
{
    public class GenericResponseBaseWithPagination<T> : GenericResponseBase<T> where T : class
    {
        public PaginationMetadata Meta { get; set; }
    }
}
