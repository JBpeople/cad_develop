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
            Database acCurDb = mycad.AcDoc.Database;

            //输出信息
            mycad.WriteMessage(">>>开始文字对齐功能");

            //选择一个基准点
            Point3d pt = mycad.GetPoint(">>>请选择一个基准点");
            //设置过滤条件
            TypedValue[] typedValue = new TypedValue[1];
            typedValue.SetValue(new TypedValue(0, "TEXT"), 0);
            SelectionSet acSSet = mycad.GetSelectionSet("GetSelection", null, typedValue);

            //判断选择集内容
            if(acSSet != null)
            {
                foreach (SelectedObject mySObj in acSSet)
                {
                    if(mySObj != null)
                    {
                        using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                        {
                            DBText myText = acTrans.GetObject(mySObj.ObjectId, OpenMode.ForWrite) as DBText;
                            //设置对齐方式
                            myText.HorizontalMode = TextHorizontalMode.TextMid;
                            Point3d myPt = myText.Position;
                            myText.AlignmentPoint = new Point3d(pt.X, myPt.Y, 0);
                            acTrans.Commit();
                        }
                    }

                }
            }
        }
    }
}

