global using AutoMapper;
global using ColbyRJ.Data;
global using ColbyRJ.DTOs;
global using ColbyRJ.Models;
global using ColbyRJ.Repository.IRepository;
global using ColbyRJ.Models.CustomValidators;
global using ColbyRJ.Services;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;

using ColbyRJ.Components;
using ColbyRJ.Components.Account;
using ColbyRJ.Mapper;
using ColbyRJ.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<BrowserService>();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddAutoMapper(typeof(Maps));
builder.Services.AddRadzenComponents();

builder.Services.AddScoped<IFileUpload, FileUpload>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailSenderIdentity>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IDbInitializer, DbInitializer>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressCommentRepository, AddressCommentRepository>();
builder.Services.AddScoped<IAddressPhotoRepository, AddressPhotoRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IBroadcastEmailRepository, BroadcastEmailRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDocRepository, DocRepository>();
builder.Services.AddScoped<IDocCommentRepository, DocCommentRepository>();
builder.Services.AddScoped<IDocPdfRepository, DocPdfRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IHintRepository, HintRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobCommentRepository, JobCommentRepository>();
builder.Services.AddScoped<IOptInSendRepository, OptInSendRepository>();
builder.Services.AddScoped<IPhotoAlbumRepository, PhotoAlbumRepository>();
builder.Services.AddScoped<IPhotoAlbumCommentRepository, PhotoAlbumCommentRepository>();
builder.Services.AddScoped<IPhotoAlbumPhotoRepository, PhotoAlbumPhotoRepository>();
builder.Services.AddScoped<IPhotoFolderRepository, PhotoFolderRepository>();
builder.Services.AddScoped<IPhotoFolderCommentRepository, PhotoFolderCommentRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ISnippetRepository, SnippetRepository>();
builder.Services.AddScoped<ISnippetCommentRepository, SnippetCommentRepository>();
builder.Services.AddScoped<ISnippetPhotoRepository, SnippetPhotoRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IStoryChapterRepository, StoryChapterRepository>();
builder.Services.AddScoped<IStoryChapterCommentRepository, StoryChapterCommentRepository>();
builder.Services.AddScoped<IStoryChapterPhotoRepository, StoryChapterPhotoRepository>();
builder.Services.AddScoped<IStoryCommentRepository, StoryCommentRepository>();
builder.Services.AddScoped<IStoryPhotoRepository, StoryPhotoRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitCommentRepository, UnitCommentRepository>();
builder.Services.AddScoped<IUtilityRepository, UtilityRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IVideoCommentRepository, VideoCommentRepository>();
builder.Services.AddScoped<IWhoAmIRepository, WhoAmIRepository>();
builder.Services.AddScoped<IWhoAmICommentRepository, WhoAmICommentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

SeedDatabase();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
