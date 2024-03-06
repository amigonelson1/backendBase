using AutoMapper;
using Business.DTOs;
using Business.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class TestService : ITest
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public TestService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> GetCountTest()
        {
            var tests = await _context.Parents.ToListAsync();
            if (tests != null)
            {
                return tests.Count;
            }
            return 0;
        }
        public async Task<TestParentTable> CreateTest(TestDTO testDTO)
        {
            if(testDTO.DateValue.Date < new DateTime(2000, 01, 01).Date || testDTO.DateValue.Date > new DateTime(2025, 01, 01).Date) throw new ArgumentException("date out of range");
            var Id = Guid.NewGuid().ToString();
            testDTO.Id = Id;
            var tets = _mapper.Map<TestParentTable>(testDTO);
            _context.Parents.Add(tets);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return await _context.Parents.FirstOrDefaultAsync(x => x.Id == Id) ?? throw new ArgumentException("There is no test in the data base");
            }
            throw new ArgumentException("Error, please try again");
        }
    }
}