
using Api.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessagePublisher _weatherDataPublisher;
        private readonly DataDbContext _dbContext;

        public MessageController(IMessagePublisher weatherDataPublisher, DataDbContext dbContext)
        {
            _weatherDataPublisher = weatherDataPublisher;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            await _weatherDataPublisher.ProduceAsync(message);

            // Save to SQL Server
            var entity = new Message { Chat = message };
            _dbContext.Message.Add(entity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
