using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SuperShop.Helpers
{
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            // Ambas estas propriedades foram herdadas da classe ViewResult
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound; // Error 404 - not found
        }
    }
}
