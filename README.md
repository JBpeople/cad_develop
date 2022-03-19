# CAD_Develop
 使用C#语言给CAD做一些插件开发。

代码测试平台为VS2022，NetFramework4.7和2020版CAD，不同版本的CAD所支持的NetFramework版本不一致，具体参考<https://www.cnblogs.com/ztcad/p/14326230.html?ivk_sa=1024320u>；

## 01_创建类库
编写了一些常用的方法封装成类库，对方法进行了简单的测试，后续会慢慢完善:stuck_out_tongue_winking_eye:；

## 02_文字对齐
比较简单的一个功能，现在大多数插件都已具备该功能，不过多研究了:bowtie:；

## 03_统计线段长度
通过选择集去循环被选择对象，拿到被选择对象的终点，利用Curve类的getDistAtPoint方法，拿到线段长度:blush:；
