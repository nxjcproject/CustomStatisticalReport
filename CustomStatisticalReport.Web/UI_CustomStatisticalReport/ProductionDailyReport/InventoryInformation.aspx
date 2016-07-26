<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryInformation.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionDailyReport.InventoryInformation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="../../js/common/PrintFile.js"charset="utf-8"></script>

    <script type="text/javascript" src="js/InventoryInformation.js"charset="utf-8"></script>
</head>
<body>
        <div class="easyui-layout" data-options="fit:true,border:false">
                <div id="toolbar_head" data-options="region:'north'" style="height: 80px;display:none">
                    <table style="padding-top: 10px">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>日报时间：</td>
                                        <td>
                                            <input id="datetime" type="text" class="easyui-datebox"  required="required" style="width: 120px;" />
                                               </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                                onclick="QueryReportFun();">查询</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>                   
                    </table>
                </div>
              <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
            <table id="inventoryInformation"class="easyui-treegrid" style="width:935px;"></table>
        </div>
        <!-- 图表结束 -->
              <%-- <table id="inventoryInformation" class="easyui-treegrid" style="width:700px;height:400px" >   
                 <thead>   
                          <tr>     
                                  <th data-options="field:'factory',rowspan:2,width:180,align: 'center'">组织库存</th>        
                                  <th data-options=" colspan: 2, width: 250, align: 'center'">期初库存</th> 
                                  <th data-options="colspan: 2, width: 250, align: 'center'">期末库存</th>       
                                  <th data-options="field: 'MaxInventory',rowspan: 2, width: 100, align: 'center'">最大库存</th>     

                          </tr>   
                     <tr>
                                  <th data-options="field: 'Beginventory', width: 100, align: 'center' ">月初库存</th>        
                                  <th data-options="field: 'YearBeinventory',  width: 100, align: 'center'">年初库存</th> 
                                  <th data-options=" field: 'Endinventory', width: 100, align: 'center' ">月末库存</th>       
                                  <th data-options="field: 'YearEndInventory', width: 100, align: 'center'">年末库存</th>     

                     </tr>
                  </thead>
                </table>--%>
            </div>
   
  
 
       <%--  columns: [[
                 { field: 'factory', title: '组织库存', rowspan: 2, width: 100, align: 'center' },
                 { title: '期初库存', colspan: 2, width: 250, align: 'center' },
                 { title: '期末库存', colspan: 2, width: 250, align: 'center' },
                 { field: 'MaxInventory', title: '最大库存', rowspan: 2, width: 100, align: 'center' }
            ], [ { field: 'Beginventory', title: '月初库存', width: 100, align: 'center' },
                 { field: 'YearBeinventory', title: '年初库存', width: 100, align: 'center' },
                 { field: 'Endinventory', title: '月末库存', width: 100, align: 'center' },
                 { field: 'YearEndInventory', title: '年末库存', width: 100, align: 'center' }]],--%>



    <form id="form1" runat="server">
    <div>  
    </div>
    </form>
</body>
</html>
