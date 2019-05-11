using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] idArray = new string[] { "201905071745579909112", "201905071751531604842", "201905071751295079136", "201905071745091534582", "201905081643008172062", "201905071750460379684", "201905071749396235309", "201905071748575436667", "201905071748167808662", "201905071747289532850", "201905071746528776026", "201905071743598434924", "201905081112293978739", "201905081111498837528", "201905081111141709428", "201905081110401705122", "201905081109495338000", "201905081107546806945", "201905081108571408692", "201905081109236133336", "201905071751531608439", "201905071748575431034", "201905071748167809294", "201905091110498174959", "201905091111428401499", "201905091112283805978", "201905091113199133415", "201905091114404671227", "201905091116097407714", "201905091116370377279", "201905091114586604930", "201905091114404674752", "201905091114586604061", "201905091115303737581", "201905091116097404198", "201905091116370376458", "201905091115303737504" };
            string[] oldArray = new string[] { "407714", "377279", "737504", "604930", "534582", "379684", "235309", "436667", "808662", "532850", "776026", "434924", "172062", "909112", "604842", "079136", "108535", "075802", "705122", "806945", "338000", "133336", "408692", "431034", "608439", "809294", "978739", "837528", "709428", "177027", "408908", "809367", "135885", "674752", "604061", "737581", "404198", "376458", "174959", "401499", "805978", "133415", "671227", };

            string empty = string.Empty;
            foreach (var item in oldArray)
            {
                bool isFind = false;
                foreach (var idStr in idArray)
                {
                    if (idStr.EndsWith(item))
                    {
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    empty += item+",";
                }
            }

            this.tbInfo.Text = empty;
        }
    }
}
