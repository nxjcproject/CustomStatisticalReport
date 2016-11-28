var SelectOrganizationName = "";
var SelectDatetime = "";

$(function () {
    var myHtmlStr = CreatHtmlStr();
    InitDate();
    // loadGridData("last", "", "");
    $('#gridMain_ReportTemplate').datagrid({ 'onLoadSuccess': onLoadSuccess });
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = nowDate.getFullYear() + '-' + (nowDate.getMonth()) + '-' + nowDate.getDate();
    $('#startDate').datebox('setValue', beforeString);
    $('#endDate').datebox('setValue', nowString);
}

function CreatHtmlStr() {
    var htmlStr = '';
    $.ajax({
        type: "POST",
        url: "report_CementStatistics.aspx/GetHtml",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            htmlStr = msg.d;

            $('#ContainID').html(htmlStr);
            $.parser.parse('#ContainID');
        },
        error: handleError
    });
    return htmlStr;
}

function QueryReportFun() {
    var startDate = $('#startDate').datebox('getValue');
    var endDate = $('#endDate').datebox('getValue');
    loadGridData("last", startDate, endDate);
    SelectDatetime = startDate + ' 至 ' + endDate;
}
function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function loadGridData(myType, startDate, endDate) {
    if (myType == 'first') {
        $('#gridMain_ReportTemplate').datagrid('loadData', []);
    }
    else {
        $.ajax({
            type: "POST",
            url: "report_CementStatistics.aspx/GetReportData",
            data: '{startDate: "' + startDate + '", endDate: "' + endDate + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                m_MsgData = jQuery.parseJSON(msg.d);
                //_data = m_MsgData;
                //InitializeGrid(m_MsgData);
                $('#gridMain_ReportTemplate').datagrid('loadData', m_MsgData);
            },
            error: handleError
        });
    }
}

function onLoadSuccess(data) {
    //var merges = [];
    $.ajax({
        type: "POST",
        url: "report_CementStatistics.aspx/GetMerges",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var merges = msg.d;

            //$('#gridMain_ReportTemplate').datagrid('mergeCells', {
            //            index: 0,
            //            field: "zc_nxjc_byc_byf_clinker01_ElectricityQuantity",
            //            colspan: 2
            //        });
            for (var i = 0; i < merges.length; i++) {
                $('#gridMain_ReportTemplate').datagrid('mergeCells', {
                    index: merges[i].index,
                    field: merges[i].field,
                    colspan: merges[i].rowspan
                });
            }
        }
    });
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("gridMain_ReportTemplate", "水泥电耗统计报表", SelectDatetime);
    var m_Parameter2 = "";

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "report_CementStatistics.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
function PrintFileFun() {
    var m_ReportTableHtml = GetDataGridTableHtml("gridMain_ReportTemplate", "水泥电耗统计报表", SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}
