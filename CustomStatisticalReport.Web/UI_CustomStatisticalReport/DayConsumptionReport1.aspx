<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DayConsumptionReport1.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.DayConsumptionReport" %>

<%@ Register Src="../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>能耗日报表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />
    <style>
        <!--
        /* Font Definitions */
        @font-face {
            font-family: 宋体;
            panose-1: 2 1 6 0 3 1 1 1 1 1;
        }

        @font-face {
            font-family: "Cambria Math";
            panose-1: 2 4 5 3 5 4 6 3 2 4;
        }

        @font-face {
            font-family: Calibri;
            panose-1: 2 15 5 2 2 2 4 3 2 4;
        }

        @font-face {
            font-family: "\@宋体";
            panose-1: 2 1 6 0 3 1 1 1 1 1;
        }
        /* Style Definitions */
        p.MsoNormal, li.MsoNormal, div.MsoNormal {
            margin: 0cm;
            margin-bottom: .0001pt;
            text-align: justify;
            text-justify: inter-ideograph;
            font-size: 10.5pt;
            font-family: "Calibri","sans-serif";
        }

        .MsoChpDefault {
            font-family: "Calibri","sans-serif";
        }
        /* Page Definitions */
        @page WordSection1 {
            size: 841.9pt 595.3pt;
            margin: 90.0pt 72.0pt 90.0pt 72.0pt;
            layout-grid: 15.6pt;
        }

        div.WordSection1 {
            page: WordSection1;
        }
        -->
    </style>


    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/page/DayConsumptionReport.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'north',split:true" style="height: 90px;">
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>分公司：</td>
                                        <td>
                                            <input id="TextBox_CompanyName" class="easyui-textbox" style="width: 180px;" readonly="true" /><input id="organizationId" readonly="true" style="display: none;" /></td>
                                        <td>时间：</td>
                                        <td>
                                            <input id="datetime" type="text" class="easyui-datebox" style="width: 120px;" required="required" /></td>
                                        <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
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
                        </tr>
                    </table>
                </div>
                <div id="ReportTable" class="WordSection1" style='layout-grid: 15.6pt' data-options="region:'center',border:false">
                    <table class="MsoNormalTable" border="0" cellspacing="0" cellpadding="0" width="1135"
                        style='width: 851.0pt; margin-left: 4.65pt; border-collapse: collapse'>
                        <tr style='height: 27.0pt'>
                            <td width="1135" nowrap colspan="15" style='width: 851.0pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 22.0pt; font-family: 宋体'>中材甘肃水泥能耗日报</span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 27.0pt'>
                            <td width="122" colspan="2" style='width: 91.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 12.0pt; font-family: 宋体'>区域划分</span></b>
                                </p>
                            </td>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 12.0pt; font-family: 宋体; color: black'>工序</span></b>
                                </p>
                            </td>
                            <td width="72" style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 12.0pt; font-family: 宋体; color: black'>用电量</span></b>
                                </p>
                            </td>
                            <td width="72" style='width: 53.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 12.0pt; font-family: 宋体; color: black'>生产量</span></b>
                                </p>
                            </td>
                            <td width="80" style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 12.0pt; font-family: 宋体; color: black'>工序电耗</span></b>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>分布电耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>综合电耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>煤粉消耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>综合煤耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>综合能耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>可比综合煤耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>可比综合电耗</span></b>
                                </p>
                            </td>
                            <td width="71" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 27.0pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'>可比综合能耗</span></b>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="318" colspan="4" style='width: 238.8pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>矿山</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id ="zc_nxjc_byc_byf_limestoneMine_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border: none; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border: none; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="71" rowspan="7" style='width: 53.1pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>熟料</span></b>
                                </p>
                            </td>
                            <td width="52" rowspan="2" style='width: 38.75pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>生料工段</span></b>
                                </p>
                            </td>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>原料预均化</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMaterialsHomogenize_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap rowspan="2" style='width: 53.85pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinker_MixtureMaterialsOutput"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMaterialsHomogenize_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap rowspan="2" style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMaterialsPreparation_ElectricityConsumption"
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border: solid windowtext 1.0pt; border-left: none; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_ElectricityConsumption_Comprehensive"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border: solid windowtext 1.0pt; border-left: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinker_PulverizedCoalInput"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_CoalConsumption_Comprehensive"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_EnergyConsumption_Comprehensive"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_ElectricityConsumption_Comparable"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_CoalConsumption_Comparable"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="7" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker_EnergyConsumption_Comparable"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>原料粉磨</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMaterialsGrind_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMaterialsGrind_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="52" rowspan="4" style='width: 38.75pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span 
                                        style='font-size: 11.0pt; font-family: 宋体'>熟料工段</span></b>
                                </p>
                            </td>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>煤粉制备</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_coalPreparation_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: none; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinker_PulverizedCoalOutput"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_coalPreparation_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap rowspan="4" style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinkerPreparation_ElectricityConsumption"
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>生料均化</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMealHomogenization_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap rowspan="4" style='width: 53.85pt; border: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinker_ClinkerOutput"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_rawMealHomogenization_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" nowrap colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>废气处理</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_kilnSystem_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_kilnSystem_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>熟料烧成</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinkerBurning_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinkerBurning_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="248" colspan="3" style='width: 185.7pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>辅助用电（分摊）</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_other_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_clinker01_other_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="71" rowspan="8" style='width: 53.1pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>水泥</span></b>
                                </p>
                            </td>
                            <td width="52" rowspan="7" style='width: 38.75pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>水泥工段</span></b>
                                </p>
                            </td>
                            <td width="76" rowspan="3" style='width: 57.15pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        lang="EN-US" style='font-size: 11.0pt; font-family: 宋体'>1#</span></b><b><span
                                            style='font-size: 11.0pt; font-family: 宋体'>水泥磨</span></b>
                                </p>
                            </td>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>熟料储存与输送</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_clinkerTransport_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap rowspan="3" style='width: 53.85pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_cement_CementOutput"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_clinkerTransport_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap rowspan="3" style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_cementPreparation_ElectricityConsumption"
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill_ElectricityConsumption_Comprehensive"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span 
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #DAEEF3; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill_EnergyConsumption_Comprehensive"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill_ElectricityConsumption_Comparable"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap rowspan="8" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; background: #FDE9D9; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill_EnergyConsumption_Comparable"
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>混合材制备</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_hybridMaterialsPreparation_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_hybridMaterialsPreparation_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>水泥粉磨</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_cementGrind_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill01_cementGrind_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="76" rowspan="3" style='width: 57.15pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        lang="EN-US" style='font-size: 11.0pt; font-family: 宋体'>2#</span></b><b><span
                                            style='font-size: 11.0pt; font-family: 宋体'>水泥磨</span></b>
                                </p>
                            </td>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span 
                                        style='font-size: 11.0pt; font-family: 宋体'>熟料储存与输送</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_clinkerTransport_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap rowspan="3" style='width: 53.85pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cement_CementOutput"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_clinkerTransport_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap rowspan="3" style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cementPreparation_ElectricityConsumption"
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>混合材制备</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_hybridMaterialsPreparation_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_hybridMaterialsPreparation_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="120" style='width: 89.8pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>水泥粉磨</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cementGrind_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cementGrind_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>水泥包装</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cementPacking_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span 
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_cementPacking_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="248" colspan="3" style='width: 185.7pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>辅助用电（分摊）</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_other_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cementmill02_other_ElectricityConsumption"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid windowtext 1.0pt; border-right: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="318" colspan="4" style='width: 238.8pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid windowtext 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>辅助用电</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_auxiliaryProduction_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border: none; border-bottom: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid windowtext 1.0pt; border-right: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border: solid windowtext 1.0pt; border-top: none; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="122" colspan="2" rowspan="4" style='width: 91.85pt; border-top: none; border-left: solid windowtext 1.0pt; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>余热发电</span></b>
                                </p>
                            </td>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>上网电量</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cogeneration01_electricityOutput_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>自用电量</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cogeneration01_electricityOwnDemand_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap valign="bottom" style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap valign="bottom" style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 14.25pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span
                                        style='font-size: 11.0pt; font-family: 宋体'>发电量</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span id="zc_nxjc_byc_byf_cogeneration01_clinkerElectricityGeneration_ElectricityQuantity"
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 12.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 14.25pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <span
                                        style='font-size: 11.0pt; font-family: 宋体'></span>
                                </p>
                            </td>
                        </tr>
                        <tr style='height: 13.5pt'>
                            <td width="196" colspan="2" style='width: 146.95pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="center" style='text-align: center'>
                                    <b><span 
                                        style='font-size: 11.0pt; font-family: 宋体'>吨熟料发电量</span></b>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span id="zc_nxjc_byc_byf_clinker01_clinkerElectricityGeneration_ElectricityConsumption"
                                        style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="72" nowrap style='width: 53.85pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="80" nowrap style='width: 60.25pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="97" nowrap style='width: 72.9pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                            <td width="71" nowrap style='width: 53.05pt; border-top: none; border-left: none; border-bottom: solid windowtext 1.0pt; border-right: solid windowtext 1.0pt; padding: 0cm 5.4pt 0cm 5.4pt; height: 13.5pt'>
                                <p class="MsoNormal" align="left" style='text-align: left'>
                                    <span style='font-size: 11.0pt; font-family: 宋体; color: black'></span>
                                </p>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <form id="formMain" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
