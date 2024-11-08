namespace DentalCare.Middlewares
{
    public class SessionCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            if (!path.StartsWithSegments("/Account")
                && !path.StartsWithSegments("/Home")
                && !path.Equals("/")
                && context.Session.GetString("UserId") == null
                && !path.StartsWithSegments("/Appointment/GetDoctorsByFaculty")
                && !path.StartsWithSegments("/Appointment/GetCustomerByPhone"))
            {
                context.Response.Redirect("/Account/Index");
                return;
            }

            await _next(context);
        }
    }
}
