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

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Test()
        {
            string name = "App1";
            var app = new StoreAppFriend(Process.GetProcessesByName(name)[0]);

            //Get MainPage
            dynamic current = app.Type().Windows.UI.Xaml.Window.Current;
            dynamic main = current.Content.Content;

            //Get Grid
            dynamic grid = main.Content;

            //Add Button
            dynamic button = app.Type().Windows.UI.Xaml.Controls.Button();
            button.Content = "New Button";
            grid.Children.Add(button);

            //Change Background
            dynamic color = app.Type().Windows.UI.Colors.Blue;
            dynamic brush = app.Type().Windows.UI.Xaml.Media.SolidColorBrush(color);
            grid.Background = brush;

            //InvokeMethod
            Assert.AreEqual("100", main.Func(100).ToString());

            app.Dispose();
        }
    }
}
