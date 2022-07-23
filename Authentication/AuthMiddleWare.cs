public class AuthMiddleware {
    private readonly RequestDelegate _next;

    public AuthMiddleware (RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke (HttpContext context, JWTTokenRepository jwtUtils) {
        // try {
            var token = context.Request.Headers["Authorization"].FirstOrDefault ()?.Split (" ").Last ();
            var userId = jwtUtils.ValidateToken (token);
            if (userId != null) {
                // attach user to context on successful jwt validation
                context.Items["User"] = userId;
            }
            // else{
            //     throw new Exception("Validation Failed!");
            // }

            await _next (context);
        // } catch (Exception) {
        //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //     HttpResponseWritingExtensions.WriteAsync (context.Response, "{\"message\": \"Unauthorized\"}").Wait ();
        // }
    }
}