using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailSyncApi.Data;
using RetailSyncApi.Models;

namespace RetailSyncApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SyncController(AppDbContext context)
        {
            _context = context;
        }

        // 1. ВІДПРАВИТИ: Хтось (1С або Каса) кладе дані на сервер
        // POST: api/sync/push
        [HttpPost("push")]
        public async Task<IActionResult> PushData([FromBody] SyncPackage package)
        {
            if (package == null) return BadRequest();

            package.CreatedAtUtc = DateTime.UtcNow; // Ставимо час сервера

            _context.SyncPackages.Add(package);
            await _context.SaveChangesAsync();

            return Ok(new { id = package.Id, status = "Saved" });
        }

        // 2. ОТРИМАТИ: "Дай мені все, що є для мене"
        // GET: api/sync/pull?target=Shop_Kyiv_01
        [HttpGet("pull")]
        public async Task<IActionResult> PullData(string target)
        {
            var packages = await _context.SyncPackages
                .Where(p => p.Target == target)
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();

            return Ok(packages);
        }

        // 3. ПІДТВЕРДИТИ: "Я забрав пакет №5, видаляй його"
        // DELETE: api/sync/ack?id=5
        [HttpDelete("ack")]
        public async Task<IActionResult> Acknowledge(int id)
        {
            var package = await _context.SyncPackages.FindAsync(id);
            if (package == null) return NotFound("Package not found");

            _context.SyncPackages.Remove(package);
            await _context.SaveChangesAsync();

            return Ok(new { status = "Deleted", id = id });
        }

        // 4. ДІАГНОСТИКА: Просто перевірити, чи сервер живий
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("Pong! Server is working.");
    }
}