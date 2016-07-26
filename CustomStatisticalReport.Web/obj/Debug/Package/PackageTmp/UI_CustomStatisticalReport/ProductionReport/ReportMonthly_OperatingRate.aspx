<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportMonthly_OperatingRate.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionReport.ReportMonthly_OperatingRate" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

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
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_CustomStatisticalReport/js/page/ReportMonthly.js" charset="utf-8"></script>
    <script>
        var g_templateURL = "/UI_CustomStatisticalReport/ReportTemplate/ReportMonthly_OperatingRate.html";
        var g_webmethod = "GetMaterialDatasetService";
    </script>
</head>
<body>
 <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div id="toolbar_ReportTemplate" data-options="region:'north'" style="height: 80px">
                    <table style="padding-top: 10px">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>生产线：</td>
                                        <td>
                                            <input id="productLineName" class="easyui-textbox" style="width: 100px;" readonly="true" /><input id="organizationId" readonly="true" style="display: none;" />
                                        </td>

                                        <td>时间：</td>
                                        <td>
                                            <input id="datetime" class="easyui-datebox" data-options="formatter:myformatter,parser:myparser" style="width: 100px;" />
                                        </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                                onclick="QueryReportFun();">查询</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                        </td>
                                        <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>

                <div data-options="region:'center',border:false" >
                    <%--<div style="outline-width:30px;outline-color:rgb(248, 245, 245);">--%>
                        <div id="contain" style="width: 1300px; height: 1000px; overflow: auto;border-top:20px solid rgb(166, 166, 166);border-right:30px solid rgb(166, 166, 166);border-bottom:20px solid rgb(166, 166, 166);border-left:30px solid rgb(166, 166, 166);padding:30px"></div>
                    <%--</div>--%>
                </div>
            </div>
        </div>
    </div>
    <style type="text/css">
        td {
            text-align: center;
            padding: 0;
            margin: 0;
        }

        input {
            text-align: center;
        }
    </style>
</body>
</html>
