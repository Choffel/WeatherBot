using Telegram.Bot;
using Telegram.Contracts;
using Telegram.Infrastructure.Handlers;
using Telegram.Infrastructure.Service;
using Weather.Contracts.Interface;
using Weather.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(
        Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
        ?? builder.Configuration["TELEGRAM_BOT_TOKEN"]
        ?? throw new InvalidOperationException("TELEGRAM_BOT_TOKEN is missing.")));

builder.Services.AddSingleton<IOpenMeteoClient, OpenMeteoClient>();
builder.Services.AddSingleton<HandlerUpdateAsync>();
builder.Services.AddSingleton<HandlerErrorAsync>();
builder.Services.AddSingleton<ITelegramService, TelegramService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();







