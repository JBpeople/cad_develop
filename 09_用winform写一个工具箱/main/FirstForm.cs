using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static main.PtCadLib;
using System.Runtime.InteropServices; //应用[DllImport("user32.dll")]需要
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)

namespace main
{
    public partial class FirstForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        public static extern int SetFocus(IntPtr hWnd); //两个套路
        public FirstForm()
        {
            InitializeComponent(); //初始化窗口
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            PtCadLib ac = new PtCadLib();
            Document acDoc = ac.AcDoc;
            SetFocus(ac.AcDoc.Window.Handle); //切换窗口焦点 非模态显示窗口时需要做该设置
            using (DocumentLock acLokDoc = ac.AcDoc.LockDocument()) //调用非模态窗口必须锁定文档
            {
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
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PtCadLib ac = new PtCadLib();
            Document acDoc = ac.AcDoc;
            Database acCurDb = acDoc.Database;

            SetFocus(ac.AcDoc.Window.Handle);
            using (DocumentLock acLokDoc = acDoc.LockDocument())
            {
                using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTb1;
                    acBlkTb1 = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTb1Rec;
                    acBlkTb1Rec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                    RotatedDimension hdim = new RotatedDimension();
                    hdim.XLine1Point = new Point3d(10, 15, 0);
                    hdim.XLine2Point = new Point3d(20, 20, 0);
                    hdim.Rotation = 0;
                    hdim.DimLinePoint = new Point3d(0, 25, 0);
                    hdim.DimensionStyle = acCurDb.Dimstyle;

                    RotatedDimension vdim = new RotatedDimension();
                    vdim.XLine1Point = new Point3d(20, 15, 0);
                    vdim.XLine2Point = new Point3d(20, 20, 0);
                    vdim.Rotation = Math.PI / 2;
                    vdim.DimLinePoint = new Point3d(280, 0, 0);
                    vdim.DimensionStyle = acCurDb.Dimstyle;

                    acBlkTb1Rec.AppendEntity(hdim);
                    acTrans.AddNewlyCreatedDBObject(hdim, true);
                    acBlkTb1Rec.AppendEntity(vdim);
                    acTrans.AddNewlyCreatedDBObject(vdim, true);
                    acTrans.Commit();
                }
            }
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PtCadLib ac = new PtCadLib();
            Document acDoc = ac.AcDoc;
            Database acCurDb = acDoc.Database;

            SetFocus(acDoc.Window.Handle);
            using (DocumentLock acLokDoc = acDoc.LockDocument())
            {
                Point3d pt1 = ac.GetPoint(">>>请选择第一个点");
                Point3d pt2 = ac.GetPoint(">>>请选择第二个点");
                Point3d pt3 = ac.GetPoint(">>>请选择第三个点");
                if((pt3.Y > pt1.Y)&&(pt3.Y > pt2.Y))
                {
                    double rotation = 0;
                    ac.CreatRotatedDimension(pt1, pt2, rotation, pt3);
                }
                else if((pt3.Y < pt1.Y)&&(pt3.Y < pt2.Y))
                {
                    double rotation = Math.PI;
                    ac.CreatRotatedDimension(pt1, pt2, rotation, pt3);
                }
                else if ((pt3.X < pt1.X) && (pt3.X < pt2.X))
                {
                    double rotation = -Math.PI/2;
                    ac.CreatRotatedDimension(pt1, pt2, rotation, pt3);
                }
                if ((pt3.X > pt1.X) && (pt3.X > pt2.X))
                {
                    double rotation = Math.PI / 2;
                    ac.CreatRotatedDimension(pt1, pt2, rotation, pt3);
                }
            }
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PtCadLib ac = new PtCadLib();
            Document acDoc = ac.AcDoc;

            SetFocus(acDoc.Window.Handle);
            using(DocumentLock acLokDoc = acDoc.LockDocument())
            {
                Point3d pt1 = ac.GetPoint("\n>>>请选择第一个点");
                Point3d pt2 = ac.GetPoint("\n>>>请选择第二个点");

                using(Transaction acTrans = acDoc.TransactionManager.StartTransaction())
                {
                    ObjectId? acEntObjId = ac.GetSelectEntity("\n>>>请指定需要标注的线段图层", null, null);
                    if(acEntObjId != null)
                    {
                        Entity acEnt = acTrans.GetObject((ObjectId)acEntObjId, OpenMode.ForRead) as Entity;
                        string acEntLyrName = acEnt.Layer.ToString();
                        TypedValue[] acTypVal = new TypedValue[2];
                        acTypVal.SetValue(new TypedValue((int)DxfCode.LayerName, acEntLyrName), 0);
                        acTypVal.SetValue(new TypedValue(0, "LINE"), 1);
                        SelectionSet acSS = ac.GetSelectionSet("GetSelection", null, acTypVal);
                        foreach(SelectedObject aSSObj in acSS)
                        {

                        }
                    }

                }
            }

        }
    }
}
