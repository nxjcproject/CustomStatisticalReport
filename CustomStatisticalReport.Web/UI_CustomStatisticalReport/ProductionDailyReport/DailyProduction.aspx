<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DailyProduction.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionDailyReport.DailyProduction" %>
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
    <script type="text/javascript" src="../../js/common/PrintFile.js"charset="utf-8"></script>

    <script type="text/javascript" src="js/DailyProduction.js"charset="utf-8"></script>
</head>
<body>
 
        <div class="easyui-layout" data-options="fit:true,border:false">
              <div data-options="region:'west',split:true,title:'组织机构树'" style="width: 150px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
                <div id="toolbar_head" data-options="region:'north'" style="height: 80px;display:none">
                    <table style="padding-top: 10px">
                        <tr>
                            <td>
                                <table>                                   
                                    <tr>
                                        <td>组织机构</td>
                                        <td style="width: 110px;">
                                            <input id="OrganizationNameF" class="easyui-textbox" style="width: 100px;" readonly="readonly" /><input id="OrganizationIdF" readonly="readonly" style="display: none;" />
                                        </td>
                                        <td>日报时间</td>
                                        <td style="width: 130px;">
                                            <input id="DatetimeF" type="text" class="easyui-datebox"  required="required" style="width: 110px;" />
                                               </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'"
                                                onclick="QueryReportFun();">查询</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>                   
                    </table>
                </div>
              <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
                  <table id="DailyProduction" class="easyui-treegrid" ></table>   
              </div>  
         </div>

</body>
</html>