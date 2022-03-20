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
        public static void JudgmentSlash()
        {
            PtCadLib ac = new PtCadLib();

            FirstForm frm = new FirstForm();
            //Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(frm); //在CAD中模态显示窗口
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(frm); //在CAD中非模态显示窗口
        }
    }
}

