using Business.DTOs;
using DataAccess;

namespace Business.Interfaces
{
    public interface ITest
    {
        Task<int> GetCountTest();
        Task<TestParentTable> CreateTest(TestDTO testDTO);
    }
}