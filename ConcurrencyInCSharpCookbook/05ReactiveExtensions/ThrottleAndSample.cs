using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace _05ReactiveExtensions {
    //事件因来的太快，导致系统卡死奔溃，需要抑制输入源，所以要限流。
    //Throttle（限流）,Sample（抽样） 用来抑制过快的事件输入
    public class ThrottleAndSample {
        //Throttle: 可以设置超市时间段，当一个事件到达时，会重新计时，到达超时时间内，会把该时间内输入的最后一个事件传递出去
        public static void UseThrottle() {
            var watcher = new FileSystemWatcher("D:\\MS\\Project\\dotnet\\Books\\ConcurrencyInCSharpCookbook\\05ReactiveExtensions", "README.md") { EnableRaisingEvents = true };
            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    handler => (e, a) => handler(e, a),
                    addHandler : handler => watcher.Changed += handler,
                    removeHandler : handler => watcher.Changed -= handler
                )
                .Where(x => x.EventArgs.ChangeType == WatcherChangeTypes.Changed)
                .Throttle(TimeSpan.FromSeconds(5))
                .Subscribe(onNext: x => Console.WriteLine(x.EventArgs.Name), onError: e =>
                    throw e, onCompleted: () => Console.WriteLine(DateTime.Now.Second + " : 已修改"));
        }

        private static void OnChanged(object source, FileSystemEventArgs e) {
            Console.WriteLine("文件重命名事件处理逻辑{0}  {1}  名字 {2}", e.ChangeType, e.FullPath, e.Name);
        }
    }
}