
;;菜单加载测试程序  看我的注释
(defun c:pt_menu (/ items items1)
  ;;'((标签 命令 帮助字串 次级菜单项)) 表为菜单项，注意命令后要有一个空格
 (vl-load-com) 
  ;第二级子菜单定义
  (setq	items1 (list '("梁图层"                              
		       "\003\003Q1 "                       
		       "功能简介，这里对程序没影响"
		      )
		     '("--" nil nil)
		     '("板图层"                               
		       "\003\003E1 "
		       "功能简介，这里对程序没影响"
		      )
	             
	       ))
(setq	items2 (list '("统计线长"                              
		       "\003\003TJXC "                       
		       "功能简介，这里对程序没影响"
		      )
		     '("--" nil nil)
		     '("统计面积"                               
		       "\003\003TJMJ "
		       "功能简介，这里对程序没影响"
		      )
	             
	       )
  ) 

  
  (setq	items
	 (list '("画直线梁"                                   ;;; 功能1 这几个字将会显示在菜单栏上
		 "\003\003HZL "                           ;;;注意003后面的为命令，命令后面加上空格
		 "功能简介，这里对程序没影响"
		)
	       '("文字对齐"                                  ;;;下面依次类推
		 "\003\003WZDQ "
		 " 功能简介，这里对程序没影响"
		)
	       	'("梁墙生中线"
		 "\003\003ZXX "
		 " 功能简介，这里对程序没影响"
		)
	       '("平行标注"
		 "\003\003DD "
		 " 功能简介，这里对程序没影响"
		)
	       
	       '("--" nil nil) 
	       (list "图层设置" nil nil items1) ;  这里插入下一级菜单，下一级菜单在开头已经定义，注意看 items1 这个变量   要加子菜单就定义一个items 然后插入一列
	       (list "统计工具" nil nil items2)
	       '("--" nil nil) 
	 )
  )
  (menu_pt-AddCassMenu
    "ACAD" 
    "个人工具库" ;_ 显示的Pop菜单项名称 菜单栏名称一定要改
    items 
    "工具箱" 
  )
  (princ)
)




;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;后面不用看了,写好的函数库;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;(menu_pt-RemoveMenuItem POPName) 移除下拉菜单，成功返回T
;;; 例如: (menu_pt-RemoveMenuItem "CASS工具") 移除 “CASS工具” 菜单
(defun menu_pt-RemoveMenuItem (POPName / MenuBar n i MenuItem Menu tag)
  (setq MenuBar (vla-get-menubar (vlax-get-acad-object)))
  ;; 找菜单 Item 
  (setq menuitem (menu_pt-CATCHAPPLY 'vla-item (list MenuBar POPName)))
  (if menuitem (menu_pt-CATCHAPPLY 'vla-RemoveFromMenuBar (list menuitem)))
)
;;函数 menu_pt-AddCassMenu 添加CASS菜单
;;;语法: (menu_pt-AddCassMenu MenuGroupName POPName PopItems InsertBeforeItem) 
;;MenuGroupName 要插入的菜单组名称
;;POPName 下拉菜单名称
;;PopItems 下拉菜单列表
;;   如 '((标签 命令 帮助字串 次级子项)...) 表为下拉菜单列表，注意命令后要有一个空格
;;InsertBeforeItem 在该菜单条名称之前插入，例如 "工具箱"，若为 nil,则插在最后
(defun menu_pt-AddCassMenu (MenuGroupName		  POPName
			PopItems     InsertBeforeItem
			/	     MENUBAR	  N
			I	     MENUITEM	  POPUPMENU
			K	     TMP	  SUBPOPUPMENU
		       )
  ;;卸载原有菜单
  (menu_pt-RemoveMenuItem POPName)

  (setq MenuBar (vla-get-menubar (vlax-get-acad-object)))
  (if InsertBeforeItem
    (progn
      ;; 查找菜单“工具箱”
      (setq n (vla-get-count MenuBar))
      (setq i (1- n))
      (while
	(and (>= i 0)			; 没有超过上限
	     (/= InsertBeforeItem
		 (vla-get-name (setq menuitem (vla-item MenuBar i)))
	     )				; 找到"工具箱"菜单条
	)
	 (setq i (1- i))
      )
      (if (< i 0)			; 如果没有文件菜单, 取最后一条菜单菜单
	(setq i (vla-get-count MenuBar))
      )
    )
    (setq i (vla-get-count MenuBar)) ;_  取最后一条菜单菜单
  )
  ;;创建"CASS工具"菜单条
  (if (not
	(setq popupmenu
	       (menu_pt-CATCHAPPLY
		 'vla-Item
		 (list
		   (vla-get-menus
		     (vla-item
		       (vla-get-MenuGroups (vlax-get-acad-object))
		       MenuGroupName ;_ "测量工具集" 菜单组名称
		     )
		   )
		   POPName ;_ "CASS工具" 下拉菜单名称
		 )
	       )
	)
      )
    (setq popupmenu
	   (vla-add
	     (vla-get-menus
	       (vla-item (vla-get-MenuGroups (vlax-get-acad-object))
			 MenuGroupName ;_ "测量工具集" 菜单组名称
	       )
	     )
	     POPName ;_ "CASS工具" 下拉菜单名称
	   )
    )
  )
  ;;清除Menu子项
  (vlax-for popupmenuitem popupmenu
    (vla-delete popupmenuitem)
  )
  ;;插入"CASS工具"菜单条
  (vla-InsertInMenuBar popupmenu i)
  (menu_pt-insertPopMenuItems popupmenu PopItems)
  (princ)
)

;;函数 menu_pt-insertPopMenuItems 逐项插入菜单条
;;语法: (menu_pt-insertPopMenuItems popupmenu PopItems)
;;popupmenu 菜单条vla对象
;;PopItems 下拉菜单列表
;;   如 '((标签 命令 帮助字串 次级子项)...) 表为下拉菜单列表，注意命令后要有一个空格
(defun menu_pt-insertPopMenuItems (popupmenu PopItems / K TMP)
  (setq k 0)
  (mapcar
    (function
      (lambda (x / Label cmdstr hlpstr subItems tmp)
	(setq Label    (car x)
	      cmdstr   (cadr x)
	      hlpstr   (caddr x)
	      subItems (cadddr x)
	)
	(if (= label "--")
	  ;; 插入分隔符
	  (vla-AddSeparator
	    popupmenu
	    (setq k (1+ k))
	  )
	  (if (and Label cmdstr)
	    ;; 插入菜单条
	    (progn
	      (setq tmp
		     (vla-addmenuitem
		       popupmenu
		       (setq k (1+ k))
		       Label
		       cmdstr
		     )
	      )
	      (vla-put-helpstring tmp hlpstr)
	    )
	    ;; 插入下一级子菜单
	    (progn
	      (setq tmp
		     (vla-addsubmenu
		       popupmenu
		       (setq k (1+ k))
		       Label
		     )
	      )
	      (if subItems ;_ 添加子级菜单
		(menu_pt-insertPopMenuItems tmp subItems)
	      )
	    )
	  )
	)
      )
    )
    ;;'((标签 命令 帮助字串 次级菜单项)) 表为菜单项，注意命令后要有一个空格
    PopItems
  )
)
;;函数 menu_pt-CatchApply 重定义 VL-CATCH-ALL-APPLY 
;;语法: (menu_pt-CatchApply fun args)
;;参数 fun 函数 如 distance or 'distance
;;     args 函数的参数表
;;返回值: 如函数运行错误返回nil,否则返回函数的返回值
(defun menu_pt-CatchApply (fun args / result)
  (if
    (not
      (vl-catch-all-error-p
	(setq result
	       (vl-catch-all-apply
		 (if (= 'SYM (type fun))
		   fun
		   (function fun)
		 )
		 args
	       )
	)
      )
    )
     result
  )
)


(c:pt_menu)
(princ)