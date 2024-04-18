using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaaghcheDemo.Infrastructure
{
    public interface IApiService
    {
        public Task<string> GetJsonDataAsString(string apiUrl, int bookId);
    }
}
