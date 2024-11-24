using Core.Contracts;
using Core.Entities;
using Data;
using Data.Repositories;
using Dochost.Aspose;
using Dochost.Server.Endpoints;
using Dochost.Server.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IPreviewManager, PreviewManager>();
builder.Services.AddScoped<IDocumentInfoRepository, DocumentInfoRepository>();

builder.Services.AddHostedService<UploadProcessor>();
builder.Services.AddSingleton<IJobQueue>(_ => new JobQueue(100));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.RegisterDocumentEndpoints();

app.Run();