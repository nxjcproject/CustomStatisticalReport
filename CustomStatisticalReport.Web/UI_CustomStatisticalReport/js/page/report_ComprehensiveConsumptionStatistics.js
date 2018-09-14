var SelectDatetime = "";
var SelectOrganizationName = "";

$(function () {
    InitDate();
    loadDataGrid("first");
});

//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    beforeDate.setMonth(beforeDate.getMonth() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate();
    $('#startTime').datebox('setValue', beforeString);
    $('#endTime').datebox('setValue', nowString);
}

function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_ComprehensiveConsumption').datagrid({
            columns: [[
                    { field: 'CompanyName', title: '公司', width: 90 },
                    { field: 'OrganizationID', title: '组织机构', width: 100, hidden: true },
                    { field: 'FactoryName', title: '分厂', width: 90 },
                    { field: 'TotalElectricityConsumptionOfClinker', title: '熟料总耗电量', width: 90, align: 'right' },
                    { field: 'TotalElectricityConsumptionOfCement', title: '水泥总耗电量', width: 90, align: 'right' },
                    { field: 'ClikerOutput', title: '熟料产量', width: 80, align: 'right' },
                    { field: 'CemmentOutput', title: '水泥产量', width: 80, align: 'right' },
                    { field: 'CemmentOfClinkerInput', title: '自产熟料消耗量', width: 100, align: 'right' },
                    { field: 'CemmentOfClinkerOutsourcingInput', title: '外购熟料消耗量', width: 100, align: 'right' },
                    { field: 'clinkerComprehensiveConsumption', title: '熟料综合电耗', width: 90, align: 'right' },
                    { field: 'cementComprehensiveConsumption', title: '水泥综合电耗', width: 90, align: 'right' },
                    { field: 'cementConsumption', title: '水泥单耗电耗', width: 90, align: 'right' }
            ]],
            fit: true,
            toolbar: "#toolbar_ComprehensiveConsumption",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        })
    }
    else {
        $('#grid_ComprehensiveConsumption').datagrid("loadData", myData);
    }
}

function Query() {
    var startTime = $('#startTime').datetimebox('getValue');//开始时间
    var endTime = $('#endTime').datetimebox('getValue');//结束时间
    SelectDatetime = startTime + ' 至 ' + endTime;
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "report_ComprehensiveConsumptionStatistics.aspx/GetComprehensiveConsumptionData",
        data: "{mStartTime:'" + startTime + "',mEndTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                $('#grid_ComprehensiveConsumption').datagrid("loadData", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_ComprehensiveConsumption").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("grid_ComprehensiveConsumption", "综合电耗统计", SelectDatetime);
    var m_Parameter2 = "各单位";

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "report_ComprehensiveConsumptionStatistics.aspx");

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