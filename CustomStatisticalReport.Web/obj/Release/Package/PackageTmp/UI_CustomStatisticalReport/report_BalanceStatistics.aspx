<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_BalanceStatistics.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.report_BalanceStatistics" %>

<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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

    <script type="text/javascript" src="js/page/report_BalanceStatistics.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/page/FormulaParse.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div id="toolbar_ReportTemplate" data-options="region:'north'" style="height: 75px">
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>生产线：</td>
                                        <td>
                                            <input id="productLineName" class="easyui-textbox" style="width: 100px;" readonly="true" /><input id="organizationId" readonly="true" style="display: none;" /></td>
                                        <%--<td>时间：</td>
                                        <td>
                                            <input id="datetime" class="easyui-datebox" data-options="formatter:myformatter,parser:myparser" style="width: 100px;" /></td>--%>
                                        <td>类型：
                                            <select id="cc" class="easyui-combobox" name="dept" data-options="panelHeight:'auto'" style="width: 100px;">
                                                <option value="MeterReading">抄表时间</option>
                                                <option value="Month">整月</option>
                                            </select>
                                        </td>
                                        <td>上次抄表：<select id="beforeId" class="easyui-combogrid" style="width: 150px"></select>
                                        </td>
                                        <td>本次抄表：<select id="nowId" class="easyui-combogrid" style="width: 150px"></select>
                                        </td>
                                        <td>
                                            <a href="#" style="margin-left: 10px" class="easyui-linkbutton" data-options="iconCls:'ext-icon-calculator',plain:true" onclick="CalculateDiffValue();">计算差值：</a>
                                            <span id="diffValueId" style="width: 205px; border: 1px;"></span>
                                        </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                                onclick="QueryReportFun();">查询</a>
                                        </td>
                                        <%--<td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                            onclick="QueryReportFun();">查询</a>
                                        </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true"
                                                onclick="SaveData();">保存</a>
                                        </td>--%>
                                    </tr>
                                    <tr>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            请选择保存时间：<input id="saveDateTime" class="easyui-datebox" data-options="formatter:myformatter,parser:myparser" style="width: 100px;" />
                                        </td>
                                        <td>
                                            <a id="save" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true"
                                                onclick="SaveData();">保存</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>

                        </tr>
                        <%-- <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <a href="#" class="easyui-linkbutton" iconcls="icon-reload" plain="true" onclick="RefreshFun();">刷新</a>
                                        </td>
                                        <td>
                                            <div class="datagrid-btn-separator"></div>
                                        </td>
                                        <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                        </td>
                                        <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>--%>
                    </table>
                    <%-- <fieldset>
                        <legend>参考值</legend>
                        上次抄表：<select id="beforeId" class="easyui-combogrid" style="width: 150px"></select>
                        本次抄表：<select id="nowId" class="easyui-combogrid" style="width: 150px"></select>
                        <a href="#" style="margin-left: 10px" class="easyui-linkbutton" data-options="iconCls:'ext-icon-calculator',plain:true" onclick="CalculateDiffValue();">计算差值：</a>
                        <span id="diffValueId" style="width: 205px; border: 1px;"></span>
                    </fieldset>--%>
                </div>

                <div data-options="region:'center',border:false">
                    <div id="contain" style="width: 1300px; height: 1000px; overflow: auto"></div>
                </div>
            </div>
        </div>
    </div>
    <%--    <div id="contain" style="width: 100px"></div>--%>
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
