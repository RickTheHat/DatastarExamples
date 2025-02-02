using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StarFederation.Datastar.DependencyInjection;
using StarFederation.Datastar.ModelBinding;
// ReSharper disable InconsistentNaming

namespace TestModelBinding.Controllers
{
    public class MySignals
    {
        public string sString  { get; set; } = "";
        public int sInt { get; set; } = 0;
        public InnerSignals sInner { get; set; } = new();

        public class InnerSignals
        {
            public string sInnerString { get; set; } = "";
            public int sInnerInt { get; set; } = 0;
            public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
        public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    public class ModelBindingTestsController : Controller
    {
        [Route("/")]
        public IActionResult Index() => Redirect("/index.html");

        [HttpGet("/test_get_signals")]
        public Task Test_GetSignals([FromServices] IDatastarServerSentEventService sse, [FromSignals] MySignals signals)
            => WriteAndSend(sse, signals);

        [HttpGet("/test_get_values")]
        public Task Test_GetValues([FromServices] IDatastarServerSentEventService sse, [FromSignals] string sString, [FromSignals] int sInt)
            => WriteAndSend(sse, new MySignals() { sString = sString, sInt = sInt });

        [HttpGet("/test_get_inner")]
        public Task Test_GetInner([FromServices] IDatastarServerSentEventService sse, [FromSignals] MySignals.InnerSignals sInner)
            => WriteAndSend(sse, sInner);

        [HttpGet("/test_get_innerPathed")]
        public Task Test_GetInnerPathed([FromServices] IDatastarServerSentEventService sse, [FromSignals(Path: "sInner")] MySignals.InnerSignals sInner)
            => WriteAndSend(sse, sInner);

        [HttpGet("/test_get_inner_values")]
        public Task Test_GetInnerValues([FromServices] IDatastarServerSentEventService sse, [FromSignals(Path: "sInner.sInnerString")] string sInnerString, [FromSignals(Path: "sInner.sInnerInt")] int sInnerInt)
            => WriteAndSend(sse, new MySignals() { sInner = new MySignals.InnerSignals { sInnerString = sInnerString, sInnerInt = sInnerInt } });

        [HttpPost("/test_post_signals")]
        public Task Test_PostSignals([FromServices] IDatastarServerSentEventService sse, [FromSignals] MySignals signals)
            => WriteAndSend(sse, signals);

        [HttpPost("/test_post_values")]
        public Task Test_PostValues([FromServices] IDatastarServerSentEventService sse, [FromSignals] string sString, [FromSignals] int sInt)
            => WriteAndSend(sse, new MySignals() { sString = sString, sInt = sInt });

        [HttpPost("/test_post_inner")]
        public Task Test_PostInner([FromServices] IDatastarServerSentEventService sse, [FromSignals] MySignals.InnerSignals sInner)
            => WriteAndSend(sse, sInner);

        [HttpPost("/test_post_innerPathed")]
        public Task Test_PostInnerPathed([FromServices] IDatastarServerSentEventService sse, [FromSignals(Path: "sInner")] MySignals.InnerSignals sInner)
            => WriteAndSend(sse, sInner);

        [HttpPost("/test_post_inner_values")]
        public Task Test_PostInnerValues([FromServices] IDatastarServerSentEventService sse, [FromSignals(Path: "sInner.sInnerString")] string sInnerString, [FromSignals(Path: "sInner.sInnerInt")] int sInnerInt)
            => WriteAndSend(sse, new MySignals() { sInner = new MySignals.InnerSignals { sInnerString = sInnerString, sInnerInt = sInnerInt } });

        [HttpGet("/test_form_get_values")]
        public IActionResult Test_FormGetValues([FromQuery] string sString, [FromQuery] int sInt)
        {
            var sig = new MySignals() { sString = sString, sInt = sInt };
            Console.WriteLine(sig);
            return Ok();
        }

        [HttpPost("/test_form_post_values")]
        public IActionResult Test_FormPostValues([FromBody] string sString, [FromBody] int sInt)
        {
            var sig = new MySignals() { sString = sString, sInt = sInt };
            Console.WriteLine(sig);
            return Ok();
        }

        private static async Task WriteAndSend<TType>(IDatastarServerSentEventService sse, TType input)
        {
            Console.WriteLine(input?.ToString() ?? "NULL");
            await sse.MergeFragmentsAsync($"<pre id=output>{input?.ToString() ?? "NULL"}</pre>");
        }
    }
}
