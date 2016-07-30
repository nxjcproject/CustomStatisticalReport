var SelectOrganizationName = "";
$(function () {
    Loaddatagrid({ "rows": [], "total": 0 });
    InitDate();
});
// datetime datebox
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 5);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() ;
    $('#startDate').datebox('setValue', beforeString);
    $('#endDate').datebox('setValue', nowString);
}
function onOrganisationTreeClick(node) {
    $('#TextBox_OrganizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
}
function QueryReportFun() {
    var m_OrganizationId = $('#organizationId').val();
    SelectOrganizationName = $('#TextBox_OrganizationName').textbox('getText');
    var mStartDate= $("#startDate").datebox("getValue");;
    var mEndDate = $("#endDate").datebox("getValue");;
    var megar = $.messager.alert('提示','数据加载中...');
    if (m_OrganizationId != undefined && m_OrganizationId != "" && mStartDate != undefined && mStartDate != "" && mEndDate != undefined && mEndDate != "") {
        $.ajax({
            type: "POST",
            url: "report_PowerConsumptionComparison.aspx/GetReportData",
            data: '{organizationId: "' + m_OrganizationId + '", mStartDate: "' + mStartDate + '", mEndDate: "' + mStartDate + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                megar.window('close');
                var m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData.total == 0) {
                    $('#datagrid_ReportTable').datagrid("loadData", []);
                } else {
                    $('#datagrid_ReportTable').datagrid("loadData", m_MsgData);
                    myMergeCell("datagrid_ReportTable", "ProcessName");
                }
            },
            beforeSend: function(XMLHttpRequest){
                megar;
            },
            error: function () {
                $("#datagrid_ReportTable").datagrid('loadData', []);
                $.messager.alert('失败', '获取数据失败');
            }
        });
    }
    else {
        alert("您没有选择分厂或者未选择时间!");
    }

}
function Loaddatagrid(myData) {
    try {
        $('#datagrid_ReportTable').datagrid({
            data: myData,
            dataType: "json",
            //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
            idField: 'id',
            treeField: 'Name',
            rownumbers: true,
            singleSelect: true,
            frozenColumns: [[
                 { width: '80', title: '产线', field: 'ProcessName' },
                { width: '80', title: '工序名称', field: 'Type' }, ]],
            columns: [[
               // { width: '100', title: '变量ID', field: 'VariableId', hidden: true },
                { width: '100', title: '组织机构层次码', field: 'OrganizationId', hidden: true },
              //  { width: '100', title: '层次码', field: 'LevelCode', hidden: true },                      
                { width: '120', title: '电量', field: 'ElectricityQuantity' },
                { width: '100', title: '产量', field: 'MaterialWeight'},
                { width: '120', title: '电耗', field: 'PowerConsumption' },
                { width: '100', title: '计算电耗', field: 'CalculationPowerConsumption' },
                { width: '120', title: '综合电耗', field: 'ComprehensivePowerConsumption' }
            ]],
            toolbar: '#toolbar_ReportTable'
        });
    }
    catch (e) {
    }
}
function RefreshFun() {
    QueryReportFun();
}
function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetTreeTableHtml("datagrid_ReportTable", "能耗日报", "Name", SelectOrganizationName, SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "DayConsumptionReport.aspx");

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
    var m_ReportTableHtml = GetTreeTableHtml("datagrid_ReportTable", "能耗日报", "Name", SelectOrganizationName, SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}



//合并单元格
function myMergeCell(myDatagridId, columnName) {
    merges = getMergeCellArray(myDatagridId, columnName);
    var columnNameList = new Array("ProcessName", "CalculationPowerConsumption", "ComprehensivePowerConsumption");
    doMergeCell(myDatagridId, columnNameList, merges);
}
//获取需要合并单元格的数组信息
function getMergeCellArray(myDatagridId, columnName) {
    var myDatagrid = $('#' + myDatagridId);
    var merges = [];
    var myDatas = myDatagrid.datagrid('getData');
    var myRows = myDatas["rows"];
    var length = myRows.length;
    var beforeValue;
    //参数
    var count = 0;//merges数组个数
    var rowspan = 0;
    var index = 0;
    for (var i = 0; i < length; i++) {
        var currentValue = myRows[i][columnName];
        //第一个要特殊处理
        if (i == 0) {
            beforeValue = currentValue;
        }
        //前一行和后一行相同时累加数加一
        if (currentValue == beforeValue) {
            rowspan++;
        }
        else {
            //当rowspan为1时不用合并单元格
            if (rowspan > 1) {
                merges.push({ "rowspan": rowspan, "index": index });
            }
            beforeValue = currentValue;
            index = i;
            //初始化rowspan
            rowspan = 1;
        }
        //最后一个也要特殊处理
        if ((length - 1) == i && rowspan > 1) {
            merges.push({ "rowspan": rowspan, "index": index });
        }
    }
    return merges;
}
//合并单元格（多列）
function doMergeCell(myDatagridId, columnNameArray, merges) {
    var myDatagrid = $('#' + myDatagridId);
    var myLength = merges.length;
    for (var i = 0; i < myLength; i++) {
        for (var j = 0; j < columnNameArray.length; j++) {
            myDatagrid.datagrid('mergeCells', {
                index: merges[i].index,
                field: columnNameArray[j],
                rowspan: merges[i].rowspan
            });
        }    
    }
}
