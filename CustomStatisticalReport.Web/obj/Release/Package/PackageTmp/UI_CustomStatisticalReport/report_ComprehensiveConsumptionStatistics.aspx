<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_ComprehensiveConsumptionStatistics.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.report_ComprehensiveConsumptionStatistics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>综合电耗统计</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/page/report_ComprehensiveConsumptionStatistics.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:true, collapsible:false, split:false">
            <table id="grid_ComprehensiveConsumption"></table>
        </div>
        <div id="toolbar_ComprehensiveConsumption" style="display: none;">
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td style="width: 60px; text-align: right;">开始时间</td>
                                <td>
                                    <input id="startTime" type="text" class="easyui-datebox" style="width: 100px" required="required" />
                                </td>
                                <td style="width: 60px; text-align: right;">结束时间</td>
                                <td>
                                    <input id="endTime" type="text" class="easyui-datebox" style="width: 100px" required="required" />
                                </td>
                                <td style="width: 70px; text-align: right;">
                                    <a id="mSelectBtn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                                </td>
                                <td style="width: 80px; text-align: right;">
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <form id="form1" runat="server"></form>
</body>
</html>
