namespace TaaghcheDemo.Services
{
    public interface IBookService
    {
        public Task<string> GetBookInfo(int bookId);
    }
}
