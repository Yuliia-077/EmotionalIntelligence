using EmotionalIntelligence.Commands;
using EmotionalIntelligence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace EmotionalIntelligence
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<DataContext>(opt => opt.UseNpgsql(_configuration.GetConnectionString("Db")), ServiceLifetime.Singleton);
            services.AddSingleton<TelegramBot>();
            services.AddSingleton<ICommandExecutor, CommandExecutor>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<BaseCommand, StartCommand>();
            services.AddSingleton<BaseCommand, GetTestInfoCommand>();
            services.AddSingleton<BaseCommand, StartTestCommand>();
            services.AddSingleton<BaseCommand, GetQuestionCommand>();
            services.AddSingleton<BaseCommand, SetAnswerCommand>();
            services.AddSingleton<BaseCommand, FinishOrStopCommand>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
