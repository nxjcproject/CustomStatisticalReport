<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_CementStatistics.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.report_CementStatistics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>水泥电耗报表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 
    <script type="text/javascript" src="js/page/report_CementStatistics.js" charset="utf-8"></script>
</head>
<body>

    <%--<div class="easyui-panel" data-options="fit:true" style="height:1000px">--%>
    <div id="toolbar_ReportTemplate">
        <div style="padding-left:5px;">
        <span>开始时间 &nbsp<input id="startDate" type="text" class="easyui-datebox" required="required" style="width: 100px;" /></span>
        <span>&nbsp &nbsp 结束时间 &nbsp<input id="endDate" type="text" class="easyui-datebox" required="required" style="width: 100px;" /></span>
        <span><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'"
            onclick="QueryReportFun();">查询</a></span>
    </div>
    <div>
        <span><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a></span>
        <span><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a></span>
    </div>
    </div>
    <div id="ContainID" class="easyui-panel" data-options="fit:true" style="margin:5px;">
        <%--<table id="gridMain_ReportTemplate" class="easyui-datagrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true,fit:true" style="border: 1px; background-color: red">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:120,rowspan:3">名称</th>
                    <th data-options="field:'zc_nxjc_byc',width:120,colspan:2">白银公司</th>
                    <th data-options="field:'zc_nxjc_qtx',width:480,colspan:8">青铜峡公司</th>
                </tr>
                <tr>
                    <th data-options="field:'zc_nxjc_byc_byf_clinker01',width:120,colspan:2">1号熟料</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker02',width:120,colspan:2">2号窑</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker03',width:120,colspan:2">3号窑</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker04',width:120,colspan:2">4号窑</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker05',width:120,colspan:2">5号窑</th>
                </tr>
                <tr>
                    <th data-options="field:'zc_nxjc_byc_byf_clinker01_ElectricityQuantity',width:60,colspan:1">电量</th>
                    <th data-options="field:'zc_nxjc_byc_byf_clinker01_ElectricityConsumption',width:60,colspan:1">电耗</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker02_ElectricityQuantity',width:60,colspan:1">电量</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker02_ElectricityConsumption',width:60,colspan:1">电耗</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker03_ElectricityQuantity',width:60,colspan:1">电量</th>
                    <th data-options="field:'zc_nxjc_qtx_efc_clinker03_ElectricityConsumption',width:60,colspan:1">电耗</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker04_ElectricityQuantity',width:60,colspan:1">电量</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker04_ElectricityConsumption',width:60,colspan:1">电耗</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker05_ElectricityQuantity',width:60,colspan:1">电量</th>
                    <th data-options="field:'zc_nxjc_qtx_tys_clinker05_ElectricityConsumption',width:60,colspan:1">电耗</th>
                </tr>
            </thead>
        </table>--%>
    </div>
</body>
</html>
