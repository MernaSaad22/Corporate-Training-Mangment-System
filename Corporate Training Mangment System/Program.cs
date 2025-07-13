using DataAccess.Data; 
using Microsoft.EntityFrameworkCore;
using DataAccess.Repository;
using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Service.Utility;
using System.Reflection;
using MapsterMapper;
using Mapster;
using Scalar;
using Scalar.AspNetCore;
using Service.Utility.DBInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Service.DTOs.Response;
namespace Corporate_Training_Mangment_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; /////



            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); //any one ,any country,any methods
                                  });
            });


            builder.Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:7046",
                        ValidAudience = "https://localhost:7046",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Eraa##Team##project##5132025##Merna##Naira##Emad"))


                    };
                });





            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
          


            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            builder.Services.AddScoped<IExamRepository, ExamRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeAssignmentRepository,EmployeeAssignmentRepository>();
            builder.Services.AddScoped<IEmployeeCourseRepository, EmployeeCourseRepository>();

            builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();


            //add this line to make me test CoursesController
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            //add this to view instructorname in course but not work
            //config.ForType<Course, CourseResponse>()
            //.Map(dest => dest.InstructorUserName,
            //src => src.Instructor != null && src.Instructor.ApplicationUser != null
            // ? src.Instructor.ApplicationUser.UserName
            // : string.Empty);


            builder.Services.AddSingleton<IMapper>(new Mapper(config));


            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);
            //add authontication for JWT
            app.UseAuthentication();


            app.UseAuthorization();
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                dbInitializer.Initilize();
            }


            app.MapControllers();

            app.Run();
        }
    }
}
