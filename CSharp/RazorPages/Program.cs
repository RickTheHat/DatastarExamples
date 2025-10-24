var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Use(async (context, next) => {

    // is this a datastar request? Datastar-Request: true
    var isDatastarRequest = context.Request.Headers.TryGetValue("Datastar-Request", out var headerValue)
        && string.Equals(headerValue, "true", StringComparison.OrdinalIgnoreCase);

    if (isDatastarRequest)
    {
        // TODO: parse the datastar payload and put it in context.Items as a simple, immutable, dictionary

        // We need to write the response for the datastar request here and then not call next.Invoke().
        // In order to do that, we need to know how to route the request or switch to a command handler pattern.
    }

    // The call to next.Invoke() triggers an exception that headers are being written but the response body has already started.
    // A custom exception handler middleware could be used to handle this more gracefully, but it would be nice to know what is causing this.
    try
    {
        await next.Invoke();
    }
    catch (InvalidOperationException ex)
    {
        // NOTE: Even though this exception is thrown, the response to the datastar request has already been sent. 

        // Log the exception or handle it as needed
        Console.WriteLine("Caught an InvalidOperationException: " + ex.Message);
    }
});

app.UseRouting();
app.UseAuthorization();


app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();