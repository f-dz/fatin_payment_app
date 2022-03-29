using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PaymentApp.Data;
using PaymentApp.Models;

namespace PaymentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly ApiDbContext _context;
        
        public PaymentController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayment()
        {
            var payment = await _context.Payment.ToListAsync();
            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentData data)
        {
            if(ModelState.IsValid)
            {
                await _context.Payment.AddAsync(data);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPayment", new {data.PaymentDetailId}, data);
            }
            return new JsonResult("Something went wrong") {StatusCode = 500};
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment= await _context.Payment.FirstOrDefaultAsync(x => x.PaymentDetailId == id);

            if(payment == null) return NotFound();

            return Ok(payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, PaymentData payment)
        {
            if(id != payment.PaymentDetailId) return BadRequest();

            var existPayment = await _context.Payment.FirstOrDefaultAsync(x => x.PaymentDetailId == id);

            if(existPayment == null) return NotFound();

            existPayment.CardOwnerName = payment.CardOwnerName;
            existPayment.CardNumber = payment.CardNumber;
            existPayment.ExpirationDate = payment.ExpirationDate;
            existPayment.SecurityCode = payment.SecurityCode;

            //implement the changes on the database level
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var existPayment = await _context.Payment.FirstOrDefaultAsync(x => x.PaymentDetailId == id);

            if(existPayment == null) return NotFound();

            _context.Payment.Remove(existPayment);
            await _context.SaveChangesAsync();

            return Ok(existPayment);
        }
    }
}
