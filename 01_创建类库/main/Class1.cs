using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Colors;
using static main.PtCadLib;

namespace main
{
    public class Class1
    {
        [CommandMethod("aa")]
        public static void Test1()
        {
            PtCadLib myCad = new PtCadLib();

            //调整窗口定位及大小
            Point ptApp = new Point(300, 300);
            Size szApp = new Size(600, 600);
            myCad.PositionApplicationWindow(ptApp, szApp);
            Thread.Sleep(2000);

            //最大化窗口，最小化窗口
            myCad.MinApplicationWindow();
            Thread.Sleep(1000);
            myCad.MaxApplicationWindow();

            //创建一个图层
            myCad.CreatLayer("3", 2);

            //命令行输出文字
            myCad.WriteMessage("我要开始装逼了！！！");

            //创建一个圆
            myCad.CreatCirecle(new Point3d(100, 100, 0), 160, 2, "3");

            //创建一条线
            myCad.CreatLine(new Point3d(100, 100, 0), new Point3d(555, 231, 0), 23, "232");

            //创建一个单行文字
            myCad.CreatDbText("我爱你", new Point3d(520, 520, 520), 300, "0");
        }

        [CommandMethod("bb")]
        public static void Test2()
        {
            PtCadLib mycad = new PtCadLib();

            //获取用户输入
            string res1 = mycad.GetString("请输入一个字：", true);
            Application.ShowAlertDialog(res1);

            //获取用户输入点链接成线
            Point3d sPt = mycad.GetPoint(">>>请选择一个起点");
            Point3d ePt = mycad.GetPoint(">>>请选择一个终点");
            mycad.CreatLine(sPt, ePt, 233, "12345");

            //强制用户输入关键字
            string res2 = mycad.GetKeyword(">>>请输入一个数字", "1", "2", "3");
            Application.ShowAlertDialog(res2);

        }

        [CommandMethod("cc")]
        public static void Test3()
        {
            PtCadLib mycad = new PtCadLib();
            SelectionSet myset = mycad.GetSelectObjects();
            if (myset != null)
            {
                mycad.WriteMessage(myset.Count.ToString());
            }
            else
            {
                mycad.WriteMessage("0");
            }
        }

        [CommandMethod("dd")]
        public static void Test4()
        {
            PtCadLib mycad = new PtCadLib();

            ObjectId myId = mycad.GetSelectEntity("》》》请选择一条直线", "选择错误", typeof(Line));
            mycad.WriteMessage(myId.ToString());
        }


    }
}

