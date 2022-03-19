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
        [CommandMethod("getslash")]
        public static void JudgmentSlash()
        {
            PtCadLib ac = new PtCadLib();
            Document acDoc = ac.AcDoc;

            //获取用户选择线段的ObjectId
            ObjectId? acObjId = ac.GetSelectEntity(">>>请选择指定线段图层", null, null);

            //判断用户是否取消命令
            if (acObjId != null)
            {
                using (Transaction acTrans = acDoc.TransactionManager.StartTransaction()) //启动事务
                {
                    Entity acEnt = acTrans.GetObject((ObjectId)acObjId, OpenMode.ForRead) as Entity; //获得图元
                    string acEntLyr = acEnt.Layer; //获得图元的图层名称

                    //设置过滤条件
                    TypedValue[] acTypVal = new TypedValue[1];
                    acTypVal.SetValue(new TypedValue((int)DxfCode.LayerName, acEntLyr), 0);

                    //获取选择集
                    ac.WriteMessage("\n>>>请框选需要判断的线段");
                    SelectionSet acSS = ac.GetSelectionSet("GetSelection", null, acTypVal);
                    if (acSS != null) //判断选择集是否为空
                    {
                        foreach (SelectedObject acSObj in acSS)
                        {
                            if (acSObj != null) //判断选择集中的项目是否正常
                            {
                                Curve acCurve = acTrans.GetObject(acSObj.ObjectId, OpenMode.ForRead) as Curve;
                                //获取线段起始点和终点的X，Y坐标
                                string sPtX = acCurve.StartPoint.X.ToString("0.00");
                                string sPtY = acCurve.StartPoint.Y.ToString("0.00");
                                string ePTX = acCurve.EndPoint.X.ToString("0.00");
                                string ePTY = acCurve.EndPoint.Y.ToString("0.00");
                                if ((sPtX != ePTX) && (sPtY != ePTY)) //判断线段是否为斜线
                                {
                                    Extents3d acExt = acCurve.GeometricExtents; //获取斜线的包围盒
                                    Point3d pt1 = acExt.MaxPoint;
                                    Point3d pt2 = acExt.MinPoint;

                                    //创建多段线框住斜线
                                    ac.CreatPolyLine(1, "斜线", 2, true, new Point2d(pt1.X + 5, pt1.Y + 5), new Point2d(pt1.X + 5, pt2.Y - 5), new Point2d(pt2.X - 5, pt2.Y - 5), new Point2d(pt2.X - 5, pt1.Y + 5));
                                }
                            }
                        }
                    }
                    acTrans.Commit(); //提交事务
                }
            }
        }
    }
}

