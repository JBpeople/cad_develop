using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline) 
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d) 
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document) 
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag) 
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using Autodesk.AutoCAD.Interop;  //引用需要添加 Autodesk.AutoCAD.Interop.dll  CAD安装目录内

[assembly: ExtensionApplication(typeof(Ptcaddebug.AcadNetloadX))] //启动时加载工具栏，注意typeof括号里的类库名
namespace Ptcaddebug
{
    //项目类记得添加引用 都是在安装目录
    public class AcadNetloadX : Autodesk.AutoCAD.Runtime.IExtensionApplication
    {
        //重写初始化
        public void Initialize()
        {
            //加载后初始化的程序放在这里 这样程序一加载DLL文件就会执行
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\n>>>自动加载C#主动调试菜单栏");
            //在此处加载 菜单栏程序
            addmenu();
        }

        //重写结束
        public void Terminate()
        {
            // do somehing to cleanup resource
        }

        //加载菜单栏
        public void addmenu()
        {

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            AcadApplication acadApp = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication as AcadApplication;
            AcadPopupMenu PTaddmenu = null;  //新建菜单栏的对象
            // 创建菜单
            if (PTaddmenu == null)
            {
                PTaddmenu = acadApp.MenuGroups.Item(0).Menus.Add("PT调试菜单");
                PTaddmenu.AddMenuItem(PTaddmenu.Count, "加载默认文件", "netloadX "); //注意 每个快捷键后面一个空格，相当于咱们输入命令按空格
                PTaddmenu.AddSeparator(PTaddmenu.Count); //加入分割符号
                PTaddmenu.AddMenuItem(PTaddmenu.Count, "手动选择加载", "netloadY ");
            }

            // 菜单是否显示  看看已经显示的菜单栏里面有没有这一栏
            bool isShowed = false;  //初始化没有显示
            foreach (AcadPopupMenu menu in acadApp.MenuBar)  //遍历现有所有菜单栏
            {
                if (menu == PTaddmenu)
                {
                    isShowed = true;
                    break;
                }
            }

            // 显示菜单 加载自定义的菜单栏
            if (!isShowed)
            {
                PTaddmenu.InsertInMenuBar(acadApp.MenuBar.Count);
            }

        }
    }


    public class mainclass
    {
        //默认路径下的dll文件加载
        [CommandMethod("netloadx")]
        public void netloadx()
        {

            //当前加载的DLL路径
            string dllfilepath = Assembly.GetExecutingAssembly().Location;
            string filepath = Path.GetDirectoryName(dllfilepath) + "\\mydll.txt"; //dll 所在文件夹下有一个mydll.txt文件
            //读取mydll.txt文件，获取需要加载的dll文件路径
            StreamReader srTxt = new StreamReader(filepath, System.Text.Encoding.Default);
            string file_dir = srTxt.ReadLine(); //读取第一行即可 第一行就是dll文件路径

            byte[] buffer = System.IO.File.ReadAllBytes(file_dir);
            //加载内存中的文件
            Assembly assembly = Assembly.Load(buffer);
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\n>>>成功重新加载:" + file_dir);
            doc.Editor.WriteMessage("\n>>>快捷键需重新定义，否则会失效");
        }

        //手动选择
        [CommandMethod("netloady")]
        public void netloady()
        {
            string file_dir = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "dll文件(*.dll)|*.dll";
            ofd.Title = "打开dll文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                file_dir = ofd.FileName;
            }
            else return;
            //打开文件，将文件以二进制方式复制到内存，自动关闭文件
            byte[] buffer = System.IO.File.ReadAllBytes(file_dir);
            //加载内存中的文件
            Assembly assembly = Assembly.Load(buffer);  //在内存中加载
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\n>>>成功重新加载:" + file_dir);
            doc.Editor.WriteMessage("\n>>>快捷键需重新定义，否则会失效");
        }
    }


}

