using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzClustering;

namespace QuartzAspNetCore50
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                   
                    services.AddQuartz(q =>
                    {
                        // base quartz scheduler, job and trigger configuration
                        // Normally would take this from appsettings.json, just show it's possible
                        q.SchedulerName = "Quartz Scheduler Training";
                        q.SchedulerId = "AUTO";

                        // Use a Scoped container for creating IJobs
                        // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                        q.UseMicrosoftDependencyInjectionJobFactory();

                        // default max concurrency is 10
                        q.UseDefaultThreadPool(x => x.MaxConcurrency = 5);
                        // this is the default 
                        q.MisfireThreshold = TimeSpan.FromSeconds(60);

                        q.UsePersistentStore(s =>
                        {
                            // force job data map values to be considered as strings
                            // prevents nasty surprises if object is accidentally serialized and then 
                            // serialization format breaks, defaults to false
                            s.UseProperties = true;
                            // there are other SQL providers supported too 
                            s.UseSqlServer(connectionString);
                            s.UseClustering();
                            // this requires Quartz.Serialization.Json NuGet package
                            s.UseJsonSerializer();
                        });

                        // add the job
                        var jobKey = new JobKey("SanpleJobKey");

                        q.AddJob<SampleJob>(opts => opts.WithIdentity(jobKey));

                       
                        // Run every 1 minute
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity(jobKey.Name + "_trigger") // Important, 
                            .WithSimpleSchedule(x => x
                                .WithIntervalInMinutes(1)
                                .RepeatForever()
                            )
                        );
                    });
                    // ASP.NET Core hosting
                    services.AddQuartzHostedService(q =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        q.WaitForJobsToComplete = true;
                    });
                });
    }
}
