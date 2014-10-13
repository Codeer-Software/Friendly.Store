using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeer.Friendly.Store;
using Codeer.Friendly.Dynamic;
using System.Diagnostics;
using Codeer.Friendly;

namespace StoreAppTest
{
    [TestClass]
    public class SmokeTest
    {
        StoreAppFriend _app;
        Process _process;

        [TestInitialize]
        public void TestInitialize()
        {
            //アタッチ
            string name = "App1";
            _app = new StoreAppFriend(Process.GetProcessesByName(name)[0]);
            _process = Process.GetProcessById(_app.ProcessId);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _app.Dispose();
        }

        [TestMethod]
        public void Test()
        {
            //①メイン画面取得
            dynamic current = _app.Type().Windows.UI.Xaml.Window.Current;
            dynamic main = current.Content.Content;

            //②メソッド直呼び
            main.Func();
            main.Func(new Async());
            /*
            //③グリッド取得
            dynamic grid = main.Content;

            //④背景色を変える これまでWindows.UI.Colorsの入ったdllがロードされていたら成功
            dynamic color = _app.Type().Windows.UI.Colors.Blue;
            dynamic brush = _app.Type().Windows.UI.Xaml.Media.SolidColorBrush(color);
            grid.Background = brush;
            

            //⑤ボタンを一つ取り付ける
            dynamic button = _app.Type().Windows.UI.Xaml.Controls.Button();
            button.Content = "新たなボタン";
            main.Add(button);*/
        }
    }
}
