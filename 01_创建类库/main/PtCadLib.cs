using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Windows;

namespace main
{
    public class PtCadLib
    {
        Document acDoc;
        Editor acDocEd;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PtCadLib()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
        }

        //**********************************修改CAD窗口*************************************************

        /// <summary>
        /// 调整CAD窗口定位及大小
        /// </summary>
        /// <param name="ptApp">屏幕定位</param>
        /// <param name="szApp">像素大小</param>
        public void PositionApplicationWindow(Point ptApp, Size szApp)
        {
            Application.MainWindow.SetLocation(ptApp);
            Application.MainWindow.SetSize(szApp);
        }

        /// <summary>
        /// CAD窗口最大化
        /// </summary>
        public void MaxApplicationWindow()
        {
            Application.MainWindow.WindowState = Window.State.Maximized;
        }

        /// <summary>
        /// CAD窗口最小化
        /// </summary>
        public void MinApplicationWindow()
        {
            Application.MainWindow.WindowState = Window.State.Minimized;
        }

        //**********************************编辑CAD实体*************************************************

        /// <summary>
        /// 在命令行输出信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public void WriteMessage(string message)
        {
            acDocEd.WriteMessage(message);
        }

        /// <summary>
        /// 创建一条直线
        /// </summary>
        /// <param name="sPt">起点</param>
        /// <param name="ePt">终点</param>
        /// <param name="color">颜色</param>
        /// <param name="layer">图层</param>
        public ObjectId CreatLine(Point3d sPt, Point3d ePt, int color, string layer)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable acBlikTb1;
                acBlikTb1 = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlikTb1Rec;
                acBlikTb1Rec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                CreatLayer(layer);
                Line acLine = new Line(sPt, ePt);
                acLine.ColorIndex = color;
                acLine.Layer = layer;

                acBlikTb1Rec.AppendEntity(acLine);
                acTrans.AddNewlyCreatedDBObject(acLine, true);
                acTrans.Commit();

                return acLine.ObjectId;
            }
        }

        /// <summary>
        /// 创建一个多段线
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="layer">图层</param>
        /// <param name="width">线宽</param>
        /// <param name="closeFlag">是否闭合</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public ObjectId CreatPolyLine(int color, string layer, double width, bool closeFlag, params Point2d[] point)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable acBlikTb1;
                acBlikTb1 = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlikTb1Rec;
                acBlikTb1Rec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                Polyline acPolyLine = new Polyline();
                for (int i = 0; i < point.Count(); i++)
                {
                    acPolyLine.AddVertexAt(i, point[i], 0, 0, 0);
                }
                CreatLayer(layer);
                acPolyLine.ColorIndex = color;
                acPolyLine.Layer = layer;
                acPolyLine.ConstantWidth = width;
                acPolyLine.Closed = closeFlag;

                acBlikTb1Rec.AppendEntity(acPolyLine);
                acTrans.AddNewlyCreatedDBObject(acPolyLine, true);
                acTrans.Commit();

                return acPolyLine.ObjectId;
            }
        }

        /// <summary>
        /// 创建一个圆
        /// </summary>
        /// <param name="centerPoint">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="color">颜色</param>
        /// <param name="layer">图层</param>
        public ObjectId CreatCirecle(Point3d centerPoint, double radius, int color, string layer)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTb1;
                acBlkTb1 = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTb1Rec;
                acBlkTb1Rec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                CreatLayer(layer);
                Circle acCircle = new Circle();
                acCircle.Center = centerPoint;
                acCircle.Radius = radius;
                acCircle.ColorIndex = color;
                acCircle.Layer = layer;

                acBlkTb1Rec.AppendEntity(acCircle);
                acTrans.AddNewlyCreatedDBObject(acCircle, true);
                acTrans.Commit();

                return acCircle.ObjectId;
            }
        }

        /// <summary>
        /// 创建单行文字
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="position">定位</param>
        /// <param name="fontHeight">字高</param>
        /// <param name="layer">图层</param>
        public ObjectId CreatDbText(string content, Point3d position, int fontHeight, string layer)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable acBlikTb1;
                acBlikTb1 = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlikTb1Rec;
                acBlikTb1Rec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                CreatLayer(layer);
                DBText acText = new DBText();
                acText.TextString = content;
                acText.Position = position;
                acText.Height = fontHeight;
                acText.Layer = layer;

                acBlikTb1Rec.AppendEntity(acText);
                acTrans.AddNewlyCreatedDBObject(acText, true);
                acTrans.Commit();

                return acText.ObjectId;
            }
        }

        /// <summary>
        /// 创建一个图层
        /// </summary>
        /// <param name="layerName">图层名</param>
        /// <param name="color">图层颜色</param>
        public ObjectId CreatLayer(string layerName)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTb1;
                acLyrTb1 = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLyrTb1.Has(layerName) == false)
                {
                    LayerTableRecord acLyrTb1Rec = new LayerTableRecord();
                    acLyrTb1Rec.Name = layerName;

                    acLyrTb1.UpgradeOpen();

                    acLyrTb1.Add(acLyrTb1Rec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTb1Rec, true);

                }
                acTrans.Commit();

                return acLyrTb1.ObjectId;
            }

        }
        public ObjectId CreatLayer(string layerName, short color)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTb1;
                acLyrTb1 = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLyrTb1.Has(layerName) == false)
                {
                    LayerTableRecord acLyrTb1Rec = new LayerTableRecord();
                    acLyrTb1Rec.Name = layerName;
                    acLyrTb1Rec.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, color);

                    acLyrTb1.UpgradeOpen();

                    acLyrTb1.Add(acLyrTb1Rec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTb1Rec, true);

                }
                acTrans.Commit();

                return acLyrTb1.ObjectId;
            }

        }

        //**********************************获取CAD交互*************************************************

        /// <summary>
        /// 返回用户输入的字符串
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="allowSpace">是否允许输入空格</param>
        /// <returns></returns>
        public string GetString(string message, bool allowSpace)
        {
            PromptStringOptions pStrOpts = new PromptStringOptions(message);
            pStrOpts.AllowSpaces = allowSpace;
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);
            return pStrRes.ToString();
        }

        /// <summary>
        /// 返回用户输入的整数
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="defaultValueFlag">是否指定默认值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public int GetInt(string message, bool defaultValueFlag, int defaultValue)
        {
            PromptIntegerOptions pIntOpts = new PromptIntegerOptions(message);
            if (defaultValueFlag)
            {
                pIntOpts.UseDefaultValue = defaultValueFlag;
                pIntOpts.DefaultValue = defaultValue;
            }
            PromptIntegerResult pIntRes = acDocEd.GetInteger(pIntOpts);
            return pIntRes.Value;
        }

        /// <summary>
        /// 返回用户点击坐标的Point3d
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        public Point3d GetPoint(string message)
        {
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions(message);

            pPtRes = acDoc.Editor.GetPoint(pPtOpts);
            return pPtRes.Value;
        }

        /// <summary>
        /// 返回用户输入关键字的字符串
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public string GetKeyword(string message, params string[] options)
        {
            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions(message);
            foreach (string option in options)
            {
                pKeyOpts.Keywords.Add(option);
            }
            pKeyOpts.AllowNone = false;

            PromptResult pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
            return pKeyRes.ToString();
        }

        /// <summary>
        /// 返回用户选择图元的ObjectId
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="rejectMessage">选择错误信息</param>
        /// <param name="entType">图元种类</param>
        /// <returns></returns>
        public ObjectId GetSelectEntity(string message, string rejectMessage, Type entType)
        {
            PromptEntityOptions pEntOpts = new PromptEntityOptions(message);
            if (entType != null)
            {
                pEntOpts.SetRejectMessage(rejectMessage);
                pEntOpts.AddAllowedClass(entType, true);
            }
            PromptEntityResult pEntRes = acDocEd.GetEntity(pEntOpts);
            return pEntRes.ObjectId;
        }

        /// <summary>
        /// 获取用户选择集
        /// </summary>
        /// <returns></returns>
        public SelectionSet GetSelectObjects(string selectType, Point3dCollection ptCollection, params TypedValue[] acTypValAr)
        {
            {
                //将过滤条件赋值给SelectionFilter对象
                SelectionFilter acSelFtr = null;
                if (acTypValAr != null)
                {
                    acSelFtr = new SelectionFilter(acTypValAr);
                }
                //识别输入的选择模式
                PromptSelectionResult acSPtRes = null;
                switch (selectType)
                {
                    case "GetSelection": acSPtRes = acDocEd.GetSelection(acSelFtr); break;
                    case "SelectAll": acSPtRes = acDocEd.SelectAll(acSelFtr); break;
                    case "SelectCrossingPolygon": acSPtRes = acDocEd.SelectCrossingPolygon(ptCollection, acSelFtr); break;
                    case "SelectCrossingWindow": acSPtRes = acDocEd.SelectCrossingWindow(ptCollection[0], ptCollection[1], acSelFtr); break;
                    case "SelectWindow": acSPtRes = acDocEd.SelectWindow(ptCollection[0], ptCollection[1], acSelFtr); break;
                }
                //判断用户有无选择对象
                if (acSPtRes.Status == PromptStatus.OK)
                {
                    return acSPtRes.Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
