参考文章来源:https://www.cnblogs.com/wenhx/p/ioptions-ioptionsmonitor-and-ioptionssnapshot.html#GeneratedCaptionsTabForHeroSec

结论
IOptions<>是单例，因此一旦生成了，除非通过代码的方式更改，它的值是不会更新的。
IOptionsMonitor<>也是单例，但是它通过IOptionsChangeTokenSource<> 能够和配置文件一起更新，也能通过代码的方式更改值。
IOptionsSnapshot<>是范围，所以在配置文件更新的下一次访问，它的值会更新，但是它不能跨范围通过代码的方式更改值，只能在当前范围（请求）内有效。

官方文档是这样介绍的：
IOptionsMonitor<TOptions>用于检索选项和管理TOptions实例的选项通知，它支持下面的场景：

实例更新通知。
命名实例。
重新加载配置。
选择性的让实例失效。
IOptionsSnapshot<TOptions>在需要对每个请求重新计算选项的场景中非常有用。
IOptions<TOptions>可以用来支持Options模式，但是它不支持前面两者所支持的场景，如果你不需要支持上面的场景，你可以继续使用IOptions<TOptions>。

所以你应该根据你的实际使用场景来选择到底是用这三者中的哪一个。
一般来说，如果你依赖配置文件，那么首先考虑IOptionsMonitor<>，如果不合适接着考虑IOptionsSnapshot<>，最后考虑IOptions<>。
有一点需要注意，在ASP.NET Core应用中IOptionsMonitor可能会导致同一个请求中选项的值不一致——当你正在修改配置文件的时候——这可能会引发一些奇怪的bug。
如果这个对你很重要，请使用IOptionsSnapshot，它可以保证同一个请求中的一致性，但是它可能会带来轻微的性能上的损失。
如果你是在app启动的时候自己构造Options（比如在Startup类中）：

services.Configure<TestOptions>(opt => opt.Name = "Test 0");
IOptions<>最简单，也许是一个不错的选择，Configure扩展方法还有其他重载可以满足你的更多需求。
