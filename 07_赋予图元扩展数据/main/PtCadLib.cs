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

        /// <summary>
        /// 获取当前编辑文档窗口
        /// </summary>
        public Document AcDoc
        {
            get { return this.acDoc; }
            set { this.acDoc = value; }
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
        public ObjectId? GetSelectEntity(string message, string rejectMessage, Type entType)
        {
            PromptEntityOptions pEntOpts = new PromptEntityOptions(message);
            if (entType != null)
            {
                pEntOpts.SetRejectMessage(rejectMessage);
                pEntOpts.AddAllowedClass(entType, true);
            }
            PromptEntityResult pEntRes = acDocEd.GetEntity(pEntOpts);
            if (pEntRes.Status == PromptStatus.Cancel)
            {
                return null;
            }
            return pEntRes.ObjectId;
        }

        /// <summary>
        /// 获取用户选择集
        /// </summary>
        /// <returns></returns>
        public SelectionSet GetSelectionSet(string selectType, Point3dCollection ptCollection, TypedValue[] acTypValAr)
        {
            {
                //将过滤条件赋值给SelectionFilter对象
                SelectionFilter acSelFtr = null;
                if (acTypValAr != null)
                {
                    acSelFtr = new SelectionFilter(acTypValAr);
                }
                //识别输入的选择模式
                PromptSelectionResult acSPtRes;
                if (selectType == "GetSelection")
                { acSPtRes = acDocEd.GetSelection(acSelFtr); }
                else if (selectType == "SelectAll")
                { acSPtRes = acDocEd.SelectAll(acSelFtr); }
                else if (selectType == "SelectCrossingPolygon")
                { acSPtRes = acDocEd.SelectCrossingPolygon(ptCollection, acSelFtr); }
                else if (selectType == "SelectFence")
                { acSPtRes = acDocEd.SelectCrossingPolygon(ptCollection, acSelFtr); }
                else if (selectType == "SelectWindowPolygon")
                { acSPtRes = acDocEd.SelectWindowPolygon(ptCollection, acSelFtr); }
                else if (selectType == "SelectCrossingWindow")
                {
                    Point3d pt1 = ptCollection[0];
                    Point3d pt2 = ptCollection[1];
                    acSPtRes = acDocEd.SelectCrossingWindow(pt1, pt2, acSelFtr);
                }
                else if (selectType == "SelectWindow")
                {
                    Point3d pt1 = ptCollection[0];
                    Point3d pt2 = ptCollection[1];
                    acSPtRes = acDocEd.SelectWindow(pt1, pt2, acSelFtr);
                }
                else { return null; }
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

        //**********************************扩展CAD数据*************************************************

        /// <summary>
        /// 添加扩展数据
        /// </summary>
        /// <param name="objid">图元ObjectId</param>
        /// <param name="resBuf">扩展数据/param>
        public bool SetEntityXdata(ObjectId objid, ResultBuffer resBuf)
        {
            //读取 ResultBuffer 里面应用程序的名称
            //ResultBuffer 没有办法读取其数值，将其转化成一个数据
            string appname = null;
            TypedValue[] tvs = resBuf.AsArray();
            for (int i = 0; i < tvs.GetLength(0); i++)
            {
                if (tvs[i].TypeCode == (int)DxfCode.ExtendedDataRegAppName)
                {
                    appname = tvs[i].Value.ToString();
                    break;
                }
            }

            if (objid != null && appname != null)
            {
                Database acCurDb = acDoc.Database;
                //启动新事物
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    Entity myEnt = acTrans.GetObject(objid, OpenMode.ForWrite) as Entity;
                    //判断应用程序名是否已经在当前文档中注册，没有注册则重新注册
                    RegAppTable appTbl = acTrans.GetObject(acCurDb.RegAppTableId, OpenMode.ForWrite) as RegAppTable;
                    if (!appTbl.Has(appname))
                    {
                        RegAppTableRecord appTblRcd = new RegAppTableRecord();
                        appTblRcd.Name = appname;
                        appTbl.Add(appTblRcd);
                        acTrans.AddNewlyCreatedDBObject(appTblRcd, true);
                    }
                    //扩展数据添加到实体
                    myEnt.XData = resBuf;
                    acTrans.Commit();
                }
                return true;
            }
            else
            { return false; }
        }

        /// <summary>
        /// 获取图元扩展属性
        /// </summary>
        /// <param name="objId">图元objectId</param>
        /// <returns></returns>
        public TypedValue[] GetEntityXdata(ObjectId objId)
        {

            if (objId != null)
            {
                Database acCurDb = acDoc.Database;
                //启动新事物
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    Entity myent = acTrans.GetObject(objId, OpenMode.ForWrite) as Entity;

                    //获取附加属性
                    ResultBuffer resBuf = myent.XData;
                    acTrans.Commit();
                    if (resBuf != null)
                    {
                        return resBuf.AsArray();//转化为数组输出 因为 ResultBuffer 无法在程序里便捷的读出来
                    }
                    else { return null; }
                }

            }
            else
            { return null; }


        }

        /// <summary>
        /// 写入命名字典
        /// </summary>
        /// <param name="dictname">字典名称</param>
        /// <param name="resBuf">自定义数据</param>
        public void WriteselfDictionary(string dictname, ResultBuffer resBuf)
        {
            Database acCurDb = acDoc.Database;
            //启动事物
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 命名对象字典 字典数据表
                DBDictionary nod = acTrans.GetObject(acCurDb.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                // 自定义数据
                Xrecord myXrecord = new Xrecord();
                myXrecord.Data = resBuf;
                // 往命名对象字典中存储自定义数据
                nod.SetAt(dictname, myXrecord);

                acTrans.AddNewlyCreatedDBObject(myXrecord, true);
                acTrans.Commit();  //提交确定
            }
        }

        /// <summary>
        /// 读取命名字典数据
        /// </summary>
        /// <param name="dictname">字典名称</param>
        /// <returns></returns>
        public TypedValue[] ReadselfDictionary(string dictname)
        {
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 命名对象字典数据表
                DBDictionary nod = acTrans.GetObject(acCurDb.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                // 查找自定义数据
                if (nod.Contains(dictname))
                {
                    ObjectId myDataId = nod.GetAt(dictname);
                    Xrecord myXrecord = acTrans.GetObject(myDataId, OpenMode.ForRead) as Xrecord;
                    ResultBuffer resBuf = myXrecord.Data;

                    acTrans.Commit();
                    if (resBuf != null)
                    {
                        return resBuf.AsArray();//转化为数组输出 因为 ResultBuffer 无法在程序里便捷的读出来
                    }
                    else { return null; }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 读取命名字典
        /// </summary>
        /// <param name="dictName">字典名称</param>
        /// <param name="noDxfcode">组码</param>
        /// <returns></returns>
        public object ReadselfDictionarybydxfcode(string dictName, int noDxfcode)
        {
            Database acCurDb = acDoc.Database;
            object myResult = null;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 命名对象字典
                DBDictionary nod = acTrans.GetObject(acCurDb.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                // 查找自定义数据
                if (nod.Contains(dictName))
                {
                    ObjectId myDataId = nod.GetAt(dictName);
                    Xrecord myXrecord = acTrans.GetObject(myDataId, OpenMode.ForRead) as Xrecord;
                    foreach (TypedValue tv in myXrecord.Data)
                    {
                        if (tv.TypeCode == noDxfcode)
                        {
                            myResult = tv.Value;
                            break;
                        }
                    }
                    return myResult;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
