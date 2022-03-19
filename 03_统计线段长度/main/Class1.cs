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
            PtCadLib mycad = new PtCadLib();
            Document acDoc = mycad.AcDoc;

            //设置过滤条件
            TypedValue[] typedValues = new TypedValue[2];
            typedValues.SetValue(new TypedValue(0, "*LINE"), 0); //限制只能选择线
            typedValues.SetValue(new TypedValue(8, "PT1*"), 1); //限制图层名
            //获取选择集
            SelectionSet mySS = mycad.GetSelectionSet("GetSelection", null, typedValues);
            //定义一个总长度变量
            double sumLength = 0;
            //判断选择集是否为空
            if(mySS != null)
            {
                foreach(SelectedObject ssObj in mySS)
                {
                    if(ssObj != null) //判断选择项目是否无误
                    {
                        using(Transaction acTrans = acDoc.TransactionManager.StartTransaction()) //启动事务
                        {
                            Curve myEnt = acTrans.GetObject(ssObj.ObjectId, OpenMode.ForRead) as Curve;
                            //获取线段终点
                            Point3d endPt = myEnt.EndPoint;
                            //获取线段长度
                            double length = myEnt.GetDistAtPoint(endPt);
                            sumLength += length;
                            acTrans.Commit();
                        }
                    }
                }
                //输出信息
                mycad.WriteMessage("\n所有线段长度为：" + sumLength);

            }
        }
    }
}

