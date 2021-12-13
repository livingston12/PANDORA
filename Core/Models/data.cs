namespace Pandora.Core.Models
{
    public sealed class Result<TData>
    {
        public TData Data { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}