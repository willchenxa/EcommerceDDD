using EcommerceDDD.Core.CQRS;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Payments.Infrastructure.Projections;
using EcommerceDDD.Payments.Application.RequestingPayment;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);
builder.Services.ConfigureCQRS();

// ---- Services
builder.Services.AddTransient<ICustomerCreditChecker, CustomerCreditChecker>();
builder.Services.AddTransient<IEventStoreRepository<Payment>, MartenRepository<Payment>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();
builder.Services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();