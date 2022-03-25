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

        private void button1_Click(object sender, EventArgs e)
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

        }
    }
}
