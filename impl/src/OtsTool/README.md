#OtsTool

调用例子

最小化参数调用

OtsTool.exe -t table_name -f file_name.xls

导出某些列

OtsTool.exe -t table_name -f file_name.xls -c 列名1[:列类型1],列名2,列名3[,...],列名n

OtsTool.exe -t table_name -f file_name.xls -c _Project,_Category,_Id,_CreatDate:DateTime,Keywords,PageIndex,PageSize

主键筛选

OtsTool.exe -t table_name -f file_name.xls -p 主键1:最小值1[:最大值2],主键2:固定值2

OtsTool.exe -t table_name -f file_name.xls -p _Project:project_name1,_Category:category_name1,_Id:id10:id20
