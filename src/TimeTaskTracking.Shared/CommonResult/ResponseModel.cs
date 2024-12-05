namespace TimeTaskTracking.Shared.CommonResult
{
    public class ResponseModel<T>
    {
        public List<string> Message { get; set; } = new List<string>();
        public T Model {  get; set; }
    }
}
