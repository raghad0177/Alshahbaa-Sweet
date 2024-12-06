using Microsoft.AspNetCore.Mvc;

namespace alshahbaasweets.Controllers
{
    public class NotificationsController : ControllerBase
    {
        // Simulate an in-memory list of orders (replace this with your actual data retrieval logic)
        private static List<string> requests = new List<string> { "Order 1", "Order 2" };

        // API endpoint to check for new requests
        [HttpGet("CheckForNewRequests")]
        public IActionResult CheckForNewRequests(int lastRequestCount)
        {
            bool hasNewRequests = requests.Count > lastRequestCount;
            return Ok(new { hasNewRequests = hasNewRequests, requestCount = requests.Count });
        }

        // API endpoint to add a new request (for testing purposes)
        [HttpPost("AddRequest")]
        public IActionResult AddRequest([FromBody] string request)
        {
            requests.Add(request);
            return Ok(new { message = "Request added successfully!" });
        }
    }
}