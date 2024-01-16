using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using Microsoft.Extensions.Options;

namespace OptionsTest
{
    public static class OptionConfig
    {
        public static IServiceCollection AddJsonPrivideOptions<T>(this IServiceCollection Services,string ConfigKey) where T : class
        {
            var configBulider = new ConfigurationBuilder().AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), optional: true, reloadOnChange: true);

            var configuration = configBulider.Build();
            Services.AddOptions();
            Services.Configure<T>(configuration.GetSection(ConfigKey));

            var servicesProvider = Services.BuildServiceProvider()!;
            Console.WriteLine("修改前：");
            Print<T>(servicesProvider);

            #region 调试
            //Change(servicesProvider); //使用代码修改Options值。
            //Console.WriteLine("使用代码修改后：");
            //Print<T>(servicesProvider);

            //Console.WriteLine("请修改配置文件。");
            //Console.ReadLine(); //等待手动修改appsettings.json配置文件。
            //Console.WriteLine("修改appsettings.json文件后：");
            //Print<T>(servicesProvider); 
            #endregion

            //监控文件变化触发
            var opMonitor = servicesProvider.GetService<IOptionsMonitor<T>>();
            WatchOptionsMonitorChange(opMonitor!, ConfigKey);
            return Services;
        }

        public static void WatchOptionsMonitorChange<T>(IOptionsMonitor<T> optionsMonitor,string ConfigKey) where T : class
        {
            // 注册配置变化的回调
            optionsMonitor.OnChange(updatedOptions =>
            {
                Debugger.Break();
                // 处理配置变化
                Console.WriteLine($"wathch Change {ConfigKey} Options-------------:{Serialize(updatedOptions)}");
            });
        }

        private static void Print<T>(IServiceProvider provider) where T : class
        {
            using (var scope = provider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var options1 = sp.GetRequiredService<IOptions<T>>();
                var options2 = sp.GetRequiredService<IOptionsMonitor<T>>();
                var options3 = sp.GetRequiredService<IOptionsSnapshot<T>>();

                Console.WriteLine($"IOptions值-------------:{Serialize(options1.Value)}");
                Console.WriteLine($"IOptionsMonitor值-------------:{Serialize(options2.CurrentValue)}");
                Console.WriteLine($"IOptionsSnapshot值-------------:{Serialize(options3.Value)}");

                Console.WriteLine();
            }
        }

        private static void Change(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                sp.GetRequiredService<IOptions<TestOptions>>().Value.Name = "IOptions Test 1";
                sp.GetRequiredService<IOptionsMonitor<TestOptions>>().CurrentValue.Name = "IOptionsMonitor Test 1";
                sp.GetRequiredService<IOptionsSnapshot<TestOptions>>().Value.Name = "IOptionsSnapshot Test 1";
            }
        }

        public static string Serialize(object value) 
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(value, options);
        }
    }
}
