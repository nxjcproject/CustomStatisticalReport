<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesInformation.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionDailyReport.SalesInformation" %>

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

    <script type="text/javascript" src="js/SalesInformation.js"  charset="utf-8"></script>
      <style type="text/css">
        .table {
            border-collapse: collapse;
        }
        .table .td  {
            border: 1px solid black;
        }
    </style>
</head>
<body>
        <div class="easyui-layout" data-options="fit:true,border:false">
                <div id="toolbar_head" data-options="region:'north'" style="height: 80px;">
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
                       <table id="SalesInformation" class="table">     
                   <thead>   
                          <tr>     
                                  <th class="td" rowspan="2"style="width:80px">分公司</th> 
                                  <th class="td" rowspan="2"style="width:80px">分厂</th>     
                                  <th class="td" rowspan="2"style="width:80px">品种</th>            
                                  <th class="td" colspan="2"style="width:160px">销售计划</th> 
                                  <th class="td" colspan="3"style="width:240px">产量</th>       
                                  <th class="td" colspan="3"style="width:240px">销售量</th>    
                                  <th class="td" colspan="3"style="width:240px">出厂量</th>    

                          </tr>   
                          <tr>
                                  <th class="td">月销售计划</th>        
                                  <th class="td">年销售计划</th> 
                                  <th class="td">日计</th>       
                                  <th class="td">月计</th>  
                                  <th class="td">年累计</th>  
                                  <th class="td">日计</th>       
                                  <th class="td">月计</th>  
                                  <th class="td">年累计</th> 
                                  <th class="td">日计</th>       
                                  <th class="td">月计</th>  
                                  <th class="td">年累计</th>  
                         </tr>
                  </thead>
                           <tbody>
                               <%--青铜峡--%>
                                    <tr>
                                       <td class="td" rowspan="4" style="text-align:center">青铜峡公司</td><td  class="td" rowspan="2"style="text-align:center">二分厂</td><td class="td"style="text-align:center">熟料</td><td class="td"></td><td class="td"></td>
                                        <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                    </tr>
                                   <tr>
                                      <td class="td"style="text-align:center">水泥</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                   <tr>
                                        <td class="td" rowspan="2"style="text-align:center">太阳山</td><td class="td"style="text-align:center">熟料</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                   <tr>
                                       <td class="td" style="text-align:center">水泥</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                               <%--银川--%>
                                <tr>
                                <td class="td" rowspan="6" style="text-align:center">银川公司</td><td  class="td" rowspan="2"style="text-align:center">一分厂</td><td class="td"style="text-align:center">熟料</td><td class="td"></td><td class="td"></td>
                                <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                               </tr>
                                   <tr>
                                      <td class="td"style="text-align:center">水泥</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                   <tr>
                                        <td class="td" rowspan="2"style="text-align:center">兰山分厂</td><td class="td"style="text-align:center">熟料</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                   <tr>
                                       <td class="td" style="text-align:center">水泥</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                    <tr>
                                        <td class="td" rowspan="2"style="text-align:center">宁东分厂</td><td class="td"style="text-align:center">熟料</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                                   <tr>
                                       <td class="td" style="text-align:center">水泥</td><td class="td"></td><td class="td"></td>  <td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td><td class="td"></td>
                                   </tr>
                           </tbody>
             

                       </table>
                   </div>
            </div>
</body>
</html>
