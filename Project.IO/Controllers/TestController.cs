using AutoMapper;
using Business.DTOs;
using Business.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class TestController : ControllerBase
    {
        private readonly ITest _testService;
        private readonly Context _context;
        private readonly IMapper _mapper;

        public TestController(ITest testService, Context context, IMapper mapper)
        {
            _testService = testService;
            _context = context;
            _mapper = mapper;
        }

        ///<summary>Get count tests</summary>
        ///<returns></returns>
        // GET : api/TestController/Count
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetCountTest()
        {
            try
            {
                var count = await _testService.GetCountTest();
                return Ok(count);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        ///<summary>Get tests</summary>
        ///<returns></returns>
        // GET : api/TestController/Count
        [HttpGet]
        public async Task<ActionResult<List<TestDTO>>> GetTests()
        {
            try
            {
                var tests = await _context.Parents.Include(x => x.Childs).ToListAsync();
                return Ok(_mapper.Map<List<TestDTO>>(tests));
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        ///<summary>Get test</summary>
        ///<param name="id">ID of the test</param>
        ///<returns></returns>
        // GET : api/TestController/Count
        [HttpGet("{id}", Name = "GetTest")]
        public async Task<ActionResult<TestDTO>> GetTest([FromRoute] string id)
        {
            try
            {
                var test = await _context.Parents.Include(x => x.Childs).FirstOrDefaultAsync(x => x.Id == id);
                if(test != null){
                    return Ok(_mapper.Map<TestDTO>(test));
                }
                return NotFound();
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        ///<summary>Create test</summary>
        ///<param name="testDTO">Schema JSON from body</param>
        ///<returns></returns>
        // GET : api/TestController/Count
        [HttpPost]
        public async Task<ActionResult> CreateTest([FromBody] TestDTO testDTO)
        {
            try
            {
                var test = await _testService.CreateTest(testDTO);
                return CreatedAtRoute("GetTest", new{ Id = test.Id}, _mapper.Map<TestDTO>(test));
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    
        ///<summary>Delete test</summary>
        ///<param name="id">ID of the test</param>
        ///<returns></returns>
        // GET : api/TestController/Count
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTest([FromRoute] string id)
        {
            try
            {
                var test = await _context.Parents.FirstOrDefaultAsync(x => x.Id == id);
                if(test != null){
                    _context.Parents.Remove(test);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}