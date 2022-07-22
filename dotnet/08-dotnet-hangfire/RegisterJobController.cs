using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HangfireExercise
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterJobController : ControllerBase
    {

        public RegisterJobController()
        {
          
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello from hangfire web api !");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult FireAndForget()
        {
            var jobId = BackgroundJob.Enqueue(() => SendMessageWithEmailService("Welcome to our app"));
            return Ok($"Job IDL {jobId}, send to the user");
        }

       
        public void SendMessageWithEmailService(string v)
        {
            Console.WriteLine(v);
        }
    }
}
