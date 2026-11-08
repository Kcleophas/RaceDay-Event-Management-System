using RaceDay.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<SessionBearerTokenHandler>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                 ?? "https://localhost:7067/";

builder.Services.AddHttpClient("RaceDayApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<SessionBearerTokenHandler>();

builder.Services.AddScoped<CategoryApiService>();
builder.Services.AddScoped<EventApiService>();
builder.Services.AddScoped<OrganiserEventApiService>();
builder.Services.AddScoped<EnrolmentApiService>();
builder.Services.AddScoped<ResultsApiService>();
builder.Services.AddScoped<UserProfileApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
