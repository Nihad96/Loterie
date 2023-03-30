using DataLayer;
using Loterie.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// on remplit cette variable grâce au fichier appsetings.json à la racine de Loterie
// Cela permet d'éviter de mettre en dur des infos sensibles dans le progamme (ici la chaine de connection à la bdd)
var ChaineConnection = builder.Configuration.GetConnectionString("primaryDb");


// Ajout de la bdd au projet

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // chaine de connection (clic droit sur la BDD -> propriétés)
    options.UseSqlServer(ChaineConnection);
    

});

// service permettant d'effectuer un job toutes les 5 minutes
builder.Services.AddHostedService<BackgroundWorkerService>();


// builder.Services.AddScoped<IMaClasse, MaClasse>();
builder.Services.AddScoped<IHelpersService, HelpersService>();
builder.Services.AddScoped<IResultatService, ResultatService>();
builder.Services.AddScoped<IGrilleService, GrilleService>();

// Add services to the container.  Toujours en dernier dans la partie Services.Add
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
