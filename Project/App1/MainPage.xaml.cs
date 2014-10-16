using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace App1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

       /*
            string name = Windows.UI.Colors.Blue.GetType().AssemblyQualifiedName;
            var color = Windows.UI.Colors.Blue;
            var brush = new Windows.UI.Xaml.Media.SolidColorBrush(color);
            _grid.Background = brush;
            

            
            var text = new TextBlock();
            text.Width = 200;
            text.Height = 200;
            text.Text = "abc";
            var collection = _grid.Children;
            var typeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(ICollection<UIElement>));
            var method = typeInfo.GetDeclaredMethod("Add");
            method.Invoke(collection, new object[] { text });
            

         //   var method = typeof(ICollection<UIElement>).GetTypeInfo().GetDeclaredMethod("Add");
            
            //★もし引っかからなかったときは、これでとる！
            var v = _grid.Children.GetType().GetTypeInfo();
            foreach(var e in v.ImplementedInterfaces)
            {
                var ee = e.GetTypeInfo();
                var m = ee.GetDeclaredMethod("Add");
                if (m != null)
                {
                }
            }*/
        }

        Type GetX()
        {
            return typeof(ICollection<UIElement>);
        }
        Type GetY()
        {
            return _grid.Children.GetType();
        }
        void SetInfo(TypeInfo i)
        {
        }

        void Func()
        {

        }
    }
}
